using System.Runtime.Serialization;

namespace WinHue3.Philips_Hue_2.Bridge_Object
{
    /// <summary>
    /// Class for the detected / discovered bridge(s).
    /// </summary>
    [DataContract]
    public class HueDevice
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
