using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.SceneObject
{
    /// <summary>
    /// Scene App Data object
    /// </summary>
    [JsonObject]
    public class AppData : ValidatableBindableBase
    {
        private int? _version;
        private string _data;

        /// <summary>
        /// Version info.
        /// </summary>
        [Category("Apddata Properties"),Description("App specific version of the data field. App should take versioning into account when parsing the data string.")]
        public int? version
        {
            get => _version;
            set => SetProperty(ref _version,value);
        }

        /// <summary>
        /// Free format string.
        /// </summary>
        [Category("Apddata Properties"),Description("App specific data. Free format string.")]
        public string data
        {
            get => _data;
            set => SetProperty(ref _data,value);
        }

        /// <summary>
        /// Convert the object to Json serialized string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return $"Version {version}, Data {data}";
        }
    }
}
