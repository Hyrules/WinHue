using System.Windows;
using Xceed.Wpf.Toolkit;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for BridgeSettings.xaml
    /// </summary>
    public partial class Form_BridgeSettings : Window
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        BridgeSettingsView _bsv;

        public Form_BridgeSettings()
        {
            InitializeComponent();
            CommandResult bresult = BridgeStore.SelectedBridge.GetBridgeSettings();
            if (!bresult.Success) return;
            _bsv = new BridgeSettingsView((BridgeSettings)bresult.resultobject);
            DataContext = _bsv;
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
            if (_bsv == null)
                Close();
        }
    }
}
