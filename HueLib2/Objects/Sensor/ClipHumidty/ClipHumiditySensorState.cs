using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Humidity Sensor State.
    /// </summary>
    [DataContract]
    public class ClipHumiditySensorState : ISensorState, IRuleBody
    {
        /// <summary>
        /// humidity.
        /// </summary>
        [DataMember]
        public int humidity { get;set; }
        [DataMember]
        public string lastupdated { get; set; }
    }
}
