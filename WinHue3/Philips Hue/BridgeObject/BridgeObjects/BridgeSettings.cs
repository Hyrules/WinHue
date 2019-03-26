using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    /// <summary>
    /// Class for the BridgeSettings
    /// </summary>
    [JsonObject]
    public class BridgeSettings
    {
        /// <summary>
        /// Name of the bridge. This is also its uPnP name, so will reflect the actual uPnP name after any conflicts have been resolved.
        /// </summary>
        
        public string name { get; set; }
        /// <summary>
        /// Zigbee Channel of the bridge and lights.
        /// </summary>
        
        public int? zigbeechannel { get; set; }
        /// <summary>
        /// Mac address of the bridge.
        /// </summary>
        [DontSerialize]
        public string mac { get; set; }
        /// <summary>
        /// DHCP ( is the bridge in DHCP mode ).
        /// </summary>
        
        public bool? dhcp { get; set; }
        /// <summary>
        /// IP Address of the bridge.
        /// </summary>
        
        public string ipaddress { get; set; }
        /// <summary>
        /// Netmask of the bridge.
        /// </summary>
        
        public string netmask { get; set; }
        /// <summary>
        /// Gateway of the bridge.
        /// </summary>
        
        public string gateway { get; set; }
        /// <summary>
        /// IP Address of the proxy server being used. A value of “none” indicates no proxy.
        /// </summary>
        
        public string proxyaddress { get; set; }
        /// <summary>
        /// Port of the proxy.
        /// </summary>
        
        public int? proxyport { get; set; }
        /// <summary>
        /// Current time stored on the bridge. in Universal Time Coordinates.
        /// </summary>
        
        public string UTC { get; set; }
        /// <summary>
        /// Current time stored on the bridge. in local Time.
        /// </summary>
        
        public string localtime { get; set; }
        /// <summary>
        /// Software version of the Bridge ( Firmware version )
        /// </summary>
        [DontSerialize]
        public string swversion { get; set; }
        /// <summary>
        /// Api version of the Bridge ( interface version )
        /// </summary>
        [DontSerialize]
        public string apiversion { get; set; }
        /// <summary>
        /// Contains information related to software updates.
        /// </summary>
        public SwUpdate swupdate { get; set; }
        /// <summary>
        /// Contains information related to software updates. V2
        /// </summary>
        [DontSerialize]
        public SwUpdate2 swupdate2 { get; set; }
        /// <summary>
        /// Indicates whether the link button has been pressed within the last 30 seconds.
        /// </summary>
        
        public bool? linkbutton { get; set; }
        /// <summary>
        /// This indicates whether the bridge is registered to synchronize data with a portal account.
        /// </summary>
        
        [DontSerialize]
        public bool? portalservices { get; set; }
        /// <summary>
        /// This indicates whether the bridge connected to the portal.
        /// </summary>
        [DontSerialize]
        public string portalconnection { get; set; }
        /// <summary>
        /// State of the portal services.
        /// </summary>
        [DontSerialize]
        public PortalState portalstate { get; set; }
        /// <summary>
        /// List of users/app allowed on the bridge.
        /// </summary>
        
        public Dictionary<string, Whitelist> whitelist { get; set; }
        /// <summary>
        /// Timezone of the bridge.
        /// </summary>
        
        public string timezone { get; set; }

        /// <summary>
        /// ID of the bridge.
        /// </summary>
        
        public string bridgeid { get; set; }

        /// <summary>
        /// Factory new.
        /// </summary>
        [DontSerialize]
        public bool factorynew { get; set; }

        /// <summary>
        /// Backup.
        /// </summary>
        
        public Backup backup { get; set; }

        /// <summary>
        /// Model id of the bridge.
        /// </summary>
        
        public string modelid { get; set; }
    }
}
