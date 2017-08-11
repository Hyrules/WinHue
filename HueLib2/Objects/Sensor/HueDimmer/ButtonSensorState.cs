using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    /// <summary>
    /// Hue Tap Sensor State.
    /// </summary>
    [DataContract]
    public class ButtonSensorState : ISensorState, IRuleBody
    {
        /// <summary>
        /// Button event number.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public int? buttonevent { get; set; }
        [DataMember]
        public string lastupdated { get; set; }
    }
}
