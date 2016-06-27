using OpenHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HueLib;
using HueLib_base;
using Microsoft.Win32;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class Form_AppSettings : Window
    {
        readonly Bridge _bridge;

        public Form_AppSettings(Bridge br)
        {
            InitializeComponent();
            _bridge = br;
            
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            WinHueSettings.settings.DetectProxy = (bool)chbDetectProxy.IsChecked;
            WinHueSettings.settings.EnableDebug = (bool)chbDebug.IsChecked;
            WinHueSettings.settings.LiveSliders = (bool)chbLiveSliders.IsChecked;
            WinHueSettings.settings.DelayLiveSliders = (int)nudSlidersDelay.Value;
            WinHueSettings.settings.ShowHiddenScenes = (bool)chbHiddenScenes.IsChecked;
            WinHueSettings.settings.UpnpTimeout = (int)nudUpnpTimeout.Value;

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

            if (WinHueSettings.settings.Language != (string) ((ComboBoxItem) cbLanguage.SelectedItem).Tag)
            {
                MessageBox.Show(GlobalStrings.Language_Change_Warning, GlobalStrings.Warning, MessageBoxButton.OK,MessageBoxImage.Information);
            }
            WinHueSettings.settings.Language = (string)((ComboBoxItem) cbLanguage.SelectedItem).Tag;
            WinHueSettings.Save();
            this.DialogResult = true;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chbDetectProxy.IsChecked = WinHueSettings.settings.DetectProxy;
            chbDebug.IsChecked = WinHueSettings.settings.EnableDebug;
            chbLiveSliders.IsChecked = WinHueSettings.settings.LiveSliders;
            nudSlidersDelay.Value = WinHueSettings.settings.DelayLiveSliders;
            chbHiddenScenes.IsChecked = WinHueSettings.settings.ShowHiddenScenes;
            nudUpnpTimeout.Value = WinHueSettings.settings.UpnpTimeout;
            chbStartWindows.IsChecked = WinHueSettings.settings.StartWithWindows;


            switch(WinHueSettings.settings.StartMode)
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

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
