using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Xml;
using System.Xml.Serialization;
using ManagedUPnP;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.Communication2;

namespace WinHue3.Philips_Hue
{
    public enum HueObjectType { lights, groups, scenes, schedules, resourcelinks, rules, sensors }
    /// <summary>
    /// Hue Class.
    /// </summary>
    public static class Hue
    {

        private static readonly BackgroundWorker _ipscanBgw = new BackgroundWorker();
        private static readonly BackgroundWorker _detectionBgw = new BackgroundWorker();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static Hue()
        {
            _ipscanBgw.DoWork += _ipscanBgw_DoWork;
            _ipscanBgw.ProgressChanged += _ipscanBgw_ProgressChanged;
            _ipscanBgw.RunWorkerCompleted += _ipscanBgw_RunWorkerCompleted;
            _ipscanBgw.WorkerSupportsCancellation = true;
            _ipscanBgw.WorkerReportsProgress = true;
            _detectionBgw.DoWork += _detectionBgw_DoWork;
            _detectionBgw.RunWorkerCompleted += _detectionBgw_RunWorkerCompleted;

        }

        private static void _detectionBgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnDetectionComplete?.Invoke(null, e);
        }

        private static void _detectionBgw_DoWork(object sender, DoWorkEventArgs e)
        {
            log.Info("Starting bridge detection...");

            
            Dictionary<string, BasicConfig> newdetectedBridge = new Dictionary<string, BasicConfig>();

            // Detect using UPNP

            try
            {
                ServiceController sc = new ServiceController("SSDPSRV");

                if (sc.Status == ServiceControllerStatus.Running)
                {
                    log.Info("Starting UPNP detection of bridges...");
                    try
                    {
                        List<ManagedUPnP.Device> upnpDevices =
                            Discovery.FindDevices(null, 3000, int.MaxValue, out bool finished).ToList();

                        foreach (ManagedUPnP.Device dev in upnpDevices)
                        {
                            if (!dev.ModelName.Contains("Philips hue bridge")) continue;
                            Bridge bridge = new Bridge() { IpAddress = IPAddress.Parse(dev.RootHostName) };
                            BasicConfig bresult = bridge.GetBridgeBasicConfig();
                            if (bresult != null)
                            {
                                log.Info($"Bridge found : {bridge.Name} at {bridge.IpAddress} ");
                                newdetectedBridge.Add(dev.RootHostName, bresult);
                            }

                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    log.Info("Ending UPNP detection of bridges...");
                }

            }
            catch (Exception ex)
            {
                log.Error($"UPNP detection error : {ex.Message}");
            }

            // If not bridge are found via upnp try the portal.
            log.Info("Starting bridge portal detection...");
            if (newdetectedBridge.Count == 0)
            {

                // Detect using Portal
                HttpResult comres = HueHttpClient.SendRequest(new Uri("https://discovery.meethue.com"), WebRequestType.Get);
                if (comres.Success)
                {
                        List<HueDevice> portalDevices = Serializer.DeserializeToObject<List<HueDevice>>(comres.Data);
                        foreach (HueDevice dev in portalDevices)
                        {
                            if (newdetectedBridge.ContainsKey(dev.internalipaddress)) continue;
                            Bridge bridge = new Bridge() {IpAddress = IPAddress.Parse(dev.internalipaddress) };
                            BasicConfig bresult = bridge.GetBridgeBasicConfig();
                            if (bresult != null)
                            {
                                log.Info($"Bridge found : {bridge.Name} at {bridge.IpAddress} ");
                                newdetectedBridge.Add(dev.internalipaddress, bresult);
                            }
                            
                        }
                }
                else
                {
                    log.Info($"Dns resolution failure. (AKA unable to connect to the bridge");
                    OnPortalDetectionTimedOut?.Invoke(null, new DetectionErrorEventArgs(comres.Data));
                    OnBridgeDetectionFailed?.Invoke(null, new DetectionErrorEventArgs(comres.Data));
                }
            }
            log.Info("Ending bridge portal detection...");

            Dictionary<string, Bridge> bridges = newdetectedBridge.Select(kvp => new Bridge
            {
                IpAddress = IPAddress.Parse(kvp.Key),
                Mac = kvp.Value.mac,
                ApiVersion = kvp.Value.apiversion,
                SwVersion = kvp.Value.swversion,
                Name = kvp.Value.name ?? ""
            }).ToDictionary(p => p.Mac, p => p);

            // Process all bridges to get needed settings.
            e.Result = bridges;
            log.Info("Ending bridge detection.");
        }

        /// <summary>
        /// Detect the available bridges. (This will clear the bridge already found)
        /// </summary>
        public static void DetectBridge()
        {
            if(!_detectionBgw.IsBusy)
                _detectionBgw.RunWorkerAsync();
        }

        /// <summary>
        /// Property for the detection of local proxies. Leave false for faster operation if no local proxy.
        /// </summary>
        public static bool DetectLocalProxy
        {
            get => Comm.DetectProxy;
            set => Comm.DetectProxy = value;
        }

        /// <summary>
        /// Check if an ip scan is scanning.
        /// </summary>
        public static bool IsScanningForBridge => _ipscanBgw.IsBusy;

        public static bool IsDetectingBridge => _detectionBgw.IsBusy;

        /// <summary>
        /// Will scan an ip range for bridges.
        /// </summary>
        public static void ScanIpForBridge()
        {
            
            _ipscanBgw.RunWorkerAsync();
        }

        /// <summary>
        /// Will cancel an ongoing Ip scan for bridges
        /// </summary>
        public static void AbortScanForBridge()
        {
            if(_ipscanBgw.IsBusy)
            { 
                _ipscanBgw.CancelAsync();
            }
        }

        private static void _ipscanBgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnIpScanComplete?.Invoke(null, e);
        }

        private static void _ipscanBgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OnIpScanProgressReport?.Invoke(null, new IpScanProgressEventArgs((byte)e.UserState));
        }

