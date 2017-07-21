using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2.BridgeMessages
{
    public class MessagesConverter : JsonConverter
    {
     
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Messages msg = new Messages();
            JArray obj = serializer.Deserialize<JArray>(reader);
            foreach (JToken tk in obj)
            {
                JObject jo = tk as JObject;
                List<JProperty> jp = jo.Properties().ToList();
                foreach (JProperty j in jp)
                {
                    switch (j.Name)
                    {
                        case "success":
                            msg.SuccessMessages.Add(ProcessSuccess(j.Value));
                            break;
                        case "error":
                            msg.ErrorMessages.Add(ProcessError(j.Value));
                            break;
                        default:
                            break;
                    }
                }
                

            }
            return msg;
        }

        private Error ProcessError(JToken token)
        {
            Error e = new Error();
            JObject o = token as JObject;
            List<JProperty> p = o.Properties().ToList();
            foreach (JProperty prop in p)
            {
                if (prop.Name == "type")
                    e.type = prop.Value.Value<int>();
                if (prop.Name == "address")
                    e.address = prop.Value.Value<string>();
                if (prop.Name == "description")
                    e.description = prop.Value.Value<string>();
            }

            return e;
        }

        private Success ProcessSuccess(JToken token)
        {
            Success s = new Success();
            JObject o = token as JObject;
            List<JProperty> p = o.Properties().ToList();
            foreach (JProperty prop in p)
            {
                s.Address = prop.Name;
                s.value = prop.Value.Value<string>();
                
            }
            return s;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Messages);
        }
    }
}
