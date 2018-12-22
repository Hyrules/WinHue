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
    public class LightCapabilities
    {
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities"), Description("Certified"), ReadOnly(true)]
        public bool certified { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities"), Description("Control"), ReadOnly(true),ExpandableObject]
        public LightCapabilitiesControl control { get; set; }
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Capabilities"), Description("Streaming"), ReadOnly(true),ExpandableObject]
        public LightCapabilitiesStreaming streaming { get;set; }

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
