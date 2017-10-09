using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight
{
    /// <summary>
    /// Sensor for the daylight saving time.
    /// </summary>
    [DataContract]
    public class DaylightSensorState : SensorStateBase
    {
 
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

        private string _lastupdated;

        [HueProperty, DataMember, ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }
    }
}
