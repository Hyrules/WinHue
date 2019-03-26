using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;


namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipOpenClose
{
    
    /// <summary>
    /// SensorState class.
    /// </summary>
    [JsonObject]
    public class ClipOpenCloseSensorState : ValidatableBindableBase, ISensorStateBase
    {
        private bool _open;

        /// <summary>
        /// Open or close.
        /// </summary>
        public bool open
        {
            get => _open;
            set => SetProperty(ref _open,value);
        }

        private string _lastupdated;
        [DontSerialize, ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated, value);
        }
    }
}
