using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Temperature sensor state.
    /// </summary>
    [DataContract]
    public class TemperatureSensorState : SensorState
    {
        /// <summary>
        /// Current temperature.
        /// </summary>
        [JsonIgnore]
        [DataMember]
        public int? temperature { get; set; }
    }
}
