using System.Windows;
using Microsoft.Win32;
using WinHue3.Settings;
using WinHue3.ViewModels;

namespace WinHue3.Views
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class Form_AppSettings : Window
    {
 
        private AppSettingsViewModel _appSettingsViewModel;
        public Form_AppSettings()
        {
            InitializeComponent();  
            _appSettingsViewModel = DataContext as AppSettingsViewModel;    
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (WinHueSettings.settings.Language != _appSettingsViewModel.MainSettingsModel.Language)
            {
                MessageBox.Show(GlobalStrings.Language_Change_Warning, GlobalStrings.Warning, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            WinHueSettings.settings.DetectProxy = _appSettingsViewModel.MainSettingsModel.DetectProxy;
            WinHueSettings.settings.EnableDebug = _appSettingsViewModel.MainSettingsModel.Debug;
            WinHueSettings.settings.ShowHiddenScenes = _appSettingsViewModel.MainSettingsModel.ShowHidden;
            WinHueSettings.settings.UpnpTimeout = _appSettingsViewModel.MainSettingsModel.UpnpTimeout;
            WinHueSettings.settings.AllOffTT = _appSettingsViewModel.DefaultModel.AllOffTt;
            WinHueSettings.settings.AllOnTT = _appSettingsViewModel.DefaultModel.AllOnTt;
            WinHueSettings.settings.Timeout = _appSettingsViewModel.MainSettingsModel.Timeout;
            WinHueSettings.settings.DefaultTT = _appSettingsViewModel.DefaultModel.DefaultTt;
            WinHueSettings.settings.WrapText = _appSettingsViewModel.ViewSettingsModel.Wrap;
            WinHueSettings.settings.ShowID = _appSettingsViewModel.ViewSettingsModel.ShowId;
            WinHueSettings.settings.Sort = _appSettingsViewModel.ViewSettingsModel.Sort;
            WinHueSettings.settings.DefaultBriGroup = _appSettingsViewModel.DefaultModel.DefaultGroupBri;
            WinHueSettings.settings.DefaultBriLight = _appSettingsViewModel.DefaultModel.DefaultLightBri;
            WinHueSettings.settings.Language = _appSettingsViewModel.MainSettingsModel.Language;
            WinHueSettings.settings.StartMode = _appSettingsViewModel.MainSettingsModel.StartMode;
            WinHueSettings.settings.CheckForUpdate = _appSettingsViewModel.MainSettingsModel.CheckUpdate;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (WinHueSettings.settings.StartMode > 0)
            {
                registryKey.SetValue("WinHue3", System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            else
            {
                if(registryKey.GetValue("WinHue3") != null)
                    registryKey.DeleteValue("WinHue3");
            }
            registryKey.Close();
    
            WinHueSettings.Save();
            DialogResult = true;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _appSettingsViewModel.DefaultModel.AllOffTt = WinHueSettings.settings.AllOffTT;
            _appSettingsViewModel.DefaultModel.AllOnTt = WinHueSettings.settings.AllOnTT;
            _appSettingsViewModel.DefaultModel.DefaultTt = WinHueSettings.settings.DefaultTT;
            _appSettingsViewModel.ViewSettingsModel.Sort = WinHueSettings.settings.Sort;
            _appSettingsViewModel.ViewSettingsModel.ShowId = WinHueSettings.settings.ShowID;
            _appSettingsViewModel.ViewSettingsModel.Wrap = WinHueSettings.settings.WrapText;
            _appSettingsViewModel.DefaultModel.DefaultLightBri = WinHueSettings.settings.DefaultBriLight;
            _appSettingsViewModel.DefaultModel.DefaultGroupBri = WinHueSettings.settings.DefaultBriGroup;
            _appSettingsViewModel.MainSettingsModel.DetectProxy = WinHueSettings.settings.DetectProxy;
            _appSettingsViewModel.MainSettingsModel.Debug = WinHueSettings.settings.EnableDebug;
            _appSettingsViewModel.MainSettingsModel.ShowHidden = WinHueSettings.settings.ShowHiddenScenes;
            _appSettingsViewModel.MainSettingsModel.Timeout = WinHueSettings.settings.Timeout;
            _appSettingsViewModel.MainSettingsModel.UpnpTimeout = WinHueSettings.settings.UpnpTimeout;
            _appSettingsViewModel.MainSettingsModel.Language = WinHueSettings.settings.Language;
            _appSettingsViewModel.MainSettingsModel.StartMode = WinHueSettings.settings.StartMode;
            _appSettingsViewModel.MainSettingsModel.CheckUpdate = WinHueSettings.settings.CheckForUpdate;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
