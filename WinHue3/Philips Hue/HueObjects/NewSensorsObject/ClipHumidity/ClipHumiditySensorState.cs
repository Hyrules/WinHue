using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity
{
    /// <summary>
    /// Humidity Sensor State.
    /// </summary>
    [DataContract]
    public class ClipHumiditySensorState : SensorStateBase
    {
        private int _humidity;

        /// <summary>
        /// humidity.
        /// </summary>
        [HueProperty, DataMember]
        public int humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity,value);
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
