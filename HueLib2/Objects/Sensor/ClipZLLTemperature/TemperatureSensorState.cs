using System.ComponentModel;
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
        [DataMember, ReadOnly(true)]
        public int? temperature { get; set; }
    }
}
