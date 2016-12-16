using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HueLib2
{
    [DataContract]
    public class ScheduleBody : RuleBody
    {
        [DataMember, HueLib(true,true)]
        public string localtime { get; set; }

        [DataMember, HueLib(true,true)]
        public string status { get; set; }

        [DataMember, HueLib(true, true)]
        public bool? autodelete { get; set; }

        [DataMember, HueLib(true, true)]
        public bool? recycle { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }
}
