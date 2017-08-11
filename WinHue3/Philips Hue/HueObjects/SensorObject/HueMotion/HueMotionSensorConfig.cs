using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.HueMotion
{
    [DataContract]
    public class HueMotionSensorConfig : ValidatableBindableBase, ISensorConfig
    {
        private byte? _battery;
        private bool? _reachable;
        private bool? _on;
        private string _alert;
        private int? _sensitivitymax;
        private int? _sensitivity;

        [HueProperty, DataMember]
        public int? sensitivity
        {
            get => _sensitivity;
            set => SetProperty(ref _sensitivity,value);
        }

        [HueProperty, DataMember]
        public int? sensitivitymax
        {
            get => _sensitivitymax;
            set => SetProperty(ref _sensitivitymax,value);
        }

        /// <summary>
        /// Alert.
        /// </summary>
        [HueProperty, DataMember, ItemsSource(typeof(AlertItemsSource))]
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
        [HueProperty, DataMember, ReadOnly(true)]
        public byte? battery
        {
            get => _battery;
            set => SetProperty(ref _battery,value);
        }

        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }
    }
}
