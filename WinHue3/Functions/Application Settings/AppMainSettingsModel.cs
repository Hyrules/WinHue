using WinHue3.Functions.Application_Settings.Settings;
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
        private string theme;
        private string themecolor;

        public AppMainSettingsModel()
        {
            detectProxy = WinHueSettings.settings.DetectProxy;
            debug = WinHueSettings.settings.EnableDebug;
            showHidden = WinHueSettings.settings.ShowHiddenScenes;
            upnpTimeout = WinHueSettings.settings.UpnpTimeout;
            timeout = WinHueSettings.settings.Timeout;
            language = WinHueSettings.settings.Language;
            startMode = WinHueSettings.settings.StartMode;
            checkupdate = WinHueSettings.settings.CheckForUpdate;
            checkforbridgeupdate = WinHueSettings.settings.CheckForBridgeUpdate;
            theme = WinHueSettings.settings.Theme;
            themecolor = WinHueSettings.settings.ThemeColor;
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

        public string Theme
        {
            get => theme;
            set => SetProperty(ref theme, value);
        }

        public string Themecolor
        {
            get => themecolor;
            set => SetProperty(ref themecolor,value);
        }
    }
}
