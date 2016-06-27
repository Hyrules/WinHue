using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Ribbon;
using WinHuePluginModule;

namespace WinHue3.Classes
{
    public class RibbonSplitButtonPlugin : RibbonSplitButton
    {
        public IWinHuePluginModule Plugin { get; set; }

    }
}
