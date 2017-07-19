using System.Runtime.Serialization;
using HueLib2.BridgeMessages;

namespace HueLib2
{
    [DataContract]
    public class DeletionSuccess
    {
        [DataMember]
        public string success { get; set; }

        public override string ToString()
        {
            return success;
        }
    }


}
