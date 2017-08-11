using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.Daylight
{
    /// <summary>
    /// Sensor for the daylight saving time.
    /// </summary>
    [DataContract]
    public class DaylightSensorState : ValidatableBindableBase, ISensorState
    {
        private string _lastupdated;
        private bool? _daylight;

        /// <summary>
        /// daylight saving time or not.
        /// </summary>
        [HueProperty, DataMember, ReadOnly(true)]
        public bool? daylight
        {
            get => _daylight;
            set => SetProperty(ref _daylight,value);
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
