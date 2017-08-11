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
    public class ScheduleBody : IRuleBody
    {
        [DataMember]
        public string localtime { get; set; }

        [DataMember]
        public string status { get; set; }

        [DataMember]
        public bool? autodelete { get; set; }

        [DataMember]
        public bool? recycle { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }
}
