using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HueLib
{
    /// <summary>
    /// Class for the BridgeSettings
    /// </summary>
    public class BridgeSettings
    {
        /// <summary>
        /// Name of the bridge. This is also its uPnP name, so will reflect the actual uPnP name after any conflicts have been resolved.
        /// </summary>
        [DataMember]
        public string name { get; set; }
        /// <summary>
        /// Zigbee Channel of the bridge and lights.
        /// </summary>
        [DataMember]
        public int? zigbeechannel { get; set; }
        /// <summary>
        /// Mac address of the bridge.
        /// </summary>
        [DataMember]
        public string mac { get; set; }
        /// <summary>
        /// DHCP ( is the bridge in DHCP mode ).
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool? dhcp { get; set; }
        /// <summary>
        /// IP Address of the bridge.
        /// </summary>
        [DataMember]
        public string ipaddress { get; set; }
        /// <summary>
        /// Netmask of the bridge.
        /// </summary>
        [DataMember]
        public string netmask { get; set; }
        /// <summary>
        /// Gateway of the bridge.
        /// </summary>
        [DataMember]
        public string gateway { get; set; }
        /// <summary>
        /// IP Address of the proxy server being used. A value of “none” indicates no proxy.
        /// </summary>
        [DataMember]
        public string proxyaddress { get; set; }
        /// <summary>
        /// Port of the proxy.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public int? proxyport { get; set; }
        /// <summary>
        /// Current time stored on the bridge. in Universal Time Coordinates.
        /// </summary>
        [DataMember]
        public string UTC { get; set; }
        /// <summary>
        /// Current time stored on the bridge. in local Time.
        /// </summary>
        [DataMember]
        public string localtime { get; set; }
        //       public Whitelist whitelist { get; set; } // An array of whitelisted user IDs.
        /// <summary>
        /// Software version of the Bridge ( Firmware version )
        /// </summary>
        [DataMember]
        public string swversion { get; set; }
        /// <summary>
        /// Api version of the Bridge ( interface version )
        /// </summary>
        [DataMember]
        public string apiversion { get; set; }
        /// <summary>
        /// Contains information related to software updates.
        /// </summary>
        [DataMember]
        public SwUpdate swupdate { get; set; }
        /// <summary>
        /// Indicates whether the link button has been pressed within the last 30 seconds.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool? linkbutton { get; set; }
        /// <summary>
        /// This indicates whether the bridge is registered to synchronize data with a portal account.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool? portalservices { get; set; }
        /// <summary>
        /// This indicates whether the bridge connected to the portal.
        /// </summary>
        [DataMember]
        public string portalconnection { get; set; }
        /// <summary>
        /// State of the portal services.
        /// </summary>
        [DataMember]
        public PortalState portalstate { get; set; }
        /// <summary>
        /// List of users/app allowed on the bridge.
        /// </summary>
        [DataMember]
        public Dictionary<string, Whitelist> whitelist { get; set; }
        /// <summary>
        /// Timezone of the bridge.
        /// </summary>
        [DataMember]
        public string timezone { get; set; }

        /// <summary>
        /// ID of the bridge.
        /// </summary>
        [DataMember]
        public string bridgeid { get; set; }

        /// <summary>
        /// Model id of the bridge.
        /// </summary>
        [DataMember]
        public string modelid { get; set; }
    }
}
