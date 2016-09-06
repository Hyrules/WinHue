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
        [DataMember, HueLib(true, true)]
        public string url { get; set; }
        /// <summary>
        /// On off state.
        /// </summary>
        [DataMember,HueLib(false,false)]
        public bool? on { get; set; }
        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [DataMember, HueLib(false, false)]
        public bool? reachable { get; set; }
        /// <summary>
        /// Battery state.
        /// </summary>
        [DataMember, HueLib(false, false)]
        public bool? battery { get; set; }

        /// <summary>
        /// Longitude of the sensor.
        /// </summary>
        [DataMember,HueLib(true,true)]
        public string @long { get; set; }

        /// <summary>
        /// Latitude of the sensor.
        /// </summary>
        [DataMember, HueLib(true, true)]
        public string lat { get; set; }

        /// <summary>
        /// Sunrise offset.
        /// </summary>
        [DataMember, HueLib(true, true)]
        public sbyte? sunriseoffset { get; set; }

        /// <summary>
        /// Sunset offsett.
        /// </summary>
        [DataMember, HueLib(true, true)]
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
