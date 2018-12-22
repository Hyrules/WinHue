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

namespace WinHue3.Functions.PowerSettings
{
    /// <summary>
    /// Interaction logic for Form_PowerCustomSettings.xaml
    /// </summary>
    public partial class Form_PowerCustomSettings : Window
    {

        private PowerCustomSettingsViewModel pfsvm;
        private Bridge _bridge;
        private string _id;

        public Form_PowerCustomSettings(Bridge bridge, PowerCustomSettings lightstate, string id)
        {
            InitializeComponent();
            _bridge = bridge;
            _id = id;
            pfsvm = DataContext as PowerCustomSettingsViewModel;
            pfsvm.Customsettings = lightstate;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool result = await _bridge.SetPowerCustomSettingsAsyncTask(pfsvm.Customsettings, _id);
            if (result)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                _bridge.ShowErrorMessages();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
