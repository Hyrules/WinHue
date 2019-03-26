using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    [JsonObject]
    public class LightConfig
    {
        [Category("Light Config"), Description("Archetype")]
        public string archetype { get; set; }
        [Category("Light Config"), Description("Function")]
        public string function { get; set; }
        [Category("Light Config"), Description("Direction")]
        public string direction { get; set; }
        [Category("Light Config"), Description("Startup"), ExpandableObject]
        public LightConfigStartup startup { get; set; }
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
