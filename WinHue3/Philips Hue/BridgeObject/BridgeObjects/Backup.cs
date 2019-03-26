using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [DataContract]
    public class Backup
    {
        [DataMember]
        public string status { get; set; }

        [DataMember]
        public int errorcode { get; set; }
    }
}
