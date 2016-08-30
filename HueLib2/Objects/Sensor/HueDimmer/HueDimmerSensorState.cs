using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    /// <summary>
    /// Hue Tap Sensor State.
    /// </summary>
    class HueDimmerSensorState : SensorState
    {
        /// <summary>
        /// Button event number.
        /// </summary>
        [DataMember]
        public int? buttonevent { get; set; }
    }
}
