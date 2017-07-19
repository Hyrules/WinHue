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
            Messages msg = new Messages();
            JArray obj = serializer.Deserialize<JArray>(reader);
            foreach (JObject tk in obj)
            {
                Dictionary<string,JToken> dic = tk.ToObject<Dictionary<string, JToken>>();
                switch(dic[0].Keys[0].ToString())
                {
                    case "success":

                        break;
                }
                // KeyValuePair<string, JToken> kvp = (KeyValuePair<string, JToken>)tk. ;
             //   switch (((KeyValuePair<string,JToken>)tk).Key)
            }
            return obj;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Messages);
        }
    }
}
