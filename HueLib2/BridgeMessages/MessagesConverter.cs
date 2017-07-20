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
                Dictionary<string,JToken> dic = tk.ToObject<Dictionary<string, JToken>>();
                foreach(KeyValuePair<string,JToken> d in dic)
                {
                    Dictionary<string, JToken> s = d.Value.ToObject<Dictionary<string, JToken>>();
                    switch (d.Key)
                    {
                            
                        case "success":
                            msg.SuccessMessages.Add(new Success(){ Address = s.Keys.ToArray()[0], value = s[s.Keys.ToArray()[0]].Value<string>()});
                            break;
                        case "error":
                            
                            msg.ErrorMessages.Add(new Error() { type = s["type"].Value<int>(), address = s["address"].Value<string>(), description = s["description"].Value<string>()});
                            break;
                        default:
                            break;
                    }
                }
            }
            return msg;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Messages);
        }
    }
}
