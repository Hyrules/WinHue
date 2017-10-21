using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinHue3.Hotkeys;

namespace WinHue3.Settings
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
