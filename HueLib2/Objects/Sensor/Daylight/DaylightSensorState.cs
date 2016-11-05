using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor for the daylight saving time.
    /// </summary>
    [DataContract]
    public class DaylightSensorState : SensorState
    {
        /// <summary>
        /// daylight saving time or not.
        /// </summary>
        [DataMember, HueLib(false, false)]
        public bool? daylight { get; set; }

    }
}
