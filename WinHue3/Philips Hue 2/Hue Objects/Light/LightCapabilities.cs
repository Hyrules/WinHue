using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    [JsonObject]
    public class LightCapabilities : ValidatableBindableBase
    {
        private bool _certified;
        private LightCapabilitiesControl _control;
        private LightCapabilitiesStreaming _streaming;

        [JsonProperty("certified"), Category("Light Capabilities"), Description("Certified")]
        public bool Certified
        {
            get => _certified;
            set => SetProperty(ref _certified,value);
        }

        [JsonProperty("control"), Category("Light Capabilities"), Description("Control"), ExpandableObject]
        public LightCapabilitiesControl Control
        {
            get => _control;
            set => SetProperty(ref _control,value);
        }

        [JsonProperty("streaming"), Category("Light Capabilities"), Description("Streaming"), ExpandableObject]
        public LightCapabilitiesStreaming Streaming
        {
            get => _streaming;
            set => SetProperty(ref _streaming,value);
        }

        public override string ToString()
        {
            return $"Certified: {Certified}, Control: {Control}, Streaming: {Streaming}";
        }
    }
}
