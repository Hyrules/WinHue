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
            ObservableCollection<LifxDevice> dev = _lfvm.Devices;
            foreach (LifxDevice l in dev)
            {
                if (WinHueSettings.lifx.ListDevices.Any(x => x.mac == l.Mac)) continue;
                WinHueSettings.lifx.ListDevices.Add(new LifxSaveDevice(l.IP.ToString(), l.Mac,l.Label));
            }
            
            WinHueSettings.SaveLifx();
            this.Close();
        }
    }
}
