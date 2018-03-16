using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WinHue3.Utils;

namespace WinHue3.Functions.LIFX.WinHue
{
    public interface ILifxObject
    {
        string Name { get; set; }
        ImageSource Image { get ; set; }

    }
}
