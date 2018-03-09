using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Functions.Grouping
{
    public class BridgeSubGroup
    {
        string Name { get; set; }
        public ObservableCollection<IHueObject> ListObjects {get;set;}
    }
}
