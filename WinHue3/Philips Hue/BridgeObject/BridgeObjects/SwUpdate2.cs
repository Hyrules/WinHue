using System.Runtime.Serialization;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [DataContract]
    public class SwUpdate2
    {
        [DataMember]
        public swbridge bridge { get; set; }
        [DataMember]
        public bool checkforupdate { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public bool install { get; set; }
        [DataMember]
        public autoinstall autoinstall { get; set; }
        [DataMember]
        public string lastchange { get; set; }
        [DataMember]
        public string lastinstall { get; set; }

    }

    [DataContract]
    public class autoinstall
    {
        [DataMember]
        public bool on { get; set; }
        [DataMember]
        public string updatetime { get; set; }
    }

    [DataContract]
    public class swbridge
    {
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string lastinstall { get; set; }
    }
}
