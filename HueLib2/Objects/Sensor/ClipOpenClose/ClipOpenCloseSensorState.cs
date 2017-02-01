using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    
    /// <summary>
    /// SensorState class.
    /// </summary>
    [DataContract]
    public class ClipOpenCloseSensorState : SensorState
    {
        /// <summary>
        /// Open or close.
        /// </summary>
        [JsonIgnore]
        [DataMember]
        public bool open { get; set; }
    }
}
