using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.HueDimmer
{
    /// <summary>
    /// Hue Tap Sensor State.
    /// </summary>
    [DataContract]
    public class ButtonSensorState : ValidatableBindableBase, ISensorState
    {
        private int? _buttonevent;
        private string _lastupdated;

        /// <summary>
        /// Button event number.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public int? buttonevent
        {
            get => _buttonevent;
            set => SetProperty(ref _buttonevent,value);
        }

        [HueProperty, DataMember]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated,value);
        }

        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }
    }
}
