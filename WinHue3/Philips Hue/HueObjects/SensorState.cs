using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects
{
    public class SensorState : ValidatableBindableBase
    {
        private bool _flag;
        private int _status;
        private int _humidity;
        private bool _open;
        private bool? _presence;
        private ushort? _lightlevel;
        private bool? _dark;
        private bool? _daylight;
        private int? _temperature;
        private int? _buttonevent;

        /// <summary>
        /// url.
        /// </summary>
        [HueProperty, DataMember]
        public bool flag
        {
            get => _flag;
            set => SetProperty(ref _flag, value);
        }     

        /// <summary>
        /// Sensor Status.
        /// </summary>
        [HueProperty, DataMember]
        public int status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        
        /// <summary>
        /// humidity.
        /// </summary>
        [HueProperty, DataMember]
        public int humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity, value);
        }
       
        /// <summary>
        /// Open or close.
        /// </summary>
        [HueProperty, DataMember]
        public bool open
        {
            get => _open;
            set => SetProperty(ref _open, value);
        }

        /// <summary>
        /// Presense detected.
        /// </summary>
        [HueProperty, DataMember]
        public bool? presence
        {
            get => _presence;
            set => SetProperty(ref _presence, value);
        }

        [HueProperty, DataMember, ReadOnly(true)]
        public ushort? lightlevel
        {
            get => _lightlevel;
            set => SetProperty(ref _lightlevel, value);
        }

        [HueProperty, DataMember, ReadOnly(true)]
        public bool? dark
        {
            get => _dark;
            set => SetProperty(ref _dark, value);
        }

        [HueProperty, DataMember, ReadOnly(true)]
        public bool? daylight
        {
            get => _daylight;
            set => SetProperty(ref _daylight, value);
        }

        /// <summary>
        /// Current temperature.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public int? temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }


        /// <summary>
        /// Button event number.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public int? buttonevent
        {
            get => _buttonevent;
            set => SetProperty(ref _buttonevent, value);
        }
    }
}
