using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.ClipGenericStatus
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericStatusSensorState  : ValidatableBindableBase, ISensorState
    {
        private int _status;
        private string _lastupdated;

        /// <summary>
        /// Sensor Status.
        /// </summary>
        [HueProperty,DataMember]
        public int status
        {
            get => _status;
            set => SetProperty(ref _status,value);
        }

        [HueProperty,DataMember, JsonIgnore]
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
