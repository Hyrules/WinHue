using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.LIFX.Framework;

namespace WinHue3.Functions.Grouping
{
    public class LifxSubGroup
    {
        public string Name { get; set; }
        public List<LifxDevice> ListObjects {get;set;}
    }
}
