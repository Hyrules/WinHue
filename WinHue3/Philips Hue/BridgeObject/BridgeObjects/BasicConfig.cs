using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [DataContract]
    public class BasicConfig
    {
        [HueProperty, DataMember]
        public string name { get; set; }
        [HueProperty, DataMember]
        public string datastoreversion { get; set; }
        [HueProperty, DataMember]
        public string swversion { get; set; }
        [HueProperty, DataMember]
        public string apiversion { get; set; }
        [HueProperty, DataMember]
        public string mac { get; set; }
        [HueProperty, DataMember]
        public string bridgeid { get; set; }
        [HueProperty, DataMember]
        public bool factorynew { get; set; }
        [HueProperty, DataMember]
        public string replacesbridgeid { get; set; }
        [HueProperty, DataMember]
        public string modelid { get; set; }
        [HueProperty, DataMember]
        public string starterkitid { get; set; }
    }
}
