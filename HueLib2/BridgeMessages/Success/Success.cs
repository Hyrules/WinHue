using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2.BridgeMessages
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
