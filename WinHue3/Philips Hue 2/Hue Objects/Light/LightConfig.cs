using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    [JsonObject]
    public class LightConfig : ValidatableBindableBase
    {
        private string _archeType;
        private string _function;
        private string _direction;
        private LightConfigStartup _startup;

        [JsonProperty("archetype"), Category("Light Config"), Description("Archetype")]
        public string ArcheType
        {
            get => _archeType;
            set => SetProperty(ref _archeType,value);
        }

        [JsonProperty("function"), Category("Light Config"), Description("Function")]
        public string Function
        {
            get => _function;
            set => SetProperty(ref _function,value);
        }

        [JsonProperty("direction"), Category("Light Config"), Description("Direction")]
        public string Direction
        {
            get => _direction;
            set => SetProperty(ref _direction,value);
        }

        [JsonProperty("startup"), Category("Light Config"), Description("Startup"), ExpandableObject]
        public LightConfigStartup Startup
        {
            get => _startup;
            set => SetProperty(ref _startup,value);
        }

        public override string ToString()
        {
            return $"ArcheType: {ArcheType}, Function: {Function}, Direction: {Direction}, Startup: {Startup}";
        }
    }
}
