using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence
{
    /// <summary>
    /// Sensor for detecting presence.
    /// </summary>
    [DataContract]
    public class PresenceSensorState : ValidatableBindableBase, ISensorStateBase
    {
        private bool? _presence;

        /// <summary>
        /// Presense detected.
        /// </summary>
        [DataMember]
        public bool? presence
        {
            get => _presence;
            set => SetProperty(ref _presence, value);
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
