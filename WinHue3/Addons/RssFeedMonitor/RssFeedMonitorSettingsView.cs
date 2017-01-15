using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WinHue3
{
    public class RssFeedMonitorSettingsView : View
    {
        private ObservableCollection<Alert> _listalerts;
        private Alert _selectedAlert;
        private double _checkdelay;

        public RssFeedMonitorSettingsView()
        {
            _checkdelay = Properties.Settings.Default.UpdateInterval;
            _listalerts = new ObservableCollection<Alert>(RssFeedAlertHandler.LoadRssFeedAlerts());
        }

        public ObservableCollection<Alert> ListAlerts
        {
            get
            {
                return _listalerts;
                
            }
            set
            {
                _listalerts = value;
                OnPropertyChanged();
            }

        }

        public bool CanEditAlert => _selectedAlert != null;

        public string UpdateDelayString => GlobalStrings.Update_Delay_String;

        public string AlertName => _selectedAlert == null ? GlobalStrings.None : _selectedAlert.Name;

        public string AlertDescription => _selectedAlert == null ? GlobalStrings.None : _selectedAlert.Description;

        public Alert SelectedAlert
        {
            get
            {
                return _selectedAlert;
                
            }
            set
            {
                _selectedAlert = value;
                OnPropertyChanged();
                OnPropertyChanged("CanEditAlert");
            }
        }

        public double AlertCheckDelay
        {
            get
            {
                return _checkdelay;
            }
            set
            {
                _checkdelay = value;
                OnPropertyChanged();
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
            Form_AlertCreator fac = new Form_AlertCreator(_selectedAlert) { Owner = Application.Current.MainWindow };
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
            Form_AlertCreator fac = new Form_AlertCreator(_selectedAlert) {Owner = Application.Current.MainWindow};
            fac.Show();
        }

        public ICommand CreateNewAlertCommand => new RelayCommand(param => CreateNewAlert());
        public ICommand EditAlertCommand => new RelayCommand(param => EditAlert());
        public ICommand DoubleClickObjectCommand  => new RelayCommand(param => DoubleClickObject());
    }
}
