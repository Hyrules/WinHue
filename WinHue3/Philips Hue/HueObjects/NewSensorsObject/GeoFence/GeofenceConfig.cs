using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.GeoFence
{
    public class GeofenceConfig : ValidatableBindableBase, ISensorConfigBase
    {
        private bool _on;
        private bool _reachable;

        [DataMember]
        public bool on
        {
            get => _on;
            set => SetProperty(ref _on, value);
        }

        [DataMember]
        public bool reachable
        {
            get => _reachable;
            set => SetProperty(ref _reachable, value);
        }
    }
}
