using System;
using System.Runtime.Serialization;
using HueLib2.BridgeMessages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2
{
    /// <summary>
    /// Error class
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Type of error.
        /// </summary>
        [DataMember]
        public int type { get; set; }
        /// <summary>s
        /// Address of the error.
        /// </summary>
        [DataMember(EmitDefaultValue=false,IsRequired=false)]
        public string address { get; set; }
        /// <summary>
        /// Description of the error.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        public override string ToString()
        {
            return $"Type : {type}, {description} at address {address}.";
        }

  
    }

   
}
