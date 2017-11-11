using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WinHue3.Functions.Application_Settings.Settings
{
    public class CustomBridges
    {
        public CustomBridges()
        {
            BridgeInfo = new Dictionary<string, BridgeSaveSettings>();
            DefaultBridge = string.Empty;
        }

        [DataMember]
        public Dictionary<string, BridgeSaveSettings> BridgeInfo { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string DefaultBridge { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
