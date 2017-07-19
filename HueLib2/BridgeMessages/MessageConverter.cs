using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2.BridgeMessages
{
    public class MessageConverter : JsonConverter
    {
       
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray obj = serializer.Deserialize<JArray>(reader);
            foreach (JToken tk in obj)
            {
                
            }
            return obj;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Messages);
        }
    }
}
