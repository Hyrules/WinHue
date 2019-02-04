using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    [JsonObject]
    public class LightSwUpdate : ValidatableBindableBase
    {
        private string _state;
        private string _lastinstall;

        [Category("Light Software Update"), Description("State")]
        public string state
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Software Update"), Description("Last install")]
        public string lastinstall
        {
            get => _lastinstall; 
            set => SetProperty(ref _lastinstall,value);
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Serializer.SerializeJsonObject(this);
        }
    }
}
