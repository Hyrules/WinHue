using System.Runtime.Serialization;

namespace HueLib2
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
        [DataMember]
        public bool signedon { get; set; }
        /// <summary>
        /// If the bridge is able to receive from the portal.
        /// </summary>
        [DataMember]
        public bool incoming { get; set; }
        /// <summary>
        /// If the bridge is able to send to the portal.
        /// </summary>
        [DataMember]
        public bool outgoing { get; set; }
        /// <summary>
        /// State of communications.
        /// </summary>
        [DataMember]
        public string communication { get; set; }
    }
}
