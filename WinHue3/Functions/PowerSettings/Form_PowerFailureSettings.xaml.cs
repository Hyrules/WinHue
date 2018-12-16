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

namespace WinHue3.Functions.PowerSettings
{
    /// <summary>
    /// Interaction logic for Form_PowerFailureSettings.xaml
    /// </summary>
    public partial class Form_PowerFailureSettings : Window
    {
        private PowerFailureSettingsViewModel pfvm;

        public Form_PowerFailureSettings(Bridge bridge)
        {
            InitializeComponent();
            pfvm = DataContext as PowerFailureSettingsViewModel;
            pfvm.CurrentBridge = bridge;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
