using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor for detecting presence.
    /// </summary>
    [DataContract]
    public class PresenceSensorState : SensorState
    {
        /// <summary>
        /// Presense detected.
        /// </summary>
        [DataMember]
        public bool? presence { get; set; }
    }
}
