using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    [DataContract]
    public class Backup
    {
        [DataMember]
        public string idle { get; set; }

        [DataMember]
        public int errorcode { get; set; }
    }
}
