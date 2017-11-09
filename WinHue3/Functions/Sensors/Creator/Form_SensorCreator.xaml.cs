using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Resources;
using WinHue3.Utils;
using WinHue3.ViewModels;

namespace WinHue3.Views
{
    /// <summary>
    /// Interaction logic for Form_SensorCreator.xaml
    /// </summary>
    public partial class Form_SensorCreator : Window
    {
        private SensorCreatorViewModel _scvm;
        private readonly Bridge _bridge;
        private string _sensorId;
        private bool _editing;

        public Form_SensorCreator(Bridge bridge, Sensor obj = null)
        {
            _bridge = bridge;
            InitializeComponent();
            _scvm = DataContext as SensorCreatorViewModel;
            if (obj != null)
            {
                _sensorId = obj.Id;
                _scvm.Sensor = obj;
                _editing = true;
                btnCreate.Content = GUI.SensorCreatorForm_EditButton;
                
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Sensor sensor = _scvm.Sensor;
            bool result;
            if (!_editing)
            {
                result = _bridge.CreateObject(sensor);
                _sensorId = _bridge.LastCommandMessages.LastSuccess.value;
            }
            else
            {
                sensor.Id = _sensorId;              
                result = _bridge.ModifyObject(sensor);
                if (result)
                {
                    _bridge.ChangeSensorConfig(sensor.Id,sensor.config);
                }

            }

            if (result)
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
            return _sensorId;
        } 

  

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
