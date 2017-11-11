namespace WinHue3.MainForm
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
