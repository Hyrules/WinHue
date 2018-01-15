using System;
using System.Runtime.Serialization;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.Functions.HotKeys
{
    [DataContract, JsonConverter(typeof(HotkeyConverter))]
    public class HotKey
    {
        [DataMember]
        public ModifierKeys Modifier { get; set; }
        [DataMember]
        public Key Key { get; set; }

        [DataMember]
        public Type objecType { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public IBaseProperties properties { get; set; }

        public string Hotkey => Modifier + " + " + Key;

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }

    }

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
