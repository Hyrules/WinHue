using System.ComponentModel;
using Newtonsoft.Json;
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
        [DataMember, ReadOnly(true)]
        public int? buttonevent { get; set; }

    }
}
