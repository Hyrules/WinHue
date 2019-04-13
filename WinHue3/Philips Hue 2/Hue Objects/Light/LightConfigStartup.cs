using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    [JsonObject]
    public class LightConfigStartup : ValidatableBindableBase
    {
        private string _mode;
        private bool _configured;
        private PowerCustomSettings _customsettings;

        [JsonProperty("mode"),Category("Light Config Startup"), Description("Mode")]
        public string Mode
        {
            get => _mode; 
            set => SetProperty(ref _mode,value);
        }

        [JsonProperty("configured"),Category("Light Config Startup"), Description("Configured")]
        public bool Configured
        {
            get => _configured;
            set => SetProperty(ref _configured, value);
        }
        
        [JsonProperty("customsettings"),Category("Light Config Startup"), Description("Custom Settings"), ExpandableObject]
        public PowerCustomSettings CustomSettings
        {
            get => _customsettings;
            set => SetProperty(ref _customsettings, value);
        }

        public override string ToString()
        {
            return Serializer.SerializeJsonObject(this);
        }
    }
}
