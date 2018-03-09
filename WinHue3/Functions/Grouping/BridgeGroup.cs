using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.Grouping
{
    public class BridgeGroup : IGroup
    {
        public string Name { get; set; }
        public Bridge Bridge { get; set; }
        public List<BridgeSubGroup> Groups { get; set; }
    }
}
