using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [DataContract]
    public class SwUpdate2
    {
        [HueProperty, DataMember]
        public swbridge bridge { get; set; }
        [HueProperty, DataMember]
        public bool checkforupdate { get; set; }
        [HueProperty, DataMember]
        public string state { get; set; }
        [HueProperty, DataMember]
        public bool install { get; set; }
        [HueProperty, DataMember]
        public autoinstall autoinstall { get; set; }
        [HueProperty, DataMember]
        public string lastchange { get; set; }
        [HueProperty, DataMember]
        public string lastinstall { get; set; }

    }

    [DataContract]
    public class autoinstall
    {
        [HueProperty, DataMember]
        public bool on { get; set; }
        [HueProperty, DataMember]
        public string updatetime { get; set; }
    }

    [DataContract]
    public class swbridge
    {
        [HueProperty, DataMember]
        public string state { get; set; }
        [HueProperty, DataMember]
        public string lastinstall { get; set; }
    }
}
