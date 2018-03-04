using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using Xceed.Wpf.Toolkit;

namespace WinHue3.Functions.BridgeSettings
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
            if (!(bool)ChbDhcp.IsChecked)
            {
                TbIpAddress.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                TbGateway.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                TbProxyAddress.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                TbNetmask.GetBindingExpression(WatermarkTextBox.TextProperty).UpdateSource();
                NudProxyPort.GetBindingExpression(IntegerUpDown.ValueProperty).UpdateSource();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           /* if (_bsv == null)
                Close();*/
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_bsvm.CanClose == false) e.Cancel = true;
        }
    }
}
