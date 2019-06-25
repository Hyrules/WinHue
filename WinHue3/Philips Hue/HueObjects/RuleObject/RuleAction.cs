using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    /// <summary>
    /// Actions.
    /// </summary>
    [JsonObject,ExpandableObject/*,JsonConverter(typeof(RuleActionJsonConverter))*/]
    public class RuleAction
    {
        /// <summary>
        /// Address.
        /// </summary>
        public HueAddress address { get; set; }
        /// <summary>
        /// Method.
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// Body.
        /// </summary>
        [JsonConverter(typeof(RuleBodyConverter))]
        public string body { get; set; }

        /// <summary>
        /// Convert the rule action to a string.
        /// </summary>
        /// <returns></returns>
        
        public override string ToString()
        {
            return $"{address} {method} {body}";
        }
    }

    public class RuleBodyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string val = (string) value;
            JObject obj = JObject.Parse(val);
            obj.WriteTo(writer);
            

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            return obj.ToString();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
