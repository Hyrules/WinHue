using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    [DataContract]
    public class HueMotionSensorConfig : SensorConfig
    {

        /// <summary>
        /// Alert.
        /// </summary>
        [DataMember, HueLib(true,true)]
        public string alert { get; set; }
        /// <summary>
        /// On off state.
        /// </summary>
        [DataMember, HueLib(false, false)]
        public bool? on { get; set; }
        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [DataMember, HueLib(false, false)]
        public bool? reachable { get; set; }
        /// <summary>
        /// Battery state.
        /// </summary>
        [DataMember, HueLib(false, false)]
        public bool? battery { get; set; }
    }
}
