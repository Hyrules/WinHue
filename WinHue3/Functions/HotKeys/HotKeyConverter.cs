using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Input;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.Functions.HotKeys
{
    public class HotkeyConverter : JsonConverter
    {
        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // NOT USED.

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            HotKey hk = new HotKey();
            JObject obj = serializer.Deserialize<JObject>(reader);
            hk.Description = obj["Description"]?.Value<string>();
            hk.Name = obj["Name"]?.Value<string>();
            hk.Key = obj["Key"].ToObject<Key>();
            hk.Modifier = obj["Modifier"].ToObject<ModifierKeys>();
            hk.id = obj["id"]?.Value<string>();
            hk.objecType = obj["objecType"]?.ToObject<Type>();
            hk.ProgramPath = obj["ProgramPath"]?.Value<string>();

            if (obj["properties"] == null) return hk;
            switch (hk.objecType?.Name)
            {
                case "Light":
                    hk.properties = obj["properties"].ToObject<State>();
                    break;
                case "Group":
                    hk.properties = obj["properties"].ToObject<Action>();
                    break;
                case "Scene":
                    hk.properties = obj["properties"].ToObject<Action>();
                    break;
                default:
                    break;

            }
            return hk;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HotKey);
        }
    }
}
