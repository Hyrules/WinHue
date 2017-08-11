using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.ClipGenericFlag
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericFlagSensorState : ValidatableBindableBase, ISensorState
    {
        private bool _flag;
        private string _lastupdated;

        /// <summary>
        /// url.
        /// </summary>
        [HueProperty,DataMember]
        public bool flag
        {
            get => _flag;
            set => SetProperty(ref _flag,value);
        }

        [HueProperty,DataMember]
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
