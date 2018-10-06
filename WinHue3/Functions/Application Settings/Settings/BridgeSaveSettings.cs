using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Functions.Application_Settings.Settings
{

    [DataContract, Serializable]
    public class BridgeSaveSettings
    {
        public BridgeSaveSettings()
        {
            ip = string.Empty;
            apikey = string.Empty;
            hiddenobjects = new List<Tuple<string, string>>();
        }

        [DataMember(IsRequired = false)]
        public string ip { get; set; }
        [DataMember(IsRequired = false)]
        public string apikey { get; set; }
        [DataMember(IsRequired = false)]
        public string name { get; set; }
        [DataMember(IsRequired = false)]
        public string clientkey { get; set; }
        [DataMember(IsRequired = false)]
        public List<Tuple<string,string>> hiddenobjects { get;set; }
    }
}
