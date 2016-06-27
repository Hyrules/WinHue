using System.Runtime.Serialization;

namespace HueLib_base
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
