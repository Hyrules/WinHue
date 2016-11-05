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
        [DataMember,HueLib(false,false)]
        public bool open { get; set; }
    }
}
