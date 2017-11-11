using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Functions.HotKeys;

namespace WinHue3.Functions.Application_Settings.Settings
{
    public class CustomHotkeys
    {
        public CustomHotkeys()
        {
            listHotKeys = new List<HotKey>();
        }

        [DataMember(EmitDefaultValue = true)]
        public List<HotKey> listHotKeys { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
