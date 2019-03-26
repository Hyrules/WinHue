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
    [JsonObject]
    public class LightCapabilitiesControl
    {
        [Category("Light Capabilities Control"), Description("Minimum Dim Level")]
        public ushort mindimlevel { get; set; }
        [Category("Light Capabilities Control"), Description("Maximum Lumens")]
        public ushort maxlumen { get; set; }
        [Category("Light Capabilities Control"), Description("Color Gamut Type")]
        public string colorgamuttype { get; set; }
        [Category("Light Capabilities Control"), Description("Color Gamut")]
        public CoordinatesCollection colorgamut { get; set; }
        [Category("Light Capabilities Control"), Description("Color Temperature"),ExpandableObject]
        public LightControlCt ct {get;set;}

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
