using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [DataContract]
    public class InternetServices
    {
        [DataMember]
        public string internet { get; set; }

        [ DataMember]
        public string remoteaccess { get; set; }

        [ DataMember]
        public string time { get; set; }

        [ DataMember]
        public string swupdate { get; set; }
    }
}
