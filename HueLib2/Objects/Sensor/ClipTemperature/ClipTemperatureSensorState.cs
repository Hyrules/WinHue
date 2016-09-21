using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Temperature sensor state.
    /// </summary>
    [DataContract]
    public class ClipTemperatureSensorState : SensorState
    {
        /// <summary>
        /// Current temperature.
        /// </summary>
        [DataMember]
        public int temperature { get; set; }
    }
}
