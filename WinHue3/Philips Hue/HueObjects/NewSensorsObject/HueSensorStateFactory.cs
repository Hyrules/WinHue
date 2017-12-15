using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipOpenClose;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllLightLevel;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllTemperature;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueMotion;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    public static class HueSensorStateFactory
    {
        public static ISensorStateBase CreateSensorStateFromSensorType(string type)
        {

            switch (type)
            {
                case "CLIPGenericFlag":
                    return new ClipGenericFlagSensorState();
                case "CLIPGenericStatus":
                    return new ClipGenericStatusSensorState();
                case "CLIPHumidity":
                    return new ClipHumiditySensorState();
                case "CLIPOpenClose":
                    return new ClipOpenCloseSensorState();
                case "CLIPPresence":
                case "ZLLPresence":
                case "Geofence":
                    return new PresenceSensorState();
                case "ZLLTemperature":
                case "CLIPTemperature":
                    return new TemperatureSensorState();
                case "CLIPLightLevel":
                case "ZLLLightLevel":
                    return new LightLevelState();
                case "CLIPSwitch":
                case "ZGPSwitch":
                case "ZLLSwitch":
                    return new ButtonSensorState();
                case "Daylight":
                    return new DaylightSensorState();
                default:
                    return null;
            }
        }
    }
}
