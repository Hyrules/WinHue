using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    
    /// <summary>
    /// SensorState class.
    /// </summary>
    [DataContract]
    public class ClipOpenCloseSensorState : ISensorState, IRuleBody
    {
        /// <summary>
        /// Open or close.
        /// </summary>
        [DataMember]
        public bool open { get; set; }
        [DataMember]
        public string lastupdated { get; set; }
    }
}
