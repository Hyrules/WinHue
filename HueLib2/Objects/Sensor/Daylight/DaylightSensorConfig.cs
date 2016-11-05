using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    [DataContract]
    public class DaylightSensorConfig : SensorConfig
    {
        /// <summary>
        /// Longitude of the sensor.
        /// </summary>
        [DataMember, HueLib(true, true)]
        public string @long { get; set; }

        /// <summary>
        /// Latitude of the sensor.
        /// </summary>
        [DataMember, HueLib(true, true)]
        public string lat { get; set; }

        /// <summary>
        /// Sunrise offset.
        /// </summary>
        [DataMember, HueLib(true, true)]
        public sbyte? sunriseoffset { get; set; }

        /// <summary>
        /// Sunset offsett.
        /// </summary>
        [DataMember, HueLib(true, true)]
        public sbyte? sunsetoffset { get; set; }

        /// <summary>
        /// Is The Sensor Configured.
        /// </summary>
        [DataMember, HueLib(false,false)]
        public bool? configured { get; set; }
    }
}
