using System.Windows;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.Lights.Finder
{
    /// <summary>
    /// Logique d'interaction pour Form_AddLightSerial.xaml
    /// </summary>
    public partial class Form_AddLightSerial : Window
    {
        private AddLightSerialViewModel _asf;
        private Bridge _bridge;
        public Form_AddLightSerial(Bridge bridge)
        {
            InitializeComponent();
            _bridge = bridge;
            _asf = DataContext as AddLightSerialViewModel;

        }

        private async void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (_asf.ListSerials.IsValid())
            {
                DialogResult = await _bridge.FindNewLightsAsync(_asf.ListSerials);
                Close();
            }
            else
            {
                MessageBox.Show(GlobalStrings.SerialCannotBeEmpty_Error,GlobalStrings.Warning,MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
