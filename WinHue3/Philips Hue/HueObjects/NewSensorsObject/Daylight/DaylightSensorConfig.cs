using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight
{
    [DataContract]
    public class DaylightSensorConfig : ValidatableBindableBase, ISensorConfigBase
    {
        private string _l;
        private string _lat;
        private sbyte? _sunriseoffset;
        private sbyte? _sunsetoffset;
        private bool? _configured;

        /// <summary>
        /// Longitude of the sensor.
        /// </summary>
        [DataMember]
        public string @long
        {
            get => _l;
            set => SetProperty(ref _l,value);
        }

        /// <summary>
        /// Latitude of the sensor.
        /// </summary>
        [DataMember]
        public string lat
        {
            get => _lat;
            set => SetProperty(ref _lat,value);
        }

        /// <summary>
        /// Sunrise offset.
        /// </summary>
        [DataMember]
        public sbyte? sunriseoffset
        {
            get => _sunriseoffset;
            set => SetProperty(ref _sunriseoffset,value);
        }

        /// <summary>
        /// Sunset offsett.
        /// </summary>
        [DataMember]
        public sbyte? sunsetoffset
        {
            get => _sunsetoffset;
            set => SetProperty(ref _sunsetoffset,value);
        }

        /// <summary>
        /// Is The Sensor Configured.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public bool? configured
        {
            get => _configured;
            set => SetProperty(ref _configured,value);
        }

    }
}
