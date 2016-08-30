using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Class for the detected / discovered bridge(s).
    /// </summary>
    [DataContract]
    public class Device
    {
        /// <summary>
        /// ID of the detected bridge.
        /// </summary>
        [DataMember]
        public string id;
        /// <summary>
        /// String of the internal IP Address.      
        /// </summary>
        [DataMember]
        public string internalipaddress;
        /// <summary>
        /// Mac address of the bridge.
        /// </summary>
        [DataMember]
        public string macaddress;
    }
}
