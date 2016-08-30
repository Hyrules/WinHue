using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HueLib2
{
    [DataContract]
    public class BasicConfig
    {
        [DataMember]
        public string swversion {get;set;}

        [DataMember]
        public string apiversion { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string mac { get; set; }

        [DataMember]
        public string bridgeid { get; set; }

        [DataMember]
        public string replacesbridgeid { get; set; }

        [DataMember]
        public bool? factorynew { get; set; }

        [DataMember]
        public string modelid { get; set; }
    }
}
