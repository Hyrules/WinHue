using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence
{
    /// <summary>
    /// Sensor for detecting presence.
    /// </summary>
    [DataContract]
    public class PresenceSensorState : SensorStateBase
    {
        private bool? _presence;

        /// <summary>
        /// Presense detected.
        /// </summary>
        [HueProperty, DataMember]
        public bool? presence
        {
            get => _presence;
            set => SetProperty(ref _presence, value);
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
