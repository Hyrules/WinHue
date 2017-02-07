using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    [DataContract]
    public class LightLevelConfig : SensorConfig
    {
        /// <summary>
        /// Threshold for insufficient light level.
        /// </summary>
        [DataMember]
        public uint? tholddark { get; set; }

        /// <summary>
        /// Threshold for sufficient light leve.
        /// </summary>
        [DataMember]
        public uint? tholdoffset { get; set; }
    }
}
