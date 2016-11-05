using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor for detecting presence.
    /// </summary>
    [DataContract]
    public class ClipPresenceSensorState : SensorState
    {
        /// <summary>
        /// Presense detected.
        /// </summary>
        [DataMember,HueLib(false,false)]
        public bool presence { get; set; }
    }
}
