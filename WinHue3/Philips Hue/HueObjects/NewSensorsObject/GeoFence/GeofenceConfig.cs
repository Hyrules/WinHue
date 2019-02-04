using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.GeoFence
{
    [JsonObject]
    public class GeofenceConfig : ValidatableBindableBase, ISensorConfigBase
    {
        private bool _on;
        private bool _reachable;

        public bool on
        {
            get => _on;
            set => SetProperty(ref _on, value);
        }

        [DontSerialize,ReadOnly(true)]
        public bool reachable
        {
            get => _reachable;
            set => SetProperty(ref _reachable, value);
        }
    }
}
