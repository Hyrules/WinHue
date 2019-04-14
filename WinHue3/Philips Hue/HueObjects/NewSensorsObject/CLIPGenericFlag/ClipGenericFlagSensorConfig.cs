using System.ComponentModel;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag
{
    [JsonObject,ExpandableObject]
    public class ClipGenericFlagSensorConfig : ValidatableBindableBase,ISensorConfigBase
    {
        private string _url;
        private bool? _on;
        private bool? _reachable;
        private byte? _battery;

        /// <summary>
        /// url.
        /// </summary>
        public string url
        {
            get => _url;
            set => SetProperty(ref _url,value);
        }

        /// <summary>
        /// On off state.
        /// </summary>
        public bool? on
        {
            get => _on;
            set => SetProperty(ref _on,value);
        }

        /// <summary>
        /// Sensor reachability.
        /// </summary>
        [DontSerialize,ReadOnly(true)]
        public bool? reachable
        {
            get => _reachable;
            set => SetProperty(ref _reachable,value);
        }

        /// <summary>
        /// Battery state.
        /// </summary>

        public byte? battery
        {
            get => _battery;
            set => SetProperty(ref _battery,value);
        }

        public override string ToString()
        {
            return Serializer.SerializeJsonObject(this);
        }
    }
}
