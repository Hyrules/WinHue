using System.Windows;
using Xceed.Wpf.Toolkit;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for BridgeSettings.xaml
    /// </summary>
    public partial class Form_BridgeSettings : Window
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        BridgeSettingsViewModel _bsvm;
        private readonly Bridge _bridge;

        public Form_BridgeSettings(Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            _bsvm = DataContext as BridgeSettingsViewModel;
            _bsvm.Bridge = _bridge;

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
