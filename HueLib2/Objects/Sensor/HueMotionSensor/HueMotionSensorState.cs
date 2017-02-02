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
    [DataContract]
    public class HueMotionSensorState : SensorState
    {
        [DataMember, ReadOnly(true)]
        public bool? presence { get; set; }
    }
}
