using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipGenericFlag;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipHumidity;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipOpenClose;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipZllLightLevel;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipZllTemperature;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueMotion;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueTap;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject
{
    public static class HueSensorConfigFactory
    {
        public static ISensorConfig CreateSensorConfigFromSensorType(string type)
        {
            switch (type)
            {
                case "CLIPGenericFlag":
                    return new ClipGenericFlagSensorConfig();
                case "CLIPGenericStatus":
                    return new ClipGenericStatusSensorConfig();
                case "CLIPHumidity":
                    return new ClipHumiditySensorConfig();
                case "CLIPOpenClose":
                    return new ClipOpenCloseSensorConfig();
                case "CLIPPresence":
                    return new ClipPresenceSensorConfig();
                case "ZLLTemperature":
                case "CLIPTemperature":
                    return new TemperatureSensorConfig();
                case "CLIPLightLevel":
                case "ZLLLightlevel":
                    return new LightLevelConfig();
                case "CLIPSwitch":
                case "ZGPSwitch":
                    return new HueTapSensorConfig();
                case "ZLLSwitch":
                    return new HueDimmerSensorConfig();
                case "ZLLPresence":
                    return new HueMotionSensorConfig();
               
                default:
                    return null;
            }
        }
    }
}
