using System;
using System.Windows;
using HueLib2;
using HueLib2.BridgeMessages;
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
        private readonly Bridge _bridge;

        public Form_SensorCreator(Bridge bridge, Sensor obj = null)
        {
            _bridge = bridge;
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
            CommandResult<Messages> comres;
            if (modifiedsensor == null)
            {
                comres = _bridge.CreateObject<Sensor>(sensor);
                Messages mc = comres.Data;
                modifiedsensor = new Sensor() {Id = mc.SuccessMessages[0].value};
            }
            else
            {
                comres = _bridge.ModifyObject(modifiedsensor, modifiedsensor.Id);
            }

            if (comres.Success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
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
