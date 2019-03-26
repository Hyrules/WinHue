using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Utils;
using IHueObject = WinHue3.Philips_Hue.HueObjects.Common.IHueObject;

namespace WinHue3.MainForm
{
    /// <summary>
    /// Main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private MainFormViewModel _mfvm;

        /// <summary>
        /// Form of the Eventlog.
        /// </summary>

        public MainWindow()
        {
        
            InitializeComponent();
            _mfvm = DataContext as MainFormViewModel;

            Hue.DetectLocalProxy = WinHueSettings.settings.DetectProxy;
             Trayicon.Icon = Properties.Resources.icon;
            _mfvm.SetToolbarTray(Trayicon);
            
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
            foreach (ListViewItem dependencyobject in (from item in LvMainObjects.Items.OfType<Light>()
                where lightlist.Contains(item.Id)
                select this.LvMainObjects.ItemContainerGenerator.ContainerFromItem(item)).OfType<ListViewItem>())
            {
                dependencyobject.Background = new SolidColorBrush();
                ((SolidColorBrush) dependencyobject.Background).Color =
                    System.Windows.Media.Color.FromArgb(20, 0, 200, 0);
            }
        }

        public void SetObjectBackground(List<IHueObject> objectlist)
        {
            if (objectlist.Count <= 0) return;
            foreach (ListViewItem dependencyobject in (from item in LvMainObjects.Items.OfType<IHueObject>()
                                                       where objectlist.Contains(item)
                                                       select this.LvMainObjects.ItemContainerGenerator.ContainerFromItem(item)).OfType<ListViewItem>())
            {
                dependencyobject.Background = new SolidColorBrush();
                ((SolidColorBrush)dependencyobject.Background).Color =
                    System.Windows.Media.Color.FromArgb(20, 0, 200, 0);
            }
        }

        public void ClearBackgroundColor()
        {
            // Clear any background color
            if (LvMainObjects.Items.Count == 0) return;
            foreach (
                ListViewItem dependencyobject in
                    (from object item in LvMainObjects.Items
                        select this.LvMainObjects.ItemContainerGenerator.ContainerFromItem(item)).OfType<ListViewItem>()
                )
            {
                dependencyobject.Background = null;
            }
        }

        private void LvMainObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            log.Debug("Clearing light bg color.");
            ClearBackgroundColor();
            if (LvMainObjects.SelectedItem == null) return;
            if (LvMainObjects.SelectedItem is Resourcelink rl)
            {
                rl = (Resourcelink) LvMainObjects.SelectedItem;
                List<IHueObject> listhue = new List<IHueObject>();
                List<IHueObject> bo = new List<IHueObject>(LvMainObjects.Items.OfType<IHueObject>());
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

            if (!LvMainObjects.SelectedItem.HasProperty("lights")) return;
            StringCollection list =
                (StringCollection)
                    LvMainObjects.SelectedItem.GetType().GetProperty("lights").GetValue(LvMainObjects.SelectedItem);
            log.Debug("Settings light BG color for lights : " + string.Join(",",list));
            SetLightBackground(list);
        }

        private void Trayicon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Visibility = this.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;

            
        }

        private void MainForm_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized) return;
            if (!WinHueSettings.settings.MinimizeToTray) return;
            this.WindowState = WindowState.Normal;    
            Application.Current.MainWindow.Hide();


        }

    }
}
