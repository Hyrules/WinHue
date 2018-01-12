using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX;
using WinHue3.LIFX.Framework;

namespace WinHue3.Functions.Application_Settings.Settings
{
    public class CustomLifxSettings
    {
        [DataMember(EmitDefaultValue = true)]
        public List<LifxDevice> ListDevices { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public int SendTimeout;

        [DataMember(EmitDefaultValue = true)]
        public int RecvTimeout;

        [DataMember(EmitDefaultValue = true)]
        public int FindTimeout;

        public CustomLifxSettings()
        {
            ListDevices = new List<LifxDevice>();
            SendTimeout = 3000;
            RecvTimeout = 3000;
            FindTimeout = 3000;
        }

    }
}
