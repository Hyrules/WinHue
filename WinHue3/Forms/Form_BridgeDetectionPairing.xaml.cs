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
using HueLib;
using Newtonsoft.Json.Serialization;
using System.Collections.ObjectModel;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BridgeDetectionPairing.xaml
    /// </summary>
    public partial class Form_BridgeDetectionPairing : Window
    {
        private BridgeDetectionPairingView _dpv;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Form_BridgeDetectionPairing()
        {
            InitializeComponent();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            _dpv = new BridgeDetectionPairingView();
            DataContext = _dpv;

        }

       private void btnDone_Click(object sender, RoutedEventArgs e)
        {
           
           if (_dpv.SaveSettings())
           {
               DialogResult = true;
               Close();
           }
           else
           {
               MessageBox.Show(GlobalStrings.SaveSettings_Error, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
               log.Error("Error while saving settings.");
           }
        }

        public ObservableCollection<Bridge> GetModifications()
        {          
            return _dpv.ListViewSource;
        }

 
    }
}
