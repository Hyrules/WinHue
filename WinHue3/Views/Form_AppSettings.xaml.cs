using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HueLib2;
using Microsoft.Win32;
using WinHue3.Models;
using WinHue3.ViewModels;

namespace WinHue3
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
       //     WinHueSettings.settings.DetectProxy = (bool)chbDetectProxy.IsChecked;
      //      WinHueSettings.settings.EnableDebug = (bool)chbDebug.IsChecked;
       //     WinHueSettings.settings.LiveSliders = (bool)chbLiveSliders.IsChecked;
        //    WinHueSettings.settings.DelayLiveSliders = (int)nudSlidersDelay.Value;
        //    WinHueSettings.settings.ShowHiddenScenes = (bool)chbHiddenScenes.IsChecked;
         //   WinHueSettings.settings.UpnpTimeout = (int)nudUpnpTimeout.Value;
            WinHueSettings.settings.AllOffTT = _appSettingsViewModel.DefaultModel.AllOffTt;
            WinHueSettings.settings.AllOnTT = _appSettingsViewModel.DefaultModel.AllOnTt;
         //   WinHueSettings.settings.Timeout = (int)nudTimeout.Value;
            WinHueSettings.settings.DefaultTT = _appSettingsViewModel.DefaultModel.DefaultTt;
            WinHueSettings.settings.WrapText = _appSettingsViewModel.ViewSettingsModel.Wrap;
            WinHueSettings.settings.ShowID = _appSettingsViewModel.ViewSettingsModel.ShowId;
            WinHueSettings.settings.Sort = _appSettingsViewModel.ViewSettingsModel.Sort;
            WinHueSettings.settings.DefaultBriGroup = _appSettingsViewModel.DefaultModel.DefaultGroupBri;
            WinHueSettings.settings.DefaultBriLight = _appSettingsViewModel.DefaultModel.DefaultLightBri;

            if (rbStartNormal.IsChecked == true)
            {
                WinHueSettings.settings.StartMode = 0;
            }
            else if (rbStartInTray.IsChecked == true)
            {
                WinHueSettings.settings.StartMode = 1;
            }
            else
            {
                WinHueSettings.settings.StartMode = 2;
            }

            WinHueSettings.settings.StartWithWindows = (bool)chbStartWindows.IsChecked;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (chbStartWindows.IsChecked == true)
            {
                registryKey.SetValue("WinHue3", System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            else
            {
                if(registryKey.GetValue("WinHue3") != null)
                    registryKey.DeleteValue("WinHue3");
            }
            registryKey.Close();

            /*if (WinHueSettings.settings.Language != (string) ((ComboBoxItem) cbLanguage.SelectedItem).Tag)
            {
                MessageBox.Show(GlobalStrings.Language_Change_Warning, GlobalStrings.Warning, MessageBoxButton.OK,MessageBoxImage.Information);
            }
           
            WinHueSettings.settings.Language = (string)((ComboBoxItem) cbLanguage.SelectedItem).Tag;*/
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
/*          




            switch (WinHueSettings.settings.StartMode)
            {
                case 0:
                    rbStartNormal.IsChecked = true;
                    break;
                case 1:
                    rbStartInTray.IsChecked = true;
                    break;
                case 2:
                    rbStartMinimized.IsChecked = true;
                    break;
                default:
                    rbStartNormal.IsChecked = true;
                    break;
            }

            foreach (ComboBoxItem c in from ComboBoxItem c in cbLanguage.Items where (string) c.Tag == WinHueSettings.settings.Language select c)
            {
                cbLanguage.SelectedIndex = cbLanguage.Items.IndexOf(c);
            }
            */
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
