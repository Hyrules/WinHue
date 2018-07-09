using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;

namespace WinHue3.Functions.BridgePairing
{
    public class BridgeHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BridgeHandler()
        {

        }

        public List<Bridge> LoadBridges(Dictionary<string, BridgeSaveSettings> savedbridges)
        {
            log.Info($"Loading saved bridges... {savedbridges.Count} bridge(s) found.");

            List<Bridge> bridges = new List<Bridge>();

            foreach (KeyValuePair<string, BridgeSaveSettings> b in savedbridges)
            {
                IPAddress.TryParse(b.Value.ip, out IPAddress ip);
                if (ip == null)
                {
                    log.Error($"Unable to parse IP ignoring bridge {b.Value.name}");
                    continue;
                }
                Bridge br = new Bridge() {IpAddress = ip};
                BasicConfig bc = br.GetBridgeBasicConfig();
                if (bc == null)
                {
                    log.Warn("Bridge not responding. Does the IP address has changed ? Will try to find it...");
                    Hue.DetectBridge();
                    if (bc == null)
                    {
                        log.Error($"Cannot find this bridge {b.Value.name} ignoring it.");
                    }
                }
                else
                {
                    
                }
            }

            return bridges;
        }

      /*  private IPAddress FindBridgeIP(string mac)
        {

        }*/
    }
}
