using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HueLib2
{
    /// <summary>
    /// SensorConfig Class.
    /// </summary>
    [DataContract]
    public class SensorConfig
    {

        /// <summary>
        /// convert state to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
            return JsonConvert.SerializeObject(this, jss);
        }
    }
}
