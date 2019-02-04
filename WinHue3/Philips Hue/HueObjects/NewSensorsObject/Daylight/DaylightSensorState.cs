using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight
{
    /// <summary>
    /// Sensor for the daylight saving time.
    /// </summary>
    [JsonObject]
    public class DaylightSensorState : ValidatableBindableBase, ISensorStateBase
    {
 
        private bool? _daylight;

        /// <summary>
        /// daylight saving time or not.
        /// </summary>
        [DataMember, ReadOnly(true)]
        public bool? daylight
        {
            get => _daylight;
            set => SetProperty(ref _daylight,value);
        }

        private string _lastupdated;

        [DataMember, ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }
    }
}
