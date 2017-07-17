using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HueLib2.BridgeMessages
{
    public class MessageConverter : JsonConverter
    {
        private static readonly string SUCCESS = typeof(Success).FullName;
        private static readonly string ERROR = typeof(Error).FullName;
        private static readonly string CR_SUCCESS = typeof(CreationSuccess).FullName;
        private static readonly string DEL_SUCCESS = typeof(DeletionSuccess).FullName;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.FullName == SUCCESS)
            {
                return serializer.Deserialize(reader, typeof(Success));
            }
            else if (objectType.FullName == ERROR)
            {
                return serializer.Deserialize(reader, typeof(Error));
            }
            else if (objectType.FullName == CR_SUCCESS)
            {
                return serializer.Deserialize(reader, typeof(CreationSuccess));
            }
            else if (objectType.FullName == DEL_SUCCESS)
            {
                return serializer.Deserialize(reader, typeof(DeletionSuccess));
            }
            throw new NotSupportedException($"Type {objectType} unexpected.");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.FullName == SUCCESS || objectType.FullName == ERROR || objectType.FullName == CR_SUCCESS || objectType.FullName == DEL_SUCCESS;
        }
    }
}
