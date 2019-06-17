using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    public class RuleActionJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RuleAction ra = (RuleAction) value;
            writer.WriteStartObject();
            writer.WritePropertyName("address");
            writer.WriteValue(ra.address);
            writer.WritePropertyName("body");
            writer.WriteRaw(ra.body);
            writer.WritePropertyName("method");
            writer.WriteValue(ra.method);
            writer.WriteEndObject();
            writer.WriteEnd();
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            RuleAction ra = new RuleAction
            {
                address = new HueAddress(obj["address"].Value<string>()),
                method = obj["method"].Value<string>(),
                body = obj["body"].ToString(),
            };
            return ra;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(RuleAction) == objectType;
        }
    }
}
