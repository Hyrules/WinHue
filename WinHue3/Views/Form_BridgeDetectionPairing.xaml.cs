using System;
using System.Windows;
using System.Collections.ObjectModel;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BridgeDetectionPairing.xaml
    /// </summary>
    public partial class Form_BridgeDetectionPairing : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BridgePairingViewModel _bpvm;

        public Form_BridgeDetectionPairing()
        {
            InitializeComponent();
            _bpvm = this.DataContext as BridgePairingViewModel;
            
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
           
            if (_bpvm.SaveSettings())
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

    }
}
