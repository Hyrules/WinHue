using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.ViewModels
{
    public partial class MainFormViewModel
    {
       
        private void RunCpuTempMon()
        {
            _ctm = new CpuTempMonitor(SelectedBridge);
            if (!_ctm.IsRunning)
                _ctm.Start();
            else
                _ctm.Stop();
            
        }

    }
}
