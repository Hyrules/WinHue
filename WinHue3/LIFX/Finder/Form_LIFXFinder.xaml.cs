using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.LIFX.Framework;

namespace WinHue3.LIFX.Finder
{
    /// <summary>
    /// Interaction logic for Form_LIFXFinder.xaml
    /// </summary>
    public partial class Form_LIFXFinder : Window
    {
        LifxFinderViewModel _lfvm;

        public Form_LIFXFinder()
        {
            InitializeComponent();
            _lfvm = this.DataContext as LifxFinderViewModel;    

        }

        private void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            if(_lfvm.Devices.Count > 0)
            {
                ObservableCollection<LifxDevice> dev = _lfvm.Devices;
                foreach (LifxDevice l in dev)
                {
                    string mac = BitConverter.ToString(l.Mac.Reverse().ToArray(), 0, 6);
                    if (WinHueSettings.lifx.ListDevices.Any(x => x.mac == mac)) continue;
                    WinHueSettings.lifx.ListDevices.Add(new LifxSaveDevice(l.IP.ToString(), mac, l.Label));
                }

                WinHueSettings.SaveLifx();
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }

            this.Close();
        }
    }
}
