using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    [JsonObject]
    public class LightSwUpdate : ValidatableBindableBase
    {
        private string _state;
        private string _lastinstall;

        [JsonProperty("state"),Category("Light Software Update"), Description("State")]
        public string State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        [JsonProperty("lastinstall"), Category("Light Software Update"), Description("Last install")]
        public string LastInstall
        {
            get => _lastinstall; 
            set => SetProperty(ref _lastinstall,value);
        }


        public override string ToString()
        {
            return $"State : {State}, Last Install: {LastInstall} ";
        }
    }
}
