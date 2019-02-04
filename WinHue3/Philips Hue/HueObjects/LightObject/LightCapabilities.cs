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
    public class LightCapabilities
    {
        [Category("Light Capabilities"), Description("Certified")]
        public bool certified { get; set; }
        [Category("Light Capabilities"), Description("Control"),ExpandableObject]
        public LightCapabilitiesControl control { get; set; }
        [Category("Light Capabilities"), Description("Streaming"),ExpandableObject]
        public LightCapabilitiesStreaming streaming { get;set; }

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
