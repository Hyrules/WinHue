using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipGenericFlag;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipHumidity;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipOpenClose;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipZllLightLevel;
using WinHue3.Philips_Hue.HueObjects.SensorObject.ClipZllTemperature;
using WinHue3.Philips_Hue.HueObjects.SensorObject.Daylight;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueMotion;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueTap;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject
{
    public static class SensorFactory
    {
        public static ISensor CreateSensor(string type)
        {
            switch (type)
            {
                case "ZGPSwitch":
                    return new HueTapSensor();
                case "Daylight":
                    return new DaylightSensor();
                case "CLIPPresence":
                    return new ClipPresenceSensor();
                case "CLIPGenericFlag":
                    return new ClipGenericFlagSensor();
                case "CLIPGenericStatus":
                    return new ClipGenericStatusSensor();
                case "CLIPHumidity":
                    return new ClipHumiditySensor();
                case "CLIPOpenClose":
                    return new ClipOpenCloseSensor();
                case "ZLLTemperature":
                case "CLIPTemperature":
                    return new ClipZllTemperatureSensor();
                case "ZLLSwitch":
                    return new HueDimmerSensor();
                case "ZLLPresence":
                     return new HueMotionSensor();
                case "CLIPLightlevel":
                case "ZLLLightLevel":
                    return new ClipZllLightLevelSensor();
                default:
                    throw new NotSupportedException($"{type} not supported.");
            }

        }
    }
}
