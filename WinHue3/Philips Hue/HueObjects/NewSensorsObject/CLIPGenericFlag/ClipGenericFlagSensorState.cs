using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericFlagSensorState : ValidatableBindableBase, ISensorStateBase
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

        private string _lastupdated;

        [HueProperty, DataMember, ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }
    }
}
