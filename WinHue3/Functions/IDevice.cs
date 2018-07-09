using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WinHue3.Functions
{
    public interface IDevice 
    {
        string Id { get; set; }
        ImageSource Image { get; set; }
        string name { get; set; }
        object Clone();
        bool visible { get; set; }
 

    }
}
