using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    public class RuleConditionJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RuleCondition rc = (RuleCondition) value;
            writer.WriteStartObject();
            writer.WritePropertyName("address");
            writer.WriteValue(rc.address);
            writer.WritePropertyName("operator");
            writer.WriteValue(rc.@operator);
            writer.WriteValue(rc.value);
            writer.WriteEndObject();
            writer.WriteEnd();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            RuleCondition rc = new RuleCondition
            {
                address = new HueAddress(obj["address"].Value<string>()),
                @operator = obj["operator"].Value<string>()
            };

            if(obj.ContainsKey("value"))
                rc.value = obj["value"].Value<string>();
            return rc;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(RuleCondition) == objectType;
        }
    }
}
