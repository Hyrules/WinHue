using System.Runtime.Serialization;
using HueLib2.BridgeMessages;

namespace HueLib2
{
    [DataContract]
    public class CreationSuccess : IMessage
    {
        [DataMember]
        public string id { get; internal set; }

        public override string ToString()
        {
            return id;
        }
    }
}
