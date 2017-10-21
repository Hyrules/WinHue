using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    public class RuleJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Rule oldrule = (Rule) value;
            List<PropertyInfo> props = oldrule.GetType().GetListHueProperties();
            writer.WriteStartObject();
            
            foreach (PropertyInfo p in props)
            {
                if (p.GetValue(oldrule) == null) continue;
                if (Attribute.IsDefined(p, typeof(JsonIgnoreAttribute))) continue;
                writer.WritePropertyName(p.Name);
                if (p.Name == "conditions" )
                {
                    string str = JsonConvert.SerializeObject(oldrule.conditions, new JsonSerializerSettings(){ NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });                    
                    writer.WriteRawValue(str);
                }
                else if (p.Name == "actions")
                {
                    string str = JsonConvert.SerializeObject(oldrule.actions, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
                    writer.WriteRawValue(str);
                }
                else
                {
                    writer.WriteValue(p.GetValue(oldrule));
                }
                
            }
            writer.WriteEnd();
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            Rule newRule = new Rule();
            if (obj["created"] != null)
                newRule.created = obj["created"].Value<string>();
            if (obj["lasttriggered"] != null)
                newRule.lasttriggered = obj["lasttriggered"].Value<string>();
            if (obj["owner"] != null)
                newRule.owner = obj["owner"].Value<string>();
            if (obj["status"] != null)
                newRule.status = obj["status"].Value<string>();
            if (obj["name"] != null)
                newRule.name = obj["name"].Value<string>();
            if (obj["timestriggered"] != null)
                newRule.timestriggered = obj["timestriggered"].Value<int>();

            if (obj["actions"] != null)
                newRule.actions = obj["actions"].ToObject<RuleActionCollection>();
            if (obj["conditions"] != null)
                newRule.conditions = obj["conditions"].ToObject<RuleConditionCollection>();
            return newRule;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Rule) == objectType;
        }
    }
}
