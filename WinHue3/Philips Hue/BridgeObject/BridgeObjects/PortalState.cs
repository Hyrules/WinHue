using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    /// <summary>
    /// State of the portal sevices.
    /// </summary>
    [DataContract]
    public class PortalState
    {
        /// <summary>
        /// If the bridge is signed on the portal.
        /// </summary>
        [ DataMember]
        public bool signedon { get; set; }
        /// <summary>
        /// If the bridge is able to receive from the portal.
        /// </summary>
        [ DataMember]
        public bool incoming { get; set; }
        /// <summary>
        /// If the bridge is able to send to the portal.
        /// </summary>
        [ DataMember]
        public bool outgoing { get; set; }
        /// <summary>
        /// State of communications.
        /// </summary>
        [ DataMember]
        public string communication { get; set; }
    }
}
