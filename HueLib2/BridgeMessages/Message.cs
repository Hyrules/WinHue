using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2
{
    [DataContract, JsonConverter(typeof(MessageJsonConverter))]
    public abstract class Message
    {
    }

    public class MessageJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Message) ? true : false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Message msg = null;
            JObject obj = serializer.Deserialize<JObject>(reader);

            foreach (var o in obj)
            {

                if (o.Key == "error")
                {
                    string str = o.Value.ToString().Replace("\r\n","");
                    msg = JsonConvert.DeserializeObject<Error>(str);
                }
                else
                {
                    if (o.Value.ToString().Contains("deleted"))
                        msg = new DeletionSuccess() { success = o.Value.ToString() };
                    else if (o.Value.ToString().Contains("id"))
                    {
                        JObject jo = Serializer.DeserializeToObject<JObject>(o.Value.ToString());
                        msg = new CreationSuccess() { id = jo["id"].Value<string>() };
                    }
                    else
                    {
                        msg = Serializer.DeserializeToObject<Success>(o.Value.ToString());
                    }
                }
            }

            return msg;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
