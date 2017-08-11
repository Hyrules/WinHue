using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    /// <summary>
    /// Class Whitelist
    /// </summary>
    [DataContract]
    public class Whitelist
    {
        /// <summary>
        /// id of the user.
        /// </summary>
        [DataMember,JsonProperty(Required = Required.Default)]
        public string Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }
        /// <summary>
        /// LastUseDate
        /// </summary>
        [DataMember(Name = "last use date")]
        public string LastUseDate { get; set; }
        /// <summary>
        /// Created Date
        /// </summary>
        [DataMember(Name = "create date")]
        public string CreateDate { get; set; }


    }


    public class WhiteListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer,value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            Whitelist wl = new Whitelist
            {
                CreateDate = obj["create date"].Value<string>(),
                LastUseDate = obj["last use date"].Value<string>(),
                Name = obj["name"].Value<string>()
            };
            return wl;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Whitelist);
        }
    }
}
