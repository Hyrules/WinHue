using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.ClipZllLightLevel
{
    [DataContract]
    public class LightLevelState : ValidatableBindableBase, ISensorState
    {
        private ushort? _lightlevel;
        private bool? _dark;
        private bool? _daylight;
        private string _lastupdated;

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