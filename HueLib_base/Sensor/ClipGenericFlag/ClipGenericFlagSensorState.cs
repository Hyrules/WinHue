using System.Runtime.Serialization;

namespace HueLib_base
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
        [DataMember]
        public bool flag { get; set; }
    }
}
