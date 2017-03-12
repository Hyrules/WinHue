using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HueLib2;

namespace WinHue3.Addons.ViewModel
{
    public class RssFeedSettingsViewModel : ValidatableBindableBase
    {
        private ObservableCollection<Alert> _listalerts;
        private Alert _selectedAlert;
        private double _checkdelay;
        private Bridge _bridge;

        public string UpdateDelayString => GlobalStrings.Update_Delay_String;

        public RssFeedSettingsViewModel()
        {
            _checkdelay = Properties.Settings.Default.UpdateInterval;
            _listalerts = new ObservableCollection<Alert>(RssFeedAlertHandler.LoadRssFeedAlerts());
        }

        public ObservableCollection<Alert> ListAlerts
        {
            get { return _listalerts; }
            set { SetProperty(ref _listalerts,value); }
        }

        public Alert SelectedAlert
        {
            get { return _selectedAlert; }
            set { SetProperty(ref _selectedAlert,value);}
        }

        public double AlertCheckDelay
        {
            get { return _checkdelay; }
            set { SetProperty(ref _checkdelay,value); }
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
            Form_AlertCreator fac = new Form_AlertCreator(_bridge, _selectedAlert) { Owner = Application.Current.MainWindow };
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
            Form_AlertCreator fac = new Form_AlertCreator(_bridge, _selectedAlert) { Owner = Application.Current.MainWindow };
            fac.Show();
        }

        public ICommand CreateNewAlertCommand => new RelayCommand(param => CreateNewAlert());
        public ICommand EditAlertCommand => new RelayCommand(param => EditAlert());
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => DoubleClickObject());

    }
}
