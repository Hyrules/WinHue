using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Functions.Grouping
{
    public class LifxGroup : IGroup
    {
        public string Name { get; set; }
        public ObservableCollection<LifxSubGroup> Groups { get; set; }
    }
}
