using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;

namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    [DataContract]
    public class LightCapabilitiesStreaming
    {
        [DataMember, Category("Light Capabilities Streaming"), Description("Renderer"), ReadOnly(true)]
        public bool renderer { get; set; }
        [DataMember, Category("Light Capabilities Streaming"), Description("Proxy"), ReadOnly(true)]
        public bool proxy { get; set; }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

            //return $"{{\"renderer\":{renderer},\"proxy\":{proxy}}}";
        }
    }
}
