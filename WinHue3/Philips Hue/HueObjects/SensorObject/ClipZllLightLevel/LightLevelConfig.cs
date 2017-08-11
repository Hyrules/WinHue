using System.Runtime.Serialization;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.SensorObject.ClipZllLightLevel
{
    [DataContract]
    public class LightLevelConfig : ValidatableBindableBase, ISensorConfig
    {
        private uint? _tholddark;
        private uint? _tholdoffset;

        /// <summary>
        /// Threshold for insufficient light level.
        /// </summary>
        [HueProperty, DataMember]
        public uint? tholddark
        {
            get => _tholddark;
            set => SetProperty(ref _tholddark,value);
        }

        /// <summary>
        /// Threshold for sufficient light leve.
        /// </summary>
        [HueProperty, DataMember]
        public uint? tholdoffset
        {
            get => _tholdoffset;
            set => SetProperty(ref _tholdoffset,value);
        }

        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }
    }
}
