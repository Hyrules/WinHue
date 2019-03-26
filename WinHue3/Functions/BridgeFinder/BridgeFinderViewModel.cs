using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Utils;
using System.ComponentModel;
using System.Net;
using WinHue3.Functions.Application_Settings.Settings;

namespace WinHue3.Functions.BridgeFinder
{
    public class BridgeFinderViewModel : ValidatableBindableBase
    {
        private int _progress;
        private string _message;
        private Bridge _br;
        private bool _found;

        public BridgeFinderViewModel()
        {
            Progress = 0;
            BridgeFinder.ProgressReported += BridgeFinder_ProgressReported;
            BridgeFinder.ScanCompleted += BridgeFinder_ScanCompleted;
            Message = "";
            Found = false;
        }

        public void FindBridge(Bridge br)
        {
            _br = br;
            BridgeFinder.FindBridge(br);
            
        }

        public void CancelSearch()
        {
            BridgeFinder.CancelSearch();
        }

        private void BridgeFinder_ScanCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;
            
            if (e.Result != null)
            {
                IPAddress newip = (IPAddress) e.Result;
                Message = $"Bridge found : {newip}";
                Found = true;
                WinHueSettings.ReplaceBridgeIp(_br.Mac, newip);
            }
            else
            {
                Message = "Bridge was not found. Please try doing a pairing with the bridge pairing function.";
            }
            
        }

        private void BridgeFinder_ProgressReported(object sender, ProgressChangedEventArgs e)
        {
            ProgressReport pr = (ProgressReport) e.UserState;
            Progress = pr.Progress;
            Message = pr.Ip.ToString();
        }

        public int Progress
        {
            get => _progress; 
            set => SetProperty(ref _progress,value);
        }

        public string Message
        {
            get => _message; 
            set => SetProperty(ref _message, value);
        }

        public bool Found
        {
            get => _found; 
            set => SetProperty(ref _found,value);
        }
    }
}
