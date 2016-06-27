using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HueLib_base
{
    /// <summary>
    /// Generic Sensor State.
    /// </summary>
    [DataContract]
    public class SensorState : RuleBody
    {
        /// <summary>
        /// LastUpdated
        /// </summary>
        [DataMember]
        public string lastupdated { get; set; }

        /// <summary>
        /// Convert state to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
            return JsonConvert.SerializeObject(this, jss);
        }
    }
}
