

using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipOpenClose;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllLightLevel;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllTemperature;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.GeoFence;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueMotion;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueTap;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    public static class HueSensorConfigFactory
    {
        public static ISensorConfigBase CreateSensorConfigFromSensorType(string type)
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
                case "ZLLLightLevel":
                    return new LightLevelConfig();
                case "CLIPSwitch":
                case "ZGPSwitch":
                    return new HueTapSensorConfig();
                case "ZLLSwitch":
                    return new HueDimmerSensorConfig();
                case "ZLLPresence":
                    return new HueMotionSensorConfig();
                case "Daylight":
                    return new DaylightSensorConfig();
                case "Geofence":
                    return new GeofenceConfig();
                default:
                    return new UnknownSensorConfig();
            }
        }

    }
}
