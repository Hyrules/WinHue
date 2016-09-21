using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using HueLib;
using HueLib2;

namespace WinHuebieLight
{
    class LightSetter
    {
        protected ScreenScanner scanner;

        protected Bridge bridge;

        /// <summary>
        /// Worker to chose lightstate from the scanner (depending on current mode)
        /// </summary>
        protected Thread worker = null;

        private bool bRun = false;
        
        /// <summary>
        /// Last State that has been set
        /// </summary>
        protected State lastState = null;

        protected UInt32 lightUpdatesSent = 0;

        public void Init(ScreenScanner scanner)
        {
            
            if (scanner == null)
                throw new Exception();
            this.scanner = scanner;

            WinHueSettings whs = new WinHueSettings();
            if (string.IsNullOrEmpty(whs.settings.DefaultBridge) || string.IsNullOrWhiteSpace(whs.settings.DefaultBridge))
                throw new Exception("Bridge not configured. Use WinHue to pair the bridge");

            if (!whs.settings.BridgeInfo.ContainsKey(whs.settings.DefaultBridge))
                throw new Exception("Bridge not configured. Use WinHue to pair the bridge");


            IPAddress ip = IPAddress.Parse(whs.settings.BridgeInfo[whs.settings.DefaultBridge].ip);
            bridge = new Bridge(ip, whs.settings.DefaultBridge, whs.settings.BridgeInfo[whs.settings.DefaultBridge].apiversion, whs.settings.BridgeInfo[whs.settings.DefaultBridge].swversion, whs.settings.BridgeInfo[whs.settings.DefaultBridge].apikey);

            /*Test if lights are available*/
            foreach (int lid in Settings.lights)
            {
                Light l = (bridge.GetLight(lid.ToString()));
                if (null == l)
                    throw new Exception("Light " + lid + " was not found in the bridge. Use WinHue to check your setup");
            }
        }

        public void Start()
        {
            if (worker != null)
                throw new Exception("Worker already started");
            bRun = true;
            worker = new Thread(LightSetterProcess);
            worker.Start();
        }

        public void Stop()
        {
            if (worker == null)
                throw new Exception("Worker not running");


            bRun = false;
            worker.Join();
            worker = null;
        }
        public UInt32 LightUpdatesSent
        { get { lock(this) return lightUpdatesSent; } }
        protected void LightSetterProcess()
        {
            while (bRun)
            {
                System.Threading.Thread.Sleep(10);

                State setState;

                switch (Settings.scanMethod)
                {
                    case ScanMethod.HueBoost:
                        { /*methodb*/
                            ColorRGB huedColor = new ColorRGB(scanner.AvarageSaturatedColor);
                            //ColorRGB totalColor = new ColorRGB(scanner.AvarageColor);
                            float luma = scanner.AvarageLuma;
                            setState = new State()
                            {
                                hue = (ushort)(huedColor.H * ushort.MaxValue),
                                sat = (byte)(huedColor.S * 255),
                                bri = (byte)(luma * 255),
                                on = true,
                                transitiontime = Settings.transitiontime
                            };

                            break;
                        }
                    case ScanMethod.Avarage:
                        {
                            //total avarages
                            ColorRGB c = new ColorRGB(scanner.AvarageColor);
                            setState = new State()
                            {
                                hue = (ushort)(c.H * ushort.MaxValue),
                                sat = (byte)(c.S * 255),
                                bri = (byte)(c.L * 255),
                                on = true,
                                transitiontime = Settings.transitiontime
                            };
                            break;
                        }
                    case ScanMethod.LumaOnly:
                            setState = new State()
                            {
                                bri = (byte)(scanner.AvarageLuma*255),
                                on = true,
                                transitiontime = Settings.transitiontime
                            };
                        break;
                    default:
                        {
                            throw new Exception("Unexpected scanmode in settings");
                        }
                }

                /*Do luma boost/reduction and limit min max light levels*/
                setState.bri = (byte)Math.Max(Settings.minLight,Math.Min((float)setState.bri * Settings.luminuosity, Settings.maxLight));

                /*Do sat boost/reduction*/
                if (Settings.saturation != 1)
                    setState.sat = (byte)Math.Max(0, Math.Min((float)setState.sat * Settings.saturation, 255));

                /*Decide if change shall be send to the bridge*/
                bool bSendUpdate = false;

                if (lastState == null) /*Send update if none sent yet*/
                {
                    bSendUpdate = true;
                }
                else if (DateTime.Now.Second % 5 == 0) /*Force send update every 5 seconds*/
                {
                    bSendUpdate = true;
                }
                else /*Decide if scene changes are big enough to send an update*/
                {
                    float delta = 0;
                    delta += Math.Abs(((float)lastState.bri - (float)setState.bri) / Byte.MaxValue);
                    delta += Math.Abs(((float)lastState.hue - (float)setState.hue) / ushort.MaxValue);
                    delta += Math.Abs(((float)lastState.sat - (float)setState.sat) / Byte.MaxValue);
                    delta /= 3;
                    if (delta >= Settings.minDelta)
                        bSendUpdate = true;
                }

                if (bSendUpdate)
                {
                    lastState = setState;
                    foreach (int lid in Settings.lights)
                    {
                        bridge.SetLightState(lid.ToString(), setState);
                    }

                    foreach(int gid in Settings.groups )
                    {
                        HueLib2.Action a = new HueLib2.Action();
                        a.on = true;
                        a.sat = setState.sat;
                        a.hue = setState.hue;
                        a.bri = setState.bri;

                        bridge.SetGroupAction(gid.ToString(), a);
                    }

                    lock( this )
                        lightUpdatesSent++;
                }
            }
        }


    }
}
