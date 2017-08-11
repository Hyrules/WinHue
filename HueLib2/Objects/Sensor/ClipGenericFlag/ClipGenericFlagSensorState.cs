using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericFlagSensorState : ISensorState, IRuleBody
    {
        /// <summary>
        /// url.
        /// </summary>
        [DataMember]
        public bool flag { get; set; }
        [DataMember]
        public string lastupdated { get; set; }
    }
}
