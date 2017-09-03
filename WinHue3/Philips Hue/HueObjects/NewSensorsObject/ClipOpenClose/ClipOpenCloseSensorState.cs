using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.SensorObject;
using WinHue3.ViewModels;

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

    }
}
