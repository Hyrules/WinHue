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
    [DataContract]
    public class LightConfig
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Config"), Description("Archetype"), ReadOnly(true)]
        public string archetype { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Config"), Description("Function"), ReadOnly(true)]
        public string function { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Config"), Description("Direction"), ReadOnly(true)]
        public string direction { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Config"), Description("Startup"), ReadOnly(true), ExpandableObject]
        public LightConfigStartup startup { get; set; }
        /// <summary>
        /// To string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }
    }
}
