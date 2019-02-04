using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    [JsonObject]
    public class SwUpdate2
    {

        public swbridge bridge { get; set; }

        public bool checkforupdate { get; set; }

        public string state { get; set; }

        public bool install { get; set; }

        public autoinstall autoinstall { get; set; }

        public string lastchange { get; set; }

        public string lastinstall { get; set; }

    }

    [JsonObject]
    public class autoinstall
    {

        public bool on { get; set; }

        public string updatetime { get; set; }
    }

    [JsonObject]
    public class swbridge
    {

        public string state { get; set; }

        public string lastinstall { get; set; }
    }
}
