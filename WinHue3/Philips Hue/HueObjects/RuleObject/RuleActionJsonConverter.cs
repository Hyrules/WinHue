using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    public class RuleActionJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RuleAction oldra = (RuleAction) value;
            List<PropertyInfo> prop = oldra.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList();

            writer.WriteStartObject();
            foreach (PropertyInfo p in prop)
            {
                if(p.GetValue(oldra) == null) continue;
                writer.WritePropertyName(p.Name);
                if (p.Name == "address")
                {
                    writer.WriteValue(p.GetValue(oldra).ToString());
                }
                else if(p.Name == "body")
                {
                    writer.WriteRawValue(oldra.body);
                }
                else
                {
                    writer.WriteValue(p.GetValue(oldra));
                }
            }
            writer.WriteEnd();
            //writer.WriteEndObject();
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            RuleAction newRuleAction = new RuleAction();
            if (obj["address"] != null)
                newRuleAction.address = obj["address"].ToObject<HueAddress>();
            if (obj["method"] != null)
                newRuleAction.method = obj["method"].Value<string>();
            if (obj["body"] != null)
                newRuleAction.body = JsonConvert.SerializeObject(obj["body"]);            

            return newRuleAction;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(RuleAction) == objectType;
        }
    }
}
