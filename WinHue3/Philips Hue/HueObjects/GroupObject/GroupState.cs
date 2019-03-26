using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.GroupObject
{
    [JsonObject]
    public class GroupState
    {

        [Description("Identify if all lights are on."), Category("Action Properties")]
        public bool? all_on { get; set; }

        [Description("Identify if any lights are on."), Category("Action Properties")]
        public bool? any_on { get; set; }

        public override string ToString()
        {
            return Serializer.SerializeJsonObject(this);
        }
    }
}
