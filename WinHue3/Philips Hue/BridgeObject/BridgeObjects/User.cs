using System.Runtime.Serialization;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    /// <summary>
    /// Class for a Hue User.
    /// </summary>
    [DataContract]
    public class User
    {
        /// <summary>
        /// Device type of the user.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string devicetype { set; get; }
        /// <summary>
        /// Username of the user.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string username { set; get; }
        /// <summary>
        /// Ask the bridge to create a client key for the steaming api
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool? generateclientkey { get; set; }
    }
}
