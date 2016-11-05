using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    [DataContract]
    public class HueDimmer : Sensor
    {
        [DataMember, HueLib(false, false)]
        public string productid { get; internal set; }

        [DataMember, HueLib(false, false)]
        public string swconfigid { get; internal set; }
    }
}
