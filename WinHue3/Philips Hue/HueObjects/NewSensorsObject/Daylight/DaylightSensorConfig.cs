using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight
{
    [DataContract]
    public class DaylightSensorConfig : SensorConfigBase
    {
        private string _l;
        private string _lat;
        private sbyte? _sunriseoffset;
        private sbyte? _sunsetoffset;
        private bool? _configured;

        /// <summary>
        /// Longitude of the sensor.
        /// </summary>
        [HueProperty, DataMember]
        public string @long
        {
            get => _l;
            set => SetProperty(ref _l,value);
        }

        /// <summary>
        /// Latitude of the sensor.
        /// </summary>
        [HueProperty, DataMember]
        public string lat
        {
            get => _lat;
            set => SetProperty(ref _lat,value);
        }

        /// <summary>
        /// Sunrise offset.
        /// </summary>
        [HueProperty, DataMember]
        public sbyte? sunriseoffset
        {
            get => _sunriseoffset;
            set => SetProperty(ref _sunriseoffset,value);
        }

        /// <summary>
        /// Sunset offsett.
        /// </summary>
        [HueProperty, DataMember]
        public sbyte? sunsetoffset
        {
            get => _sunsetoffset;
            set => SetProperty(ref _sunsetoffset,value);
        }

        /// <summary>
        /// Is The Sensor Configured.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public bool? configured
        {
            get => _configured;
            set => SetProperty(ref _configured,value);
        }

    }
}
