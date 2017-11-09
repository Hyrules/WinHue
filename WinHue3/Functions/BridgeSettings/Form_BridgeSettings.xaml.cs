using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.ViewModels;
using Xceed.Wpf.Toolkit;

namespace WinHue3.Views
{
    /// <summary>
    /// Interaction logic for BridgeSettings.xaml
    /// </summary>
    public partial class Form_BridgeSettings : Window
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        BridgeSettingsViewModel _bsvm;
        private Bridge _bridge;

        public Form_BridgeSettings()
        {
            InitializeComponent();
            _bsvm = DataContext as BridgeSettingsViewModel;
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            await _bsvm.Initialize(bridge);
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();       
        }

        private void chbDHCP_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)chbDHCP.IsChecked)
            {
                tbIPAddress.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                tbGateway.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                tbProxyAddress.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                tbNetmask.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                nudProxyPort.GetBindingExpression(IntegerUpDown.ValueProperty).UpdateSource();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           /* if (_bsv == null)
                Close();*/
        }
    }
}
