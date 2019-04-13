using System.Runtime.Serialization;

namespace WinHue3.Philips_Hue_2.BridgeMessages
{
    /// <summary>
    /// Error class
    /// </summary>
    public class Error : IMessage
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
