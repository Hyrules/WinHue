using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Annotations;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    [JsonObject]
    public class LightConfigStartup : ValidatableBindableBase
    {
        private string _mode;
        private bool _configured;
        private PowerCustomSettings _customsettings;

        [Category("Light Config Startup"), Description("Mode")]
        public string mode
        {
            get => _mode; 
            set => SetProperty(ref _mode,value);
        }

        [Category("Light Config Startup"), Description("Configured")]
        public bool configured
        {
            get => _configured;
            set => SetProperty(ref _configured, value);
        }
        
        [Category("Light Config Startup"), Description("Custom Settings"), ExpandableObject]
        public PowerCustomSettings customsettings
        {
            get => _customsettings;
            set => SetProperty(ref _customsettings, value);
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
