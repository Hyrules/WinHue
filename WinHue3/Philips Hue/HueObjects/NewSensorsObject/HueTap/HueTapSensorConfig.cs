using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueTap
{
    [JsonObject]
    public class HueTapSensorConfig : ValidatableBindableBase, ISensorConfigBase
    {
        private bool? _on;

        /// <summary>
        /// On off state.
        /// </summary>
        public bool? on
        {
            get => _on;
            set => SetProperty(ref _on,value);
        }

    }
}
