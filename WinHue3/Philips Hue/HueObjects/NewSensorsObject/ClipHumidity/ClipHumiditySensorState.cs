using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity
{
    /// <summary>
    /// Humidity Sensor State.
    /// </summary>
    [DataContract]
    public class ClipHumiditySensorState : ValidatableBindableBase, ISensorStateBase
    { 
        private int _humidity;

        /// <summary>
        /// humidity.
        /// </summary>
        [DataMember]
        public int humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity,value);
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
