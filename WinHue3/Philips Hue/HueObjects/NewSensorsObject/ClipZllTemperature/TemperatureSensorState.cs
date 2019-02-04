using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllTemperature
{
    /// <summary>
    /// Temperature sensor state.
    /// </summary>
    [JsonObject]
    public class TemperatureSensorState : ValidatableBindableBase, ISensorStateBase
    {
        private int? _temperature;

        /// <summary>
        /// Current temperature.
        /// </summary>
        [DontSerialize,ReadOnly(true)]
        public int? temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature,value);
        }

        private string _lastupdated;

        [DontSerialize,ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }
    }
}
