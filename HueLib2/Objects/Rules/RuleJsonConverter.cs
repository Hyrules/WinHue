using System;
using System.Diagnostics.Eventing.Reader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HueLib2
{
    /// <summary>
    /// Json converter for the RuelBody Class.
    /// </summary>
    public class RuleBodyJsonConverter : JsonConverter
    {
        /// <summary>
        /// Check if the rulebody can be converted to json and back.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RuleBody) ? true : false;
        }

        /// <summary>
        /// Read a Rule Body and transform it to a string.
        /// </summary>
        /// <param name="reader">Json Reader</param>
        /// <param name="objectType">Typeo of Object</param>
        /// <param name="existingValue">Existing value.</param>
        /// <param name="serializer">Json Serializer.</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = (JObject)serializer.Deserialize(reader);
            RuleBody rulebody;

            if (obj.ToString().Contains("scene"))
            {
                rulebody = JsonConvert.DeserializeObject<SceneBody>(obj.ToString());
            }
            else if (obj.ToString().Contains("status")|| obj.ToString().Contains("autodelete") || obj.ToString().Contains("recycle"))
            {
                rulebody = JsonConvert.DeserializeObject<ScheduleBody>(obj.ToString());
            }
            else
            {
                rulebody = JsonConvert.DeserializeObject<State>(obj.ToString());
            }
            return rulebody;
        }

        /// <summary>
        /// Write string to json.
        /// </summary>
        /// <param name="writer">Json Writer</param>
        /// <param name="value">Value to write.</param>
        /// <param name="serializer">Json Serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

    }
}
