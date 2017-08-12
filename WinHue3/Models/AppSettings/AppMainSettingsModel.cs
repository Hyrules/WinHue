using WinHue3.ViewModels;

namespace WinHue3.Models.AppSettings
{
    public class AppMainSettingsModel : ValidatableBindableBase
    {
        private bool detectProxy;
        private bool debug;
        private bool showHidden;
        private int upnpTimeout;
        private int timeout;
        private bool startWindows;
        private string language;
        private int startMode;
        private bool checkupdate;

        public AppMainSettingsModel()
        {
            DetectProxy = false;
            Debug = true;
            ShowHidden = false;
            upnpTimeout = 5000;
            Timeout = 3000;
            Language = "en-US";
            StartMode = 0;
            CheckUpdate = true;

        }

        public bool DetectProxy
        {
            get => detectProxy;
            set => SetProperty(ref detectProxy,value);
        }

        public bool Debug
        {
            get => debug;
            set => SetProperty(ref debug,value);
        }

        public bool ShowHidden
        {
            get => showHidden;
            set => SetProperty(ref showHidden,value);
        }

        public int UpnpTimeout
        {
            get => upnpTimeout;
            set => SetProperty(ref upnpTimeout,value);
        }

        public int Timeout
        {
            get => timeout;
            set => SetProperty(ref timeout,value);
        }

        public string Language
        {
            get => language;
            set => SetProperty(ref language,value);
        }

        public int StartMode
        {
            get => startMode;
            set => SetProperty(ref startMode,value);
        }

        public bool CheckUpdate
        {
            get => checkupdate;
            set => SetProperty(ref checkupdate,value);
        }
    }
}
