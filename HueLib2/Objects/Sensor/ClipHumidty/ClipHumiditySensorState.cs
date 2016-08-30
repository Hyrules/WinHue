using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Humidity Sensor State.
    /// </summary>
    [DataContract]
    public class ClipHumiditySensorState : SensorState
    {
        /// <summary>
        /// humidity.
        /// </summary>
        [DataMember]
        public int humidity { get;set; }
    }
}
