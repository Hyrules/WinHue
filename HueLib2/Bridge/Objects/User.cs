using System;
using System.Runtime.Serialization;

namespace HueLib2
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
        [DataMember(EmitDefaultValue = false),]
        public string username { set; get; }
    }
}
