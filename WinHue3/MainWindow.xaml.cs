using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Settings;
using WinHue3.Utils;
using WinHue3.ViewModels;
using WinHue3.ViewModels.MainFormViewModels;
using IHueObject = WinHue3.Philips_Hue.HueObjects.Common.IHueObject;

namespace WinHue3
{
    /// <summary>
    /// Main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);     
        public Views.Form_EventLog _fel;
        private MainFormViewModel _mfvm;

        /// <summary>
        /// Form of the Eventlog.
        /// </summary>

        public MainWindow(Views.Form_EventLog formEventLog)
        {
            _fel = formEventLog;

            if (!string.IsNullOrEmpty(WinHueSettings.settings.Language) &&
                !string.IsNullOrWhiteSpace(WinHueSettings.settings.Language))
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(WinHueSettings.settings.Language);

            InitializeComponent();
            _mfvm = DataContext as MainFormViewModel;
            _mfvm.Eventlogform = _fel;
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version; 
            Hue.DetectLocalProxy = WinHueSettings.settings.DetectProxy;
             trayicon.Icon = Properties.Resources.icon;

        }

        private void lvMainObjects_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_mfvm.SelectedObject == null) e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            log.Info("WinHue Closing.");
            Application.Current.Shutdown();
        }

        public void SetLightBackground(List<string> lightlist)
        {
            // Set the new background color.
            if (lightlist.Count <= 0) return;
            foreach (ListViewItem dependencyobject in (from item in lvMainObjects.Items.OfType<Light>()
                where lightlist.Contains(item.Id)
                select this.lvMainObjects.ItemContainerGenerator.ContainerFromItem(item)).OfType<ListViewItem>())
            {
                dependencyobject.Background = new SolidColorBrush();
                ((SolidColorBrush) dependencyobject.Background).Color =
                    System.Windows.Media.Color.FromArgb(20, 0, 200, 0);
            }
        }

        public void SetObjectBackground(List<IHueObject> objectlist)
        {
            if (objectlist.Count <= 0) return;
            foreach (ListViewItem dependencyobject in (from item in lvMainObjects.Items.OfType<IHueObject>()
                                                       where objectlist.Contains(item)
                                                       select this.lvMainObjects.ItemContainerGenerator.ContainerFromItem(item)).OfType<ListViewItem>())
            {
                dependencyobject.Background = new SolidColorBrush();
                ((SolidColorBrush)dependencyobject.Background).Color =
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
                dependencyobject.Background = null;
            }
        }

        private void lvMainObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            log.Debug("Clearing light bg color.");
            ClearBackgroundColor();
            if (lvMainObjects.SelectedItem == null) return;
            if (lvMainObjects.SelectedItem is Resourcelink)
            {
                Resourcelink rl = (Resourcelink) lvMainObjects.SelectedItem;
                List<IHueObject> listhue = new List<IHueObject>();
                List<IHueObject> bo = new List<IHueObject>(lvMainObjects.Items.OfType<IHueObject>());
                foreach (string s in rl.links)
                {
                    string[] objbreak = s.Split('/');
                    string classname = objbreak[1].TrimEnd('s');
                    classname = "HueLib2." + classname.First().ToString().ToUpper() + string.Join("", classname.Skip(1));
                    Type objtype = Type.GetType(classname + ", HueLib2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                    listhue.Add(bo.Find(x => x.Id == objbreak[2] && x.GetType() == objtype));
                }
                SetObjectBackground(listhue);
                return;
            }

            if (!lvMainObjects.SelectedItem.HasProperty("lights")) return;
            StringCollection list =
                (StringCollection)
                    lvMainObjects.SelectedItem.GetType().GetProperty("lights").GetValue(lvMainObjects.SelectedItem);
            log.Debug("Settings light BG color for lights : " + string.Join(",",list));
            SetLightBackground(list);
        }

        private void trayicon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = this.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }


    }
}
