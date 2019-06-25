using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.Communication2;

namespace WinHue3.Functions.BridgeFinder
{
    public static class BridgeFinder
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static BackgroundWorker _bgw;

        public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
        public static event ProgressChangedEventHandler ProgressReported;

        public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e);
        public static event RunWorkerCompletedEventHandler ScanCompleted;


        static BridgeFinder()
        {
            _bgw = new BackgroundWorker();
            _bgw.DoWork += _bgw_DoWork;
            _bgw.RunWorkerCompleted += _bgw_RunWorkerCompleted;
            _bgw.ProgressChanged += _bgw_ProgressChanged; ;
            _bgw.WorkerReportsProgress = true;
            _bgw.WorkerSupportsCancellation = true;
        }

        public static void CancelSearch()
        {
            _bgw.CancelAsync();
        }

        private static void _bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressReported?.Invoke(null,e);
        }

        private static void _bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ScanCompleted?.Invoke(null,e);
        }

        public static void FindBridge(Bridge br)
        {
            _bgw.RunWorkerAsync(br.Mac);
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

        private static void _bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            string mac = e.Argument.ToString();
            IPAddress ip = IPAddress.Parse(GetLocalIPAddress());
            byte[] ipArray = ip.GetAddressBytes();
            byte currentip = ipArray[3];
            e.Result = null;

            for (byte x = 2; x <= 254; x++)
            {
                if (_bgw.CancellationPending)
                {
                    log.Info("Bridge lookup cancelled.");
                    e.Cancel = true;
                    break;
                }

                
                if (x == currentip) continue;
                ipArray[3] = x;
                _bgw.ReportProgress(0, new ProgressReport(new IPAddress(ipArray),x));

                HueHttpClient.Timeout = 50;
                if (_bgw.CancellationPending) break;

                HttpResult comres = HueHttpClient.SendRequest(new Uri($@"http://{new IPAddress(ipArray)}/api/config"), WebRequestType.Get);
                Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings desc = new Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings();

                if (comres.Success)
                {
                    desc = Serializer.DeserializeToObject<Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings>(comres.Data); // try to deserialize the received message.
                    if (desc == null) continue; // if the deserialisation didn't work it means this is not a bridge continue with next ip.
                    if (desc.mac == mac)
                    {
                        e.Result = new IPAddress(ipArray);
                        break;
                    }

                }

                if (e.Result != null)
                {
                    _bgw.ReportProgress(0, new ProgressReport(new IPAddress(ipArray),254));
                    break;
                }
                    
            }

            // Restore the timeout to the original value
            HueHttpClient.Timeout = WinHueSettings.settings.Timeout;
        }

    }
}
