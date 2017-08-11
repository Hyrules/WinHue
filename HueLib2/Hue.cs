using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Xml;
using System.Xml.Serialization;
using ManagedUPnP;
using HueLib2;
using System.ServiceProcess;

namespace HueLib2
{
    /// <summary>
    /// Hue Class.
    /// </summary>
    public static class Hue
    {

        private static BackgroundWorker _ipscanBgw = new BackgroundWorker();
        private static BackgroundWorker _detectionBgw = new BackgroundWorker();

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
            if (e.Result != null)
                OnDetectionComplete?.Invoke(null, e);
        }

        private static void _detectionBgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, BasicConfig> newdetectedBridge = new Dictionary<string, BasicConfig>();

            // Detect using UPNP
            bool finished;

            ServiceController sc = new ServiceController("SSDPSRV");

            if (sc.Status == ServiceControllerStatus.Running)
            {
                try
                {
                    List<ManagedUPnP.Device> upnpDevices =
                        Discovery.FindDevices(null, 3000, int.MaxValue, out finished).ToList();

                    foreach (ManagedUPnP.Device dev in upnpDevices)
                    {
                        if (!dev.ModelName.Contains("Philips hue bridge")) continue;

                        CommandResult<BasicConfig> bresult = GetBridgeBasicConfig(IPAddress.Parse(dev.RootHostName));
                        if (bresult.Success)
                        {
                            newdetectedBridge.Add(dev.RootHostName, bresult.Data);
                        }

                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            // If not bridge are found via upnp try the portal.
            if (newdetectedBridge.Count == 0)
            {

                // Detect using Portal
                CommResult comres = Communication.SendRequest(new Uri("http://www.meethue.com/api/nupnp"), WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        List<Device> portalDevices = Serializer.DeserializeToObject<List<Device>>(comres.data);
                        foreach (Device dev in portalDevices)
                        {
                            if (newdetectedBridge.ContainsKey(dev.internalipaddress)) continue;
                            CommandResult<BasicConfig> bresult = GetBridgeBasicConfig(IPAddress.Parse(dev.internalipaddress));
                            if (bresult.Success)
                            {
                                newdetectedBridge.Add(dev.internalipaddress, (BasicConfig) bresult.Data);
                            }
                            
                        }
                        break;
                    case WebExceptionStatus.Timeout:
                        OnPortalDetectionTimedOut?.Invoke(null, new DetectionErrorEventArgs(comres.data));
                        OnBridgeDetectionFailed?.Invoke(null, new DetectionErrorEventArgs(comres.data));
                        break;
                    default:
                        OnPortalDetectionError?.Invoke(null, new DetectionErrorEventArgs(comres.data));
                        OnBridgeDetectionFailed?.Invoke(null, new DetectionErrorEventArgs(comres.data));
                        break;
                }

            }


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
            get { return Communication.DetectProxy;  }
            set { Communication.DetectProxy = value; }
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
            IPAddress ip = IPAddress.Parse(GetLocalIPAddress());
            byte[] ipArray = ip.GetAddressBytes();
            byte currentip = ipArray[3];
            Dictionary<string,Bridge> newlist = new Dictionary<string, Bridge>();

            BridgeSettings desc = new BridgeSettings();
            
            for (byte x = 2; x <= 254; x++)
            {
                if (_ipscanBgw.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                _ipscanBgw.ReportProgress(0,x);
                if (x == currentip) continue;
                ipArray[3] = x;

                Communication.Timeout = 50;
                if (_ipscanBgw.CancellationPending) break;

                CommResult comres = Communication.SendRequest(new Uri($@"http://{new IPAddress(ipArray)}/api/config"), WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        desc = Serializer.DeserializeToObject<BridgeSettings>(comres.data); // try to deserialize the received message.
                        if(desc == null) continue; // if the deserialisation didn't work it means this is not a bridge continue with next ip.
                        if (newlist.Count > 0)
                        {
                            if (!newlist.Any(y => Equals(y.Value.IpAddress, ipArray)))
                            {
                                newlist.Add(desc.mac,new Bridge() { IpAddress = new IPAddress(ipArray), ApiVersion = desc.apiversion, Mac = desc.mac });
                            }
                        }
                        else
                        {
                            newlist.Add(desc.mac,new Bridge() { IpAddress = new IPAddress(ipArray), ApiVersion = desc.apiversion, Mac = desc.mac });
                        }
                        break;
                    case WebExceptionStatus.Timeout:
                        break;
                    default:
                        
                        break;
                }
                
            }

            e.Result = newlist;
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

        /// <summary>
        /// Try to contact the specified bridge to get the basic config.
        /// </summary>
        /// <param name="BridgeIP">IP Address of the bridge.</param>
        /// <returns>The basic configuration of the bridge.</returns>
        public static CommandResult<BasicConfig> GetBridgeBasicConfig(IPAddress BridgeIP)
        {
            CommandResult<BasicConfig> bresult = new CommandResult<BasicConfig>() {Success = false};

            CommResult comres = Communication.SendRequest(new Uri($@"http://{BridgeIP}/api/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    BasicConfig config = Serializer.DeserializeToObject<BasicConfig>(comres.data);
                    if (config != null)
                    {
                        bresult.Success = true;
                        bresult.Data = config;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    bresult.Exception = new System.TimeoutException("Connexion timout !");
                    break;
                case WebExceptionStatus.UnknownError:
                    bresult.Exception = new Exception("Unknown error !");
                    break;
                default:
                    bresult.Exception = new Exception("Other Exception !");
                    break;
            }

            return bresult;
        }

        #region EVENTS

        public static event PortalDetectionTimedOutEvent OnPortalDetectionTimedOut;
        public delegate void PortalDetectionTimedOutEvent(object sender, DetectionErrorEventArgs e);

        public static event PortalDetectionErrorEvent OnPortalDetectionError;
        public delegate void PortalDetectionErrorEvent(object sender, DetectionErrorEventArgs e);

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

    enum HueObjectType
    {
        lights,
        groups,
        sensors,
        scenes,
        schedules,
        resourcelinks,
        rules
    };

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
