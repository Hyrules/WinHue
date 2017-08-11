using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Utils;


namespace WinHue3.Addons.RssFeedMonitor
{
    public class RssFeedMonitorSettingsView : WinHue3.View
    {
        private ObservableCollection<Alert> _listalerts;
        private Alert _selectedAlert;
        private double _checkdelay;
        private Bridge _bridge;
        public RssFeedMonitorSettingsView()
        {
            _checkdelay = Properties.Settings.Default.UpdateInterval;
            _listalerts = new ObservableCollection<Alert>(RssFeedAlertHandler.LoadRssFeedAlerts());
        }

        public ObservableCollection<Alert> ListAlerts
        {
            get => _listalerts;
            set
            {
                _listalerts = value;
                RaisePropertyChanged();
            }

        }

        public bool CanEditAlert => _selectedAlert != null;

        public string UpdateDelayString => GlobalStrings.Update_Delay_String;

        public string AlertName => _selectedAlert == null ? GlobalStrings.None : _selectedAlert.Name;

        public string AlertDescription => _selectedAlert == null ? GlobalStrings.None : _selectedAlert.Description;

        public Alert SelectedAlert
        {
            get => _selectedAlert;
            set
            {
                _selectedAlert = value;
                RaisePropertyChanged();
                RaisePropertyChanged("CanEditAlert");
            }
        }

        public double AlertCheckDelay
        {
            get => _checkdelay;
            set
            {
                _checkdelay = value;
                RaisePropertyChanged();
            }
        }

        private void CreateNewAlert()
        {
        /*    Form_AlertCreator fcc = new Form_AlertCreator() { Owner = Application.Current.MainWindow };
            if (fcc.ShowDialog() == true)
            {
                //PopulateAlertList();
            }*/
        }

        private void EditAlert()
        {
            if (_selectedAlert == null) return;
            View.Form_AlertCreator fac = new View.Form_AlertCreator(_bridge,_selectedAlert) { Owner = Application.Current.MainWindow };
            if (fac.ShowDialog() == true)
            {
                
            }
             //   PopulateAlertList();
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.UpdateInterval = AlertCheckDelay;
            Properties.Settings.Default.Save();
       
        }

        public void DoubleClickObject()
        {
            if (_selectedAlert == null) return;
            View.Form_AlertCreator fac = new View.Form_AlertCreator(_bridge,_selectedAlert) {Owner = Application.Current.MainWindow};
            fac.Show();
        }

        public ICommand CreateNewAlertCommand => new RelayCommand(param => CreateNewAlert());
        public ICommand EditAlertCommand => new RelayCommand(param => EditAlert());
        public ICommand DoubleClickObjectCommand  => new RelayCommand(param => DoubleClickObject());
    }
}
