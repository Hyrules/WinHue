using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    [JsonObject]
    public class LightCapabilitiesControl : ValidatableBindableBase
    {
        private ushort _minDimLevel;
        private ushort _maxLumen;
        private string _colorGamutType;
        private CoordinatesCollection _colorGamut;
        private LightControlCt _ct;

        [JsonProperty("mindimlevel"), Category("Light Capabilities Control"), Description("Minimum Dim Level")]
        public ushort MinDimLevel
        {
            get => _minDimLevel;
            set => SetProperty(ref _minDimLevel,value);
        }

        [JsonProperty("maxlumen"), Category("Light Capabilities Control"), Description("Maximum Lumens")]
        public ushort MaxLumen
        {
            get => _maxLumen;
            set => SetProperty(ref _maxLumen,value);
        }

        [JsonProperty("colorgamuttype"), Category("Light Capabilities Control"), Description("Color Gamut Type")]
        public string ColorGamutType
        {
            get => _colorGamutType;
            set => SetProperty(ref _colorGamutType,value);
        }

        [JsonProperty("colorgamut"), Category("Light Capabilities Control"), Description("Color Gamut")]
        public CoordinatesCollection ColorGamut
        {
            get => _colorGamut;
            set => SetProperty(ref _colorGamut,value);
        }

        [JsonProperty("ct"), Category("Light Capabilities Control"), Description("Color Temperature"), ExpandableObject]
        public LightControlCt Ct
        {
            get => _ct;
            set => SetProperty(ref _ct,value);
        }

        public override string ToString()
        {
            return $"MinDimLevel: {MinDimLevel}, MaxLumen: {MaxLumen}, ColorGamutType: {ColorGamutType}, ColorGamut: {ColorGamut}, Ct: {Ct}";
        }
    }
}
