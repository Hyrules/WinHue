using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.GroupObject
{
    public class GroupState
    {

        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Description("Identify if all lights are on."), Category("Action Properties"), ReadOnly(true)]
        public bool? all_on { get; set; }

        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Description("Identify if any lights are on."), Category("Action Properties"), ReadOnly(true)]
        public bool? any_on { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
