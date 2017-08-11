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
    public class DaylightSensorConfig : ISensorConfig
    {
        /// <summary>
        /// Longitude of the sensor.
        /// </summary>
        [DataMember]
        public string @long { get; set; }

        /// <summary>
        /// Latitude of the sensor.
        /// </summary>
        [DataMember]
        public string lat { get; set; }

        /// <summary>
        /// Sunrise offset.
        /// </summary>
        [DataMember]
        public sbyte? sunriseoffset { get; set; }

        /// <summary>
        /// Sunset offsett.
        /// </summary>
        [DataMember]
        public sbyte? sunsetoffset { get; set; }

        /// <summary>
        /// Is The Sensor Configured.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public bool? configured { get; set; }
    }
}
