using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence
{
    /// <summary>
    /// Sensor for detecting presence.
    /// </summary>
    [JsonObject]
    public class PresenceSensorState : ValidatableBindableBase, ISensorStateBase
    {
        private bool? _presence;

        /// <summary>
        /// Presense detected.
        /// </summary>
        public bool? presence
        {
            get => _presence;
            set => SetProperty(ref _presence, value);
        }

        private string _lastupdated;

        [DontSerialize,ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }
    }
}
