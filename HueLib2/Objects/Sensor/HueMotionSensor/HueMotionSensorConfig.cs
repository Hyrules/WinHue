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
    public class HueMotionSensorConfig : ISensorConfig
    {
        [DataMember]
        public int? sensitivity { get; set; }

        [DataMember]
        public int? sensitivitymax { get; internal set; }
        /// <summary>
        /// Alert.
        /// </summary>
        [DataMember]
        public string alert { get; set; }
        /// <summary>
        /// On off state.
        /// </summary>
        [DataMember]
        public bool? on { get; set; }
        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [DataMember,ReadOnly(true)]
        public bool? reachable { get; set; }
        /// <summary>
        /// Battery state.
        /// </summary>
        [DataMember, CreateOnly]
        public byte? battery { get; set; }
    }
}
