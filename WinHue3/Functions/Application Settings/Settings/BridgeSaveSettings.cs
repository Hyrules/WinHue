using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WinHue3.Functions.Application_Settings.Settings
{

    [DataContract, Serializable]
    public class BridgeSaveSettings
    {
        public BridgeSaveSettings()
        {
            ip = string.Empty;
            apikey = string.Empty;
            hiddenobjects = new List<Tuple<string, Type>>();
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
        public List<Tuple<string,Type>> hiddenobjects { get;set; }
    }
}
