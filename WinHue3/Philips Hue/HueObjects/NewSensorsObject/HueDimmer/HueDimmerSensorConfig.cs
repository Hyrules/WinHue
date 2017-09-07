using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer
{
    [DataContract]
    public class HueDimmerSensorConfig : SensorConfigBase
    { 
        private string _alert;
        private bool? _on;
        private bool? _reachable;
        private byte? _battery;

        /// <summary>
        /// Alert.
        /// </summary>
        [HueProperty, DataMember]
        public string alert
        {
            get => _alert;
            set => SetProperty(ref _alert,value);
        }

        /// <summary>
        /// On off state.
        /// </summary>
        [HueProperty, DataMember]
        public bool? on
        {
            get => _on;
            set => SetProperty(ref _on,value);
        }

        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public bool? reachable
        {
            get => _reachable;
            set => SetProperty(ref _reachable,value);
        }

        /// <summary>
        /// Battery state.
        /// </summary>
        [HueProperty, DataMember,ReadOnly(true)]
        public byte? battery
        {
            get => _battery;
            set => SetProperty(ref _battery,value);
        }

    }
}
