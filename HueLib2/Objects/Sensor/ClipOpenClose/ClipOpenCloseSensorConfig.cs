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
    public class ClipOpenCloseSensorConfig : ISensorConfig
    {
        /// <summary>
        /// url.
        /// </summary>
        [DataMember]
        public string url { get; set; }
        /// <summary>
        /// On off state.
        /// </summary>
        [DataMember]
        public bool? on { get; set; }
        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public bool? reachable { get; set; }
        /// <summary>
        /// Battery state.
        /// </summary>
        [DataMember,CreateOnly]
        public byte? battery { get; set; }
    }
}
