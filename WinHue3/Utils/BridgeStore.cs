using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using ICSharpCode.SharpDevelop;

namespace WinHue3
{
    public static class BridgeStore
    {
        private static ObservableCollection<Bridge> _bridgelist;
        private static Bridge _selectedBridge;

        public enum AddManualBridgeResult
        {
            Success,
            Alreadyexists,
            NotResponding,
            UnknownError
        }

        static BridgeStore()
        {
            _bridgelist = new ObservableCollection<Bridge>();
            _selectedBridge = null;
        }

        public static ObservableCollection<Bridge> ListBridges
        {
            get { return _bridgelist; }
            set { _bridgelist = value; }
        }

        public static Bridge SelectedBridge
        {
            get { return _selectedBridge; }
            set { _selectedBridge = value; }
        }

        /// <summary>
        /// Add a bridge manually.
        /// </summary>
        /// <param name="bridgeaddress">IP address of the bridge.</param>
        /// <returns>AddManualBridgeResult</returns>
        public static AddManualBridgeResult AddManualBridge(IPAddress bridgeaddress)
        {
             CommandResult bresult = Hue.GetBridgeBasicConfig(bridgeaddress);
             if (!bresult.Success)
             {
                 WebExceptionStatus webex = (WebExceptionStatus)bresult.resultobject;
                 return webex == WebExceptionStatus.Timeout ? AddManualBridgeResult.NotResponding : AddManualBridgeResult.UnknownError;
             }
             BasicConfig bconfig = (BasicConfig) bresult.resultobject;
             Bridge newbridge = new Bridge()
             {
                 IpAddress = bridgeaddress,
                 Mac = bconfig.mac,
                 SwVersion = bconfig.swversion,
                 ApiVersion = bconfig.apiversion,
             };


             if (_bridgelist.Any(x => x.Mac == newbridge.Mac))
             {
                 return AddManualBridgeResult.Alreadyexists;
             }

             _bridgelist.Add(newbridge);

             return AddManualBridgeResult.Success;

         }
    }
}
