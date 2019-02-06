using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.PowerSettings
{
    /// <summary>
    /// Interaction logic for Form_PowerCustomSettings.xaml
    /// </summary>
    public partial class Form_PowerCustomSettings : Window
    {

        private PowerCustomSettingsViewModel pfsvm;
        private string _id;

        public Form_PowerCustomSettings(PowerCustomSettings lightstate, string id)
        {
            InitializeComponent();
            _id = id;
            pfsvm = DataContext as PowerCustomSettingsViewModel;
            pfsvm.Customsettings = lightstate;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool result = await BridgeManager.SelectedBridge.SetPowerCustomSettingsAsyncTask(pfsvm.Customsettings, _id);
            if (result)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                BridgeManager.SelectedBridge.ShowErrorMessages();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
