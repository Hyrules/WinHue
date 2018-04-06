using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.Communication;

namespace WinHue3.Functions.BridgeFinder
{
    public static class BridgeFinder
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static BackgroundWorker _bgw;

        static BridgeFinder()
        {
            _bgw = new BackgroundWorker();
            _bgw.DoWork += _bgw_DoWork;
            _bgw.RunWorkerCompleted += _bgw_RunWorkerCompleted;
        }

        private static void _bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                IPAddress ip = (IPAddress)e.Result;
            }

        }

        public static void FindBridge(Bridge br)
        {
            _bgw.RunWorkerAsync(br);
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

                _bgw.ReportProgress(0, x);
                if (x == currentip) continue;
                ipArray[3] = x;

                Comm.Timeout = 50;
                if (_bgw.CancellationPending) break;

                CommResult comres = Comm.SendRequest(new Uri($@"http://{new IPAddress(ipArray)}/api/config"), WebRequestType.Get);
                Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings desc = new Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings();

                switch (comres.Status)
                {
                    case WebExceptionStatus.Success:
                        desc = Serializer.DeserializeToObject<Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings>(comres.Data); // try to deserialize the received message.
                        if (desc == null) continue; // if the deserialisation didn't work it means this is not a bridge continue with next ip.
                        if (desc.mac == mac)
                        {
                            e.Result = new IPAddress(ipArray);
                            break;
                        }

                        break;
                    case WebExceptionStatus.Timeout:
                        // IP DOES NOT RESPOND
                        break;
                    default:

                        break;
                }
            }
        }

    }
}
