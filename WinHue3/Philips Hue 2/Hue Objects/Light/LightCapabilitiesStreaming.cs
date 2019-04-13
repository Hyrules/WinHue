using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    [JsonObject]
    public class LightCapabilitiesStreaming : ValidatableBindableBase
    {
        private bool _renderer;
        private bool _proxy;

        [JsonProperty("renderer"), Category("Light Capabilities Streaming"), Description("Renderer")]
        public bool Renderer
        {
            get => _renderer;
            set => SetProperty(ref _renderer,value);
        }

        [JsonProperty("proxy"), Category("Light Capabilities Streaming"), Description("Proxy")]
        public bool Proxy
        {
            get => _proxy;
            set => SetProperty(ref _proxy, value);
        }

        public override string ToString()
        {            
            return $"Renderer :{Renderer}, Proxy :{Proxy}";
        }
    }
}
