using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipOpenClose
{
    
    /// <summary>
    /// SensorState class.
    /// </summary>
    [DataContract]
    public class ClipOpenCloseSensorState : SensorStateBase
    {
        private bool _open;

        /// <summary>
        /// Open or close.
        /// </summary>
        [HueProperty, DataMember]
        public bool open
        {
            get => _open;
            set => SetProperty(ref _open,value);
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
