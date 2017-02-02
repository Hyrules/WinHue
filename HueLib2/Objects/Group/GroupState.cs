using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HueLib2.Objects.Group
{
    public class GroupState
    {

        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("Identify if all lights are on."), Category("Action Properties"), ReadOnly(true)]
        public bool? all_on { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("Identify if any lights are on."), Category("Action Properties"), ReadOnly(true)]
        public bool? any_on { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
        }
    }
}
