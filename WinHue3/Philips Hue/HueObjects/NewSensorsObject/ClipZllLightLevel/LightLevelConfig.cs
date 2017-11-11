using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllLightLevel
{
    [DataContract]
    public class LightLevelConfig : ValidatableBindableBase, ISensorConfigBase
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

    }
}
