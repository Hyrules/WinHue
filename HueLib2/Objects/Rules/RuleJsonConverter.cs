using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2.Objects.Rules
{
    public class RuleJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Rule);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = (JObject)serializer.Deserialize(reader);
            Rule rule = new Rule();
            rule.name = obj["name"].ToString();

            if (obj["owner"] != null)
                rule.owner = obj["owner"].ToObject<string>();

            if (obj["timestriggered"] != null)
                rule.timestriggered = obj["timestriggered"].ToObject<int>();

            if (obj["lasttriggered"] != null)
                rule.lasttriggered = obj["lasttriggered"].ToObject<string>();

            if (obj["created"] != null)
                rule.created = obj["created"].ToObject<string>();

            if (obj["status"] != null)
                rule.status = obj["status"].ToObject<string>();

            if(obj["actions"] != null)
            {
                JArray actions = obj["actions"].ToObject<JArray>();
                foreach(JToken t in actions)
                {

                }
            }


            return rule;


        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
