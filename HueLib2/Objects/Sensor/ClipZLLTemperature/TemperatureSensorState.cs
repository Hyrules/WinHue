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
        [DataMember,HueLib(false,false)]
        public int temperature { get; set; }
    }
}
