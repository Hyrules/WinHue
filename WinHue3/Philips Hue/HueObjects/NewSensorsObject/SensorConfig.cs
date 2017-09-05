using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    public class SensorConfig : ValidatableBindableBase
    {
        private string _url;
        private bool? _on;
        private bool? _reachable;
        private byte? _battery;
        private uint? _tholddark;
        private uint? _tholdoffset;
        private string _l;
        private string _lat;
        private sbyte? _sunriseoffset;
        private sbyte? _sunsetoffset;
        private bool? _configured;
        private string _alert;
        private int? _sensitivitymax;
        private int? _sensitivity;

        [HueProperty, DataMember]
        public int? sensitivity
        {
            get => _sensitivity;
            set => SetProperty(ref _sensitivity, value);
        }

        [HueProperty, DataMember]
        public int? sensitivitymax
        {
            get => _sensitivitymax;
            set => SetProperty(ref _sensitivitymax, value);
        }

        /// <summary>
        /// Alert.
        /// </summary>
        [HueProperty, DataMember]
        public string alert
        {
            get => _alert;
            set => SetProperty(ref _alert, value);
        }
        /// <summary>
        /// url.
        /// </summary>
        [HueProperty, DataMember]
        public string url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        /// <summary>
        /// On off state.
        /// </summary>
        [HueProperty, DataMember]
        public bool? on
        {
            get => _on;
            set => SetProperty(ref _on, value);
        }

        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public bool? reachable
        {
            get => _reachable;
            set => SetProperty(ref _reachable, value);
        }

        /// <summary>
        /// Battery state.
        /// </summary>
        [HueProperty, DataMember]
        public byte? battery
        {
            get => _battery;
            set => SetProperty(ref _battery, value);
        }

        /// <summary>
        /// Threshold for insufficient light level.
        /// </summary>
        [HueProperty, DataMember]
        public uint? tholddark
        {
            get => _tholddark;
            set => SetProperty(ref _tholddark, value);
        }

        /// <summary>
        /// Threshold for sufficient light leve.
        /// </summary>
        [HueProperty, DataMember]
        public uint? tholdoffset
        {
            get => _tholdoffset;
            set => SetProperty(ref _tholdoffset, value);
        }

        /// <summary>
        /// Longitude of the sensor.
        /// </summary>
        [HueProperty, DataMember]
        public string @long
        {
            get => _l;
            set => SetProperty(ref _l, value);
        }

        /// <summary>
        /// Latitude of the sensor.
        /// </summary>
        [HueProperty, DataMember]
        public string lat
        {
            get => _lat;
            set => SetProperty(ref _lat, value);
        }

        /// <summary>
        /// Sunrise offset.
        /// </summary>
        [HueProperty, DataMember]
        public sbyte? sunriseoffset
        {
            get => _sunriseoffset;
            set => SetProperty(ref _sunriseoffset, value);
        }

        /// <summary>
        /// Sunset offsett.
        /// </summary>
        [HueProperty, DataMember]
        public sbyte? sunsetoffset
        {
            get => _sunsetoffset;
            set => SetProperty(ref _sunsetoffset, value);
        }

        /// <summary>
        /// Is The Sensor Configured.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public bool? configured
        {
            get => _configured;
            set => SetProperty(ref _configured, value);
        }

    }
}
