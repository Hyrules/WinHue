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
        /// url.
        /// </summary>
        [DataMember]
        public string url { get; set; }
        /// <summary>
        /// On off state.
        /// </summary>
        [DataMember]
        public bool? on { get; set; }
        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [DataMember]
        public bool? reachable { get; set; }
        /// <summary>
        /// Battery state.
        /// </summary>
        [DataMember]
        public bool? battery { get; set; }

        /// <summary>
        /// Longitude of the sensor.
        /// </summary>
        [DataMember]
        public string @long { get; set; }

        /// <summary>
        /// Latitude of the sensor.
        /// </summary>
        [DataMember]
        public string lat { get; set; }

        /// <summary>
        /// Sunrise offset.
        /// </summary>
        [DataMember]
        public sbyte? sunriseoffset { get; set; }

        /// <summary>
        /// Sunset offsett.
        /// </summary>
        [DataMember]
        public sbyte? sunsetoffset { get; set; }

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
