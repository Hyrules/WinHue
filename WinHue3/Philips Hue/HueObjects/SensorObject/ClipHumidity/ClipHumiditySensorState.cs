using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.ClipHumidity
{
    /// <summary>
    /// Humidity Sensor State.
    /// </summary>
    [DataContract]
    public class ClipHumiditySensorState : ValidatableBindableBase, ISensorState
    {
        private int _humidity;
        private string _lastupdated;

        /// <summary>
        /// humidity.
        /// </summary>
        [HueProperty, DataMember]
        public int humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity,value);
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
