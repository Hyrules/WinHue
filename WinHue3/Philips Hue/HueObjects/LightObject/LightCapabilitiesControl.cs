using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinHue3.Functions.PropertyGrid;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    [DataContract]
    public class LightCapabilitiesControl
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities Control"), Description("Minimum Dim Level"), ReadOnly(true)]
        public ushort mindimlevel { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities Control"), Description("Maximum Lumens"), ReadOnly(true)]
        public ushort maxlumen { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities Control"), Description("Color Gamut Type"), ReadOnly(true)]
        public string colorgamuttype { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities Control"), Description("Color Gamut"), ReadOnly(true)]
        public CoordinatesCollection colorgamut { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities Control"), Description("Color Temperature"), ReadOnly(true), ExpandableObject]
        public LightControlCt ct {get;set;}

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
