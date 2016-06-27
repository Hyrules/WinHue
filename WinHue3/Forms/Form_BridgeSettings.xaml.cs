using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using HueLib;
using Xceed.Wpf.Toolkit;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for BridgeSettings.xaml
    /// </summary>
    public partial class Form_BridgeSettings : Window
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        BridgeSettingsView _bsv;

        public Form_BridgeSettings(Bridge bridge)
        {
            InitializeComponent();
            _bsv = new BridgeSettingsView(bridge);
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

    }
}
