using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2.Objects.Sensor.ClipGenericFlag;
using HueLib2.Objects.Sensor.ClipGenericStatus;
using HueLib2.Objects.Sensor.ClipHumidty;
using HueLib2.Objects.Sensor.ClipOpenClose;
using HueLib2.Objects.Sensor.ClipPresence;
using HueLib2.Objects.Sensor.ClipZLLLightLevel;
using HueLib2.Objects.Sensor.ClipZLLTemperature;
using HueLib2.Objects.Sensor.Daylight;
using HueLib2.Objects.Sensor.HueMotionSensor;
using HueLib2.Objects.Sensor.HueTap;

namespace HueLib2.Objects.Interfaces
{
    public abstract class SensorFactory
    {
        public abstract ISensor CreateCreator(string sensortype);
    }

    public class SensorCreator : SensorFactory
    {
        public override ISensor CreateCreator(string sensortype) 
        {
            switch (sensortype)
            {
                case "ZGPSwitch":
                    return new HueTapSensor();
                case "Daylight":
                    return new DayLightSensor();
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
                    return new HueTapSensor();
                case "ZLLPresence":
                    return new HueMotionSensor();     
                case "CLIPLightlevel":
                case "ZLLLightLevel":
                    return new ClipZLLLightLevelSensor();
                default:
                    throw new NotSupportedException($"{sensortype} not supported.");
                    

            }

        }
        
    }
}
