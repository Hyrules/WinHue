using System;
using System.Windows;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_SensorCreator.xaml
    /// </summary>
    public partial class Form_SensorCreator : Window
    {
        private Sensor modifiedsensor;
        private SensorCreatorViewModel _scvm;

        public Form_SensorCreator(Sensor obj = null)
        {
            InitializeComponent();
            _scvm = this.DataContext as SensorCreatorViewModel;
            if (obj != null)
            {
                _scvm.Sensor = obj;
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Sensor sensor = _scvm.Sensor;
            CommandResult comres;
            if (modifiedsensor == null)
            {
                comres = BridgeStore.SelectedBridge.CreateObject<Sensor>(sensor);
                MessageCollection mc = (MessageCollection) comres.resultobject;
                modifiedsensor = new Sensor() {Id = ((CreationSuccess)mc[0]).id};
            }
            else
            {
                comres = BridgeStore.SelectedBridge.ModifyObject(modifiedsensor, modifiedsensor.Id);
            }

            if (comres.Success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
                                                       
 
        }

        public string GetCreatedOrModifiedID()
        {
            return modifiedsensor.Id;
        } 

  

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
