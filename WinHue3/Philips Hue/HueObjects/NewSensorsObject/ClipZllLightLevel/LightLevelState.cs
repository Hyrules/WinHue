using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllLightLevel
{
    [DataContract]
    public class LightLevelState : SensorStateBase
    {
        private ushort? _lightlevel;
        private bool? _dark;
        private bool? _daylight;

        [HueProperty, DataMember, ReadOnly(true)]
        public ushort? lightlevel
        {
            get => _lightlevel;
            set => SetProperty(ref _lightlevel,value);
        }

        [HueProperty, DataMember, ReadOnly(true)]
        public bool? dark
        {
            get => _dark;
            set => SetProperty(ref _dark,value);
        }

        [HueProperty, DataMember, ReadOnly(true)]
        public bool? daylight
        {
            get => _daylight;
            set => SetProperty(ref _daylight,value);
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