using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;
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
 /*   public class SensorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Sensor oldsensor = (Sensor) value;
            writer.WriteStartObject();
            List<PropertyInfo> prop = oldsensor.GetType().GetListHueProperties();
            foreach (PropertyInfo p in prop)
            {
                if (p.GetValue(oldsensor) == null) continue;
                writer.WritePropertyName(p.Name);
                if (p.Name == "config")
                {
                    writer.WriteStartObject();
                    List<PropertyInfo> propsc = oldsensor.config.GetType().GetListHueProperties();
                    foreach (PropertyInfo pc in propsc)
                    {
                        if (pc.GetValue(oldsensor.config) == null) continue;
                        writer.WritePropertyName(pc.Name);
                        writer.WriteValue(pc.GetValue(oldsensor.config));
                    }

                    writer.WriteEndObject();
                }
                else if (p.Name == "state")
                {
                    writer.WriteStartObject();
                    List<PropertyInfo> propss = oldsensor.state.GetType().GetListHueProperties();
                    foreach (PropertyInfo ps in propss)
                    {
                        if (ps.GetValue(oldsensor.state) == null) continue;
                        writer.WritePropertyName(ps.Name);
                        writer.WriteValue(ps.GetValue(oldsensor.state));
                    }

                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteValue(p.GetValue(oldsensor));
                }
                
            }
            writer.WriteEnd();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);

            Sensor newsensor = new Sensor
            {
                name = obj["name"]?.Value<string>(),
                manufacturername = obj["manufacturername"]?.Value<string>(),
                modelid = obj["modelid"]?.Value<string>(),
                productid = obj["productid"]?.Value<string>(),
                swconfigid = obj["swconfigid"]?.Value<string>(),
                swversion = obj["swversion"]?.Value<string>(),
                uniqueid = obj["uniqueid"]?.Value<string>()
            };


            string type = obj["type"]?.Value<string>();
            newsensor.type = type;

            if (type != null)
            {
                switch (type)
                {
                    case "ZGPSwitch":
                        newsensor.state = obj["state"]?.ToObject<ButtonSensorState>();
                        newsensor.config = obj["config"]?.ToObject<HueTapSensorConfig>();
                        break;
                    case "Daylight":
                        //newsensor.state = obj["state"]?.ToObject<DaylightSensorState>();
                       // newsensor.config = obj["config"]?.ToObject<DaylightSensorConfig>();
                        break;
                    case "CLIPPresence":
                        newsensor.state = obj["state"]?.ToObject<PresenceSensorState>();
                        newsensor.config = obj["config"]?.ToObject<ClipPresenceSensorConfig>();
                        break;
                    case "CLIPGenericFlag":
                        newsensor.state = obj["state"]?.ToObject<ClipGenericFlagSensorState>();
                        newsensor.config = obj["config"]?.ToObject<ClipGenericFlagSensorConfig>();
                        break;
                    case "CLIPGenericStatus":
                        newsensor.state = obj["state"]?.ToObject<ClipGenericStatusSensorState>();
                        newsensor.config = obj["config"]?.ToObject<ClipGenericStatusSensorConfig>();
                        break;
                    case "CLIPHumidity":
                        newsensor.state = obj["state"]?.ToObject<ClipHumiditySensorState>();
                        newsensor.config = obj["config"]?.ToObject<ClipHumiditySensorConfig>();
                        break;
                    case "CLIPOpenClose":
                        newsensor.state = obj["state"]?.ToObject<ClipOpenCloseSensorState>();
                        newsensor.config = obj["config"]?.ToObject<ClipOpenCloseSensorConfig>();
                        break;
                    case "ZLLTemperature":
                    case "CLIPTemperature":
                        newsensor.state = obj["state"]?.ToObject<TemperatureSensorState>();
                        newsensor.config = obj["config"]?.ToObject<TemperatureSensorConfig>();
                        break;
                    case "ZLLSwitch":
                        newsensor.state = obj["state"]?.ToObject<ButtonSensorState>();
                        newsensor.config = obj["config"]?.ToObject<HueDimmerSensorConfig>();
                        break;
                    case "ZLLPresence":
                        newsensor.state = obj["state"]?.ToObject<PresenceSensorState>();
                        newsensor.config = obj["config"]?.ToObject<HueMotionSensorConfig>();
                        break;
                    case "CLIPLightlevel":
                    case "ZLLLightLevel":
                        newsensor.state = obj["state"]?.ToObject<LightLevelState>();
                        newsensor.config = obj["config"]?.ToObject<LightLevelConfig>();
                        break;
                    default:
                        newsensor.state = null;
                        newsensor.config = null;
                        break;

                }
            }
           
            return newsensor;
            
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Sensor) == objectType;
        }
    }*/
}

