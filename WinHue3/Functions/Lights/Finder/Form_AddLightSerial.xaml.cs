using System.Windows;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Lights.Finder
{
    /// <summary>
    /// Logique d'interaction pour Form_AddLightSerial.xaml
    /// </summary>
    public partial class Form_AddLightSerial : Window
    {
        private AddLightSerialViewModel _asf;

        public Form_AddLightSerial()
        {
            InitializeComponent();
            _asf = DataContext as AddLightSerialViewModel;

        }

        private async void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (_asf.ListSerials.IsValid())
            {
                DialogResult = await BridgeManager.BridgeManager.Instance.SelectedBridge.FindNewLightsAsync(_asf.ListSerials);
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
