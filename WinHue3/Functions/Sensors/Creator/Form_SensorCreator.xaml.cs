using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Resources;
using WinHue3.Utils;

namespace WinHue3.Functions.Sensors.Creator
{
    /// <summary>
    /// Interaction logic for Form_SensorCreator.xaml
    /// </summary>
    public partial class Form_SensorCreator : Window
    {
        private SensorCreatorViewModel _scvm;
        private string _sensorId;
        private bool _editing;

        public Form_SensorCreator(Sensor obj = null)
        {
            InitializeComponent();
            _scvm = DataContext as SensorCreatorViewModel;
            if (obj != null)
            {
                _sensorId = obj.Id;
                _scvm.Sensor = obj;
                _editing = true;
                BtnCreate.Content = GUI.SensorCreatorForm_EditButton;
                
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Sensor sensor = _scvm.Sensor;
            bool result;
            if (!_editing)
            {
                result = BridgeManager.BridgeManager.Instance.SelectedBridge.CreateObject(sensor);
                _sensorId = BridgeManager.BridgeManager.Instance.SelectedBridge.LastCommandMessages.LastSuccess.value;
            }
            else
            {
                sensor.Id = _sensorId;              
                result = BridgeManager.BridgeManager.Instance.SelectedBridge.ModifyObject(sensor);
                if (result)
                {
                    BridgeManager.BridgeManager.Instance.SelectedBridge.ChangeSensorConfig(sensor.Id,sensor.config);
                }

            }

            if (result)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeManager.BridgeManager.Instance.SelectedBridge);
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
