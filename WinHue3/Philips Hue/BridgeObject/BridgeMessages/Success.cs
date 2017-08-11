using System.Runtime.Serialization;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeMessages
{
    [DataContract]
    public class Success : IMessage
    {
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string value { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(value) ? $"Success : {Address}" : $"Success : {Address} => {value}";
        }
    }
}
