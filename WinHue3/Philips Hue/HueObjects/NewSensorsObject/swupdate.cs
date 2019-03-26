using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    [JsonObject]
    public class SwUpdate : ValidatableBindableBase
    {
        private string _state;
        private string _lastinstall;

        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public string Lastinstall
        {
            get { return _lastinstall; }
            set { SetProperty(ref _lastinstall,value); }
        }
    }
}
