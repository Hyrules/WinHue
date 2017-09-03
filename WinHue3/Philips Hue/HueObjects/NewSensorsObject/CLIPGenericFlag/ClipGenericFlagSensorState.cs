using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.SensorObject;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericFlagSensorState : SensorStateBase
    {
        private bool _flag;

        /// <summary>
        /// url.
        /// </summary>
        [HueProperty,DataMember]
        public bool flag
        {
            get => _flag;
            set => SetProperty(ref _flag,value);
        }
    }
}
