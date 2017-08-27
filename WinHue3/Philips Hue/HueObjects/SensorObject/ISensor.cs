using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
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
using WinHue3.Philips_Hue.HueObjects.SensorObject.UnknownSensorObject;


namespace WinHue3.Philips_Hue.HueObjects.SensorObject
{
    [HueType("sensors"),JsonConverter(typeof(ISensorJsonConverter))]
    public interface ISensor : IHueObject
    {
        string modelid { get; set; }
        string swversion { get; set; }
        string type { get; set; }
        string manufacturername { get; set; }
        string uniqueid { get; set; }
        string productid { get; set; }
        string swconfigid { get; set; }
        ISensorConfig GetConfig();
        ISensorState GetState();
        bool SetConfig(ISensorConfig sconfig);
        bool SetState(ISensorState sstate);
    }

    public class ISensorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IContractResolver cr = serializer.ContractResolver;
            serializer.ContractResolver = new DefContractResolver();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer,value);
            serializer.ContractResolver = cr;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IContractResolver cr = serializer.ContractResolver;
            serializer.ContractResolver = new DefContractResolver();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            JObject obj = serializer.Deserialize<JObject>(reader);
            ISensor sensor;

            string type = obj["type"].Value<string>();

            switch (type)
            {
                case "ZGPSwitch":
                    sensor = serializer.Deserialize<HueTapSensor>(new JTokenReader(obj));
                    break;
                case "Daylight":
                    sensor = serializer.Deserialize<DaylightSensor>(new JTokenReader(obj));
                    break;
                case "CLIPPresence":
                    sensor = serializer.Deserialize<ClipPresenceSensor>(new JTokenReader(obj));
                    break;
                case "CLIPGenericFlag":
                    sensor = serializer.Deserialize<ClipGenericFlagSensor>(new JTokenReader(obj));
                    break;
                case "CLIPGenericStatus":
                    sensor =  serializer.Deserialize<ClipGenericStatusSensor>(new JTokenReader(obj));
                    break;
                case "CLIPHumidity":
                    sensor =  serializer.Deserialize<ClipHumiditySensor>(new JTokenReader(obj));
                    break;
                case "CLIPOpenClose":
                    sensor =  serializer.Deserialize<ClipOpenCloseSensor>(new JTokenReader(obj));
                    break;
                case "ZLLTemperature":
                case "CLIPTemperature":
                    sensor =  serializer.Deserialize<ClipZllTemperatureSensor>(new JTokenReader(obj));
                    break;
                case "ZLLSwitch":
                    sensor =  serializer.Deserialize<HueDimmerSensor>(new JTokenReader(obj));
                    break;
                case "ZLLPresence":
                    sensor =  serializer.Deserialize<HueMotionSensor>(new JTokenReader(obj));
                    break;
                case "CLIPLightlevel":
                case "ZLLLightLevel":
                    sensor =  serializer.Deserialize<ClipZllLightLevelSensor>(new JTokenReader(obj));
                    break;
                default:
                    sensor =  serializer.Deserialize<UnknownSensor>(new JTokenReader(obj));
                    break;
            }
            serializer.ContractResolver = cr;
            return sensor;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ISensor);
        }
    }

    public class DefContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            return null;
        }
    }
}
