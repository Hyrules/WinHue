using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Hue Tap Sensor State.
    /// </summary>
    [DataContract]
    public class HueTapSensorState : SensorState
    {
        /// <summary>
        /// Button event number.
        /// </summary>
        [DataMember, HueLib(false, false)]
        public int? buttonevent { get; set; }

    }
}
