using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer
{
    /// <summary>
    /// Hue Tap Sensor State.
    /// </summary>
    [DataContract]
    public class ButtonSensorState : SensorStateBase
    {
        private int? _buttonevent;

        /// <summary>
        /// Button event number.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public int? buttonevent
        {
            get => _buttonevent;
            set => SetProperty(ref _buttonevent,value);
        }

        private string _lastupdated;

        [HueProperty, DataMember]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }

    }
}
