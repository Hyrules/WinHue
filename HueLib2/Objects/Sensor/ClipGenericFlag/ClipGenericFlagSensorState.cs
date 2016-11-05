using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericFlagSensorState : SensorState
    {
        /// <summary>
        /// url.
        /// </summary>
        [DataMember, HueLib(false,false)]
        public bool flag { get; set; }
    }
}
