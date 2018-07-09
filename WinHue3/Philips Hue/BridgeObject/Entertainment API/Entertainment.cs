using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Philips_Hue.BridgeObject.Entertainment_API
{
    public class Entertainment
    {
        public string type { get; set; }
        public List<string> lights { get; set; }
        public string @class { get; set; }
    }
}
