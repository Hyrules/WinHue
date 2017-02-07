using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Settings
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
