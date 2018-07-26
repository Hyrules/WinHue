using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using IWshRuntimeLibrary;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Theming;
using File = System.IO.File;

namespace WinHue3.Functions.Application_Settings
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class Form_AppSettings
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
            WinHueSettings.settings.CheckForBridgeUpdate = _appSettingsViewModel.MainSettingsModel.CheckForBridgeUpdate;
            WinHueSettings.settings.UseLastBriState = _appSettingsViewModel.DefaultModel.UseLastBriState;
            WinHueSettings.settings.MinimizeToTray = _appSettingsViewModel.MainSettingsModel.MinimizeToTray;
            WinHueSettings.settings.UsePropertyGrid = _appSettingsViewModel.MainSettingsModel.UsePropGrid;
            WinHueSettings.settings.SlidersBehavior = _appSettingsViewModel.DefaultModel.SlidersBehavior;
            WinHueSettings.settings.ThemeLayout = _appSettingsViewModel.MainSettingsModel.ThemeLayout;
            WinHueSettings.settings.Theme = _appSettingsViewModel.MainSettingsModel.Theme;
            WinHueSettings.settings.UseWindowsAccent = _appSettingsViewModel.MainSettingsModel.UseWindowsAccent;

            string pathtostartupfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "WinHue3.lnk");

            if (WinHueSettings.settings.StartMode > 0)
            {
                
                if (!File.Exists(pathtostartupfile))
                {
                 
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(pathtostartupfile);
                    shortcut.Description = "WinHue Startup";
                    shortcut.TargetPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    shortcut.Save();
                }

            }
            else
            {
                if (File.Exists(pathtostartupfile))
                {
                    File.Delete(pathtostartupfile);
                }

            }
            //registryKey.Close();

            WinHueSettings.SaveSettings();

            try
            {
                ThemeEngine.ChangeAppTheme(WinHueSettings.settings.Theme, WinHueSettings.settings.ThemeLayout);
                ThemeEngine.ChangeAppAccent(WinHueSettings.settings.ThemeColor);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Issue applying theme: " + ex.Message);
            }

            DialogResult = true;
            Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            
           // Fluent.ThemeManager.ChangeAppStyle(Application.Current, Fluent.ThemeManager.GetAccent(WinHueSettings.settings.ThemeColor), Fluent.ThemeManager.GetAppTheme(WinHueSettings.settings.Theme));
            
            Close();
        }

        private void btnAccent_Click(object sender, RoutedEventArgs e)
        {
            //Work in Progress

            var curColor = WinHueSettings.settings.ThemeColor;
            System.Drawing.Color _color = System.Drawing.Color.FromArgb(curColor.A, curColor.R, curColor.G, curColor.B);
            var cp = new System.Windows.Forms.ColorDialog();
            cp.Color = _color;
            cp.ShowDialog();
            System.Windows.Media.Color newAccent = System.Windows.Media.Color.FromArgb(cp.Color.A, cp.Color.R, cp.Color.G, cp.Color.B);
            WinHueSettings.settings.ThemeColor = newAccent;

        }

        private void cbThemeLayout_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((e.AddedItems[0] as System.Windows.Controls.ComboBoxItem).Content.ToString() == "Legacy")
            {
                //MessageBox.Show("legacy");
                //cbTheme.IsEnabled = false;
                //buttonAccent.IsEnabled = false;
                //chkWindowsAccent.IsEnabled = false;
            } else
            {
                //cbTheme.IsEnabled = true;
                //buttonAccent.IsEnabled = true;
                //chkWindowsAccent.IsEnabled = true;
            }
        }
    }
    public class CustomAccentEnabler : System.Windows.Data.IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)values[0] == false && ((System.Windows.Controls.ComboBoxItem)values[1]).ToString().Contains("Modern"))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack should never be called");
        }

    }
}
