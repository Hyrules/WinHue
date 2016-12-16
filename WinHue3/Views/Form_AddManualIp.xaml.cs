using System.Windows;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_AddManualIp.xaml
    /// </summary>
    public partial class Form_AddManualIp : Window
    {
        AddManualIPView amiv;

        public Form_AddManualIp()
        {
            InitializeComponent();
            amiv = new AddManualIPView();
            DataContext = amiv;
        }

        public string GetIPAddress()
        {
            return amiv.BridgeIPAddress;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnSaveIp_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
