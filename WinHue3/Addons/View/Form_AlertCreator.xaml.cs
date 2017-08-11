using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinHue3.Addons.RssFeedMonitor;
using WinHue3.Addons.ViewModel;
using Bridge = WinHue3.Philips_Hue.BridgeObject.Bridge;

namespace WinHue3.Addons.View
{
    /// <summary>
    /// Interaction logic for Form_AlertCreator.xaml
    /// </summary>
    public partial class Form_AlertCreator : Window
    {

        private bool _actionSet;
        private RssFeedMonitor.Form_ActionPicker fap;
        private string _oldName;
        private AlertCreatorViewModel _alertCreatorModelView;

        public Form_AlertCreator(Bridge bridge, Alert alert = null)
        {
            InitializeComponent();
            _alertCreatorModelView = DataContext as AlertCreatorViewModel;
            fap = new RssFeedMonitor.Form_ActionPicker(bridge);
            _oldName = null;

            if(alert != null)
            {
                _alertCreatorModelView.Alert = alert;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }



        private void ClearCondition()
        {
            cbCondition.SelectedIndex = 0;
            tbUserCondition.Text = string.Empty;
            cbRssElements.SelectedIndex = -1;
            cbRssElements.SelectedItem = null;
            cbRssElements.Text = "";
          
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbUrl.Text = string.Empty;
            btnCheck.IsEnabled = true;
            lbConditions.Items.Clear();
            ClearCondition();
            _actionSet = false;
        }

        private void lbConditions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;
            if (lbConditions.SelectedItem == null) return;

            switch (((Criteria)lbConditions.SelectedItem).RSSElement)
            {
                case "Title":
                    cbiTitle.Visibility = Visibility.Visible;
                    break;
                case "Description":
                    cbiEquals.Visibility = Visibility.Visible;
                    break;
                case "Category":
                    cbiCat.Visibility = Visibility.Visible;
                    break;
                case "Publication Date":
                    cbiPubDate.Visibility = Visibility.Visible;
                    break;
            }

            lbConditions.Items.Remove(lbConditions.SelectedItem);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text) || string.IsNullOrEmpty(tbName.Name))
            {
                MessageBox.Show("Please enter a name for the alert to check.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (lbConditions.Items.Count == 0)
            {
                MessageBox.Show("Please enter at least one alert to check.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!_actionSet)
            {
                MessageBox.Show("Please select the action desired upon criteria match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }

            Alert newCondition = _alertCreatorModelView.Alert;

            if (_oldName != null)
            {
                RssFeedAlertHandler.DeleteFeedAlert(_oldName);
            }

            if (RssFeedAlertHandler.SaveFeedAlert(newCondition))
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Therer was an error while saving the current Rss Feed Alert.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void cbRssElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbiGreater == null || cbiLower == null) return;
            if (cbRssElements.SelectedIndex == 3)
            {
                cbiGreater.Visibility = Visibility.Visible;
                cbiLower.Visibility = Visibility.Visible;
            }
            else
            {
                cbiGreater.Visibility = Visibility.Collapsed;
                cbiLower.Visibility = Visibility.Collapsed;
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fap.Owner = this;
        }
    }
}
