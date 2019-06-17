using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.ScheduleObject
{
    public class CommandJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Command cmd = (Command) value;
            writer.WriteStartObject();
            writer.WritePropertyName("address");
            writer.WriteValue(cmd.address);
            writer.WritePropertyName("body");
            writer.WriteRawValue(cmd.body);
            writer.WritePropertyName("method");
            writer.WriteValue(cmd.method);
            writer.WriteEndObject();
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Command cmd = new Command();
            JObject obj = serializer.Deserialize<JObject>(reader);
            cmd.address = new HueAddress(obj["address"].Value<string>());
            cmd.body = obj["body"].ToString();
            cmd.method = obj["method"].Value<string>();
            return cmd;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Command);
        }
    }
}
