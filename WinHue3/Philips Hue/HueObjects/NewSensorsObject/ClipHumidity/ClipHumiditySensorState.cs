using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity
{
    /// <summary>
    /// Humidity Sensor State.
    /// </summary>
    [JsonObject]
    public class ClipHumiditySensorState : ValidatableBindableBase, ISensorStateBase
    { 
        private int _humidity;

        /// <summary>
        /// humidity.
        /// </summary>
        public int humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity,value);
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
