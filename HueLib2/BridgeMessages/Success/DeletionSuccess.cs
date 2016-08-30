using System.Runtime.Serialization;

namespace HueLib2
{
    [DataContract]
    public class DeletionSuccess : Message
    {
        [DataMember]
        public string success { get; set; }

        public override string ToString()
        {
            return success;
        }
    }


}
