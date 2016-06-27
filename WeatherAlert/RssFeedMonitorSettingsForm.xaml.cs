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

using System.Xaml;
using System.Xml;
using WinHuePluginModule;

namespace RssFeedMonitor
{
    /// <summary>
    /// Interaction logic for WeatherSettingsForm.xaml
    /// </summary>
    public partial class RssFeedMonitorSettingsForm : Window
    {
        private IWinhuePluginHost _host;
        public RssFeedMonitorSettingsForm(IWinhuePluginHost host)
        {
            InitializeComponent();
            _host = host;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            Form_AlertCreator fcc = new Form_AlertCreator(_host) {Owner = this};
            if (fcc.ShowDialog() == true)
            {
                PopulateAlertList();
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PopulateAlertList()
        {
            List<Alert> listConditions = RssFeedAlertHandler.LoadRssFeedAlerts();

            foreach (Alert c in listConditions)
            {
                lbConditionList.Items.Add(c);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateAlertList();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lbConditionList.SelectedItem == null) return;
            Form_AlertCreator fac = new Form_AlertCreator(_host,(Alert)lbConditionList.SelectedItem) {Owner = this};
            if(fac.ShowDialog() == true)
                PopulateAlertList();
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.UpdateInterval = (int)nudUpdateInterval.Value;
            Properties.Settings.Default.Save();
            Close();
        }

        private void lbConditionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbConditionList.SelectedItem == null)
            {
                lblName.Content = "none";
                lblDescription.Content = "none";
                chbIsEnabled.IsChecked = null;
                btnEdit.IsEnabled = false;
            }
            else
            {
                Alert current = (Alert)lbConditionList.SelectedItem;
                lblName.Content = current.Name;
                lblDescription.Content = current.Description;
                chbIsEnabled.IsChecked = current.Enabled;
                btnEdit.IsEnabled = true;
            }
        }
    }
}