        private static void _ipscanBgw_DoWork(object sender, DoWorkEventArgs e)
        {

            log.Info("Starting IP scan for bridge...");
            IPAddress ip = IPAddress.Parse(GetLocalIPAddress());
            byte[] ipArray = ip.GetAddressBytes();
            byte currentip = ipArray[3];
            Dictionary<string,Bridge> newlist = new Dictionary<string, Bridge>();

            BridgeSettings desc = new BridgeSettings();
            
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(50);
            

            for (byte x = 2; x <= 254; x++)
            {
                if (_ipscanBgw.CancellationPending)
                {
                    log.Info("IP scan cancelled.");
                    e.Cancel = true;
                    break;
                }
                _ipscanBgw.ReportProgress(0,x);
                if (x == currentip) continue;
                ipArray[3] = x;

                
                if (_ipscanBgw.CancellationPending) break;
                try
                {
                    HttpResponseMessage httpr = client.GetAsync(new Uri($@"http://{new IPAddress(ipArray)}/api/config")).Result;
                    if (httpr.IsSuccessStatusCode)
                    {
                        desc = Serializer.DeserializeToObject<BridgeSettings>(httpr.Content.ReadAsStringAsync().Result); // try to deserialize the received message.
                        if (desc == null) continue; // if the deserialisation didn't work it means this is not a bridge continue with next ip.
                        Bridge bridge = new Bridge()
                        {
                            IpAddress = new IPAddress(ipArray),
                            ApiVersion = desc.apiversion,
                            Mac = desc.mac
                        };

                        if (newlist.Count > 0)
                        {
                            if (!newlist.Any(y => Equals(y.Value.IpAddress, ipArray)))
                            {

                                log.Info($"Bridge found : {bridge.Name} at {bridge.IpAddress} ");
                                newlist.Add(desc.mac, bridge);
                            }
                            else
                            {
                                log.Info($"Bridge {bridge.Name} at {bridge.IpAddress} already in the list ignoring...");
                            }
                        }
                        else
                        {
                            log.Info($"Bridge found : {bridge.Name} at {bridge.IpAddress} ");
                            newlist.Add(desc.mac, bridge);
                        }
                    }
                }
                catch(System.TimeoutException)
                {
                    // Address is not responding ignore.
                }
                catch(Exception)
                {
                    
                }
          
                
            }

            client.Dispose();
            e.Result = newlist;
            log.Info("Ending IP scan for bridge.");
        }

        /// <summary>
        /// Get the current IP of the computer to scan current range for bridge.
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        /// <summary>
        /// Detect if there is a bridge at provided ip Address.
        /// </summary>
        /// <param name="BridgeIP"></param>
        /// <returns></returns>
        public static bool IsBridge(IPAddress BridgeIP)
        {
            bool bridge = false;
            Description desc = GetBridgeDescription(BridgeIP);
            if (desc != null)
            {
                bridge = true;
            }
            return bridge;
        }

        /// <summary>
        /// Get the description xml a bridge at an ip address.
        /// </summary>
        /// <param name="BridgeIP">IP address of the brdge</param>
        /// <returns>Description of the Bridge in the form of a Root Object.</returns>
        private static Description GetBridgeDescription(IPAddress BridgeIP)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Description));
            Description ro = null;
            try
            {
                XmlReader xr = XmlReader.Create("http://" + BridgeIP + "/description.xml");
                ro = (Description)ser.Deserialize(xr);
            }
            catch(Exception)
            {

            }
            return ro;
        }

        #region EVENTS

        public static event PortalDetectionTimedOutEvent OnPortalDetectionTimedOut;
        public delegate void PortalDetectionTimedOutEvent(object sender, DetectionErrorEventArgs e);

       // public static event PortalDetectionErrorEvent OnPortalDetectionError;
      //  public delegate void PortalDetectionErrorEvent(object sender, DetectionErrorEventArgs e);

        public static event IpScanDetectionCompleted OnIpScanComplete;
        public delegate void IpScanDetectionCompleted(object sender, RunWorkerCompletedEventArgs e);

        public static event IpScanProgressReport OnIpScanProgressReport;
        public delegate void IpScanProgressReport(object sender, IpScanProgressEventArgs e);

        public static event BrideDetectionComplete OnDetectionComplete;
        public delegate void BrideDetectionComplete(object sender, RunWorkerCompletedEventArgs e);

        public static event BridgeDetectionFailed OnBridgeDetectionFailed;
        public delegate void BridgeDetectionFailed(object sender, DetectionErrorEventArgs e);

        #endregion
    }

    public class DetectionErrorEventArgs : EventArgs
    {
        public DetectionErrorEventArgs(object e)
        {
            Error = e;
        }

        public object Error { get; }
    }

    public class IpScanProgressEventArgs : EventArgs
    {
        public IpScanProgressEventArgs(byte progress)
        {
            Progress = progress;
        }

        public byte Progress { get; }
    
    }
}
