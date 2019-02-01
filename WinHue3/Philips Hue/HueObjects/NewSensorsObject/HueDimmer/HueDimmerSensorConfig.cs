﻿using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer
{
    [DataContract]
    public class HueDimmerSensorConfig : ValidatableBindableBase, ISensorConfigBase
    { 
        private string _alert;
        private bool? _on;
        private bool? _reachable;
        private byte? _battery;

        /// <summary>
        /// Alert.
        /// </summary>
        [DataMember]
        public string alert
        {
            get => _alert;
            set => SetProperty(ref _alert,value);
        }

        /// <summary>
        /// On off state.
        /// </summary>
        [DataMember]
        public bool? on
        {
            get => _on;
            set => SetProperty(ref _on,value);
        }

        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public bool? reachable
        {
            get => _reachable;
            set => SetProperty(ref _reachable,value);
        }

        /// <summary>
        /// Battery state.
        /// </summary>
        [DataMember,ReadOnly(true)]
        public byte? battery
        {
            get => _battery;
            set => SetProperty(ref _battery,value);
        }

    }
}
