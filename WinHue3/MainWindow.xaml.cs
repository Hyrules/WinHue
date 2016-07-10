using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using HueLib;
using HueLib_base;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.Core.Converters;
using Hardcodet.Wpf.TaskbarNotification;

namespace WinHue3
{
    /// <summary>
    /// Main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Version { get; set; }
        public Form_EventLog _fel;
        private MainFormView mfv;

        /// <summary>
        /// Form of the Eventlog.
        /// </summary>

        public MainWindow(Form_EventLog formEventLog)
        {
            _fel = formEventLog;


            if (!string.IsNullOrEmpty(WinHueSettings.settings.Language) &&
                !string.IsNullOrWhiteSpace(WinHueSettings.settings.Language))
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(WinHueSettings.settings.Language);

            InitializeComponent();
            Title += " " + Version;
            Hue.DetectLocalProxy = WinHueSettings.settings.DetectProxy;
             trayicon.Icon = Properties.Resources.icon;


        }

        private void lvMainObjects_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (mfv.SelectedObject == null) e.Handled = true;
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void btnSupportForum_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://sourceforge.net/p/winhue/discussion/support/");
        }

        private void btnWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://sourceforge.net/projects/winhue/");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            log.Info("WinHue Closing.");
            Application.Current.Shutdown();
        }


        private void Window_ContentRendered(object sender, EventArgs e)
        {
            mfv = new MainFormView(_fel);
            DataContext = mfv;
            _fel.Owner = this;
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /* if (_bridge?.IpAddress == null || _bridge?.ApiKey == null) return;
                 _stateRefresher.Stop();*/
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /*  if (_bridge?.IpAddress == null || _bridge?.ApiKey == null) return;
                  _stateRefresher.Start();*/
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Form_AppSettings settings = new Form_AppSettings(mfv.SelectedBridge) {Owner = this};
            if (settings.ShowDialog() != true) return;
        }

        public void SetLightBackground(List<string> lightlist)
        {
            // Set the new background color.
            if (lightlist.Count <= 0) return;
            foreach (ListViewItem dependencyobject in (from item in lvMainObjects.Items.OfType<Light>()
                where lightlist.Contains((item as Light).Id.ToString())
                select this.lvMainObjects.ItemContainerGenerator.ContainerFromItem(item)).OfType<ListViewItem>())
            {
                ((ListViewItem) dependencyobject).Background = new SolidColorBrush();
                ((SolidColorBrush) ((ListViewItem) dependencyobject).Background).Color =
                    System.Windows.Media.Color.FromArgb(20, 0, 200, 0);
            }
        }

        public void ClearBackgroundColor()
        {
            // Clear any background color
            if (lvMainObjects.Items.Count == 0) return;
            foreach (
                ListViewItem dependencyobject in
                    (from object item in lvMainObjects.Items
                        select this.lvMainObjects.ItemContainerGenerator.ContainerFromItem(item)).OfType<ListViewItem>()
                )
            {
                ((ListViewItem) dependencyobject).Background = null;
            }
        }

        private void lvMainObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            log.Debug("Clearing light bg color.");
            ClearBackgroundColor();
            if (lvMainObjects.SelectedItem == null) return;
            if (!lvMainObjects.SelectedItem.HasProperty("lights")) return;
            List<string> list =
                (List<string>)
                    lvMainObjects.SelectedItem.GetType().GetProperty("lights").GetValue(lvMainObjects.SelectedItem);
            log.Debug("Settings light BG color for lights : " + list);
            SetLightBackground(list);
        }

        private void trayicon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = this.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }

        private void lvMainObjects_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            mfv?.KeyPress(e.Key);
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            mfv?.HandleHotkey(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

 
    }
}
