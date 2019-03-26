using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Utils;

namespace WinHue3.Functions.Application_Settings
{
    public class  AppMainSettingsModel : ValidatableBindableBase
    {
        private bool _detectProxy;
        private bool _debug;
        private bool _showHidden;
        private int _upnpTimeout;
        private int _timeout;
        private bool _startWindows;
        private string _language;
        private int _startMode;
        private bool _checkupdate;
        private bool _checkforbridgeupdate;
        private string _theme;
        private string _themecolor;
        private bool _minimizeToTray;
        private bool _usePropGrid;
        private bool _OSRAMFix;
        private double _refreshTime;

        public AppMainSettingsModel()
        {
            _detectProxy = WinHueSettings.settings.DetectProxy;
            _debug = WinHueSettings.settings.EnableDebug;
            _showHidden = WinHueSettings.settings.ShowHiddenScenes;
            _upnpTimeout = WinHueSettings.settings.UpnpTimeout;
            _timeout = WinHueSettings.settings.Timeout;
            _language = WinHueSettings.settings.Language;
            _startMode = WinHueSettings.settings.StartMode;
            _checkupdate = WinHueSettings.settings.CheckForUpdate;
            _checkforbridgeupdate = WinHueSettings.settings.CheckForBridgeUpdate;
            _theme = WinHueSettings.settings.Theme;
            _themecolor = WinHueSettings.settings.ThemeColor;
            _minimizeToTray = WinHueSettings.settings.MinimizeToTray;
            _usePropGrid = WinHueSettings.settings.UsePropertyGrid;
            _OSRAMFix = WinHueSettings.settings.OSRAMFix;
            RefreshTime = WinHueSettings.settings.RefreshTime;
        }

        public bool StartWindows
        {
            get => _startWindows;
            set => SetProperty(ref _startWindows, value);
        }

        public bool CheckForBridgeUpdate
        {
            get => _checkforbridgeupdate;
            set => SetProperty(ref _checkforbridgeupdate, value);
        }

        public bool DetectProxy
        {
            get => _detectProxy;
            set => SetProperty(ref _detectProxy,value);
        }

        public bool Debug
        {
            get => _debug;
            set => SetProperty(ref _debug,value);
        }

        public bool ShowHidden
        {
            get => _showHidden;
            set => SetProperty(ref _showHidden,value);
        }

        public int UpnpTimeout
        {
            get => _upnpTimeout;
            set => SetProperty(ref _upnpTimeout,value);
        }

        public int Timeout
        {
            get => _timeout;
            set => SetProperty(ref _timeout,value);
        }

        public string Language
        {
            get => _language;
            set => SetProperty(ref _language,value);
        }

        public int StartMode
        {
            get => _startMode;
            set => SetProperty(ref _startMode,value);
        }

        public bool CheckUpdate
        {
            get => _checkupdate;
            set => SetProperty(ref _checkupdate,value);
        }

        public string Theme
        {
            get => _theme;
            set => SetProperty(ref _theme, value);
        }

        public string Themecolor
        {
            get => _themecolor;
            set => SetProperty(ref _themecolor,value);
        }

        public bool MinimizeToTray
        {
            get => _minimizeToTray;
            set => SetProperty(ref _minimizeToTray,value);
        }

        public bool UsePropGrid
        {
            get => _usePropGrid;
            set => SetProperty(ref _usePropGrid,value);
        }

        public bool OSRAMFix
        {
            get => _OSRAMFix; 
            set => SetProperty(ref _OSRAMFix,value);
        }

        public double RefreshTime
        {
            get => _refreshTime;
            set => SetProperty(ref _refreshTime, value);
        }
    }
}
