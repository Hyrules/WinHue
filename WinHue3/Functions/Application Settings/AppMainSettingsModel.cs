using WinHue3.Utils;

namespace WinHue3.Functions.Application_Settings
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
        private bool checkforbridgeupdate;

        public AppMainSettingsModel()
        {
            detectProxy = false;
            debug = true;
            showHidden = false;
            upnpTimeout = 5000;
            timeout = 3000;
            language = "en-US";
            startMode = 0;
            checkupdate = true;
            checkforbridgeupdate = true;
        }

        public bool StartWindows
        {
            get => startWindows;
            set => SetProperty(ref startWindows, value);
        }

        public bool CheckForBridgeUpdate
        {
            get => checkforbridgeupdate;
            set => SetProperty(ref checkforbridgeupdate, value);
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
