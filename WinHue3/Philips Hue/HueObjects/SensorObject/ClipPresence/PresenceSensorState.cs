using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.ClipPresence
{
    /// <summary>
    /// Sensor for detecting presence.
    /// </summary>
    [DataContract]
    public class PresenceSensorState : ValidatableBindableBase, ISensorState
    {
        private bool? _presence;
        private string _lastupdated;

        /// <summary>
        /// Presense detected.
        /// </summary>
        [DataMember]
        public bool? presence
        {
            get => _presence;
            set => SetProperty(ref _presence, value);
        }

        [DataMember]
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
