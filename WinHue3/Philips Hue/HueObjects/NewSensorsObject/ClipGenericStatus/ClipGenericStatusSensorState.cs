using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericStatusSensorState  : SensorStateBase
    {
        private int _status;


        /// <summary>
        /// Sensor Status.
        /// </summary>
        [HueProperty,DataMember]
        public int status
        {
            get => _status;
            set => SetProperty(ref _status,value);
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
