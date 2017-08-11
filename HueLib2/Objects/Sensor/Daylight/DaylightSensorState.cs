using System.ComponentModel;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor for the daylight saving time.
    /// </summary>
    [DataContract]
    public class DaylightSensorState : ISensorState, IRuleBody
    {
        /// <summary>
        /// daylight saving time or not.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public bool? daylight { get; set; }
        [DataMember]
        public string lastupdated { get; set; }
    }
}
