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
