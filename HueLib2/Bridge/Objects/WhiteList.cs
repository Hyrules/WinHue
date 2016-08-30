using System;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Class Whitelist
    /// </summary>
    [DataContract]
    public class Whitelist
    {
        /// <summary>
        /// id of the user.
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// LastUseDate
        /// </summary>
        [DataMember(Name = "last use date")]
        public string LastUseDate { get; set; }
        /// <summary>
        /// Created Date
        /// </summary>
        [DataMember(Name = "create date")]
        public string CreateDate { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

    }
}
