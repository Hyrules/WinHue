using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [DataContract]
    public class BasicConfig
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string datastoreversion { get; set; }
        [DataMember]
        public string swversion { get; set; }
        [DataMember]
        public string apiversion { get; set; }
        [DataMember]
        public string mac { get; set; }
        [DataMember]
        public string bridgeid { get; set; }
        [DataMember]
        public bool factorynew { get; set; }
        [DataMember]
        public string replacesbridgeid { get; set; }
        [DataMember]
        public string modelid { get; set; }
        [DataMember]
        public string starterkitid { get; set; }
    }
}
