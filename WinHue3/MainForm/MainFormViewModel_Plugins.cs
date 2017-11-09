namespace WinHue3.ViewModels.MainFormViewModels
{
    public partial class MainFormViewModel
    {
       
        private void RunCpuTempMon()
        {
            
            if (!_ctm.IsRunning)
                _ctm.Start();
            else
                _ctm.Stop();
            
        }

    }
}
