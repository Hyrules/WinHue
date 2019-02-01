using Newtonsoft.Json;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.SceneObject
{
    [JsonObject]
    public class SceneBody : ValidatableBindableBase
    {
        private string _scene;

        public string scene
        {
            get => _scene;
            set => SetProperty(ref _scene,value);
        }
    }
}
