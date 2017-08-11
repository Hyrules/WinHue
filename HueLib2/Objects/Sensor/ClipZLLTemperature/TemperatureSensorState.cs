using System.ComponentModel;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Temperature sensor state.
    /// </summary>
    [DataContract]
    public class TemperatureSensorState : ISensorState, IRuleBody
    {
        /// <summary>
        /// Current temperature.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public int? temperature { get; set; }
        [DataMember]
        public string lastupdated { get; set; }
    }
}
