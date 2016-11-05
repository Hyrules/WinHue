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
        [DataMember,HueLib(false,false)]
        public int status { get; set; }
    }
}
