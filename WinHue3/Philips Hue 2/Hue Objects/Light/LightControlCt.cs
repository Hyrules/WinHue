using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    [JsonObject]
    public class LightControlCt : ValidatableBindableBase
    {
        private ushort _min;
        private ushort _max;

        [JsonProperty("min"), Category("Light Control CT"), Description("Min")]
        public ushort Min
        {
            get => _min;
            set => SetProperty(ref _min,value);
        }

        [JsonProperty("max"), Category("Light Control CT"), Description("Max")]
        public ushort Max
        {
            get => _max;
            set => SetProperty(ref _max,value);
        }

        public override string ToString()
        {
            return $"Min :{Min}, Max: {Max}";
        }
    }
}
