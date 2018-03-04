using System.Runtime.Serialization;

namespace WinHue3.Functions.Application_Settings.Settings
{
    [DataContract]
    public class LifxSaveDevice
    {

        public LifxSaveDevice()
        {
            ipaddress = "";
            mac = "";
            name = "";
        }

        public LifxSaveDevice(string ip, string macaddress, string devname)
        {
            ipaddress = ip;
            mac = macaddress;
            name = devname;
        }

        [DataMember(EmitDefaultValue = true)]
        public string ipaddress { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string mac { get; set; }
        [DataMember(EmitDefaultValue = true)]
        public string name { get; set; }

        
    }
}
