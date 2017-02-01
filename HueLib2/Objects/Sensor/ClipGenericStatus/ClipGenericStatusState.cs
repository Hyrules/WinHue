using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericStatusState : SensorState
    {
        /// <summary>
        /// Sensor Status.
        /// </summary>
        [JsonIgnore]
        [DataMember]
        public int status { get; set; }
    }
}
