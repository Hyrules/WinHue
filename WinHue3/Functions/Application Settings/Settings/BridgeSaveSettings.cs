using System;
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
        }

        [DataMember(IsRequired = false)]
        public string ip { get; set; }
        [DataMember(IsRequired = false)]
        public string apikey { get; set; }
        [DataMember(IsRequired = false)]
        public string apiversion { get; set; }
        [DataMember(IsRequired = false)]
        public string swversion { get; set; }
        [DataMember(IsRequired = false)]
        public string name { get; set; }
    }
}
