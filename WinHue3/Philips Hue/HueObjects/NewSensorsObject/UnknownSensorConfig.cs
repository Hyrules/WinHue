using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    public class UnknownSensorConfig : ValidatableBindableBase, ISensorConfigBase
    {
        public dynamic value { get; set; }
    }
}
