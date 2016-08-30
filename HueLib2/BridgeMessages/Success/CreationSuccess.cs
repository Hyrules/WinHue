using System.Runtime.Serialization;

namespace HueLib2
{
    [DataContract]
    public class CreationSuccess : Message 
    {
        [DataMember]
        public string id { get; internal set; }

        public override string ToString()
        {
            return id;
        }
    }
}
