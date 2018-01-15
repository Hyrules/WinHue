using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WinHue3.Functions.Application_Settings.Settings
{
    public class CustomLifxSettings
    {
        [DataMember(EmitDefaultValue = true)]
        public List<LifxSaveDevice> ListDevices { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public int SendTimeout;

        [DataMember(EmitDefaultValue = true)]
        public int RecvTimeout;

        [DataMember(EmitDefaultValue = true)]
        public int FindTimeout;

        public CustomLifxSettings()
        {
            ListDevices = new List<LifxSaveDevice>();
            SendTimeout = 3000;
            RecvTimeout = 3000;
            FindTimeout = 3000;
        }

    }
}
