using System;
using System.Windows;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_SensorCreator.xaml
    /// </summary>
    public partial class Form_SensorCreator : Window
    {
        readonly Bridge _br;
        string _id;
        readonly SensorCreatorView _scv;

        public Form_SensorCreator(Bridge bridge)
        {
            _br = bridge;
            InitializeComponent();
            _scv = new SensorCreatorView();
            DataContext = _scv;
        }

        public Form_SensorCreator(Bridge bridge, Sensor sensor)
        {
            _br = bridge;
            InitializeComponent();
            _scv = new SensorCreatorView(sensor);
            DataContext = _scv;
            _id = sensor.Id;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Sensor sensor = _scv.GetSensor();

            if (sensor.Id == null)
            {
                _id = _br.CreateSensor(sensor);            
            }
            else
            {
                _br.UpdateSensor(_id,sensor);
            }

            if (_id != "")
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_br);
            }
                                                       
 
        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        } 

        private void btnTestURL_Click(object sender, RoutedEventArgs e)
        {
            string msg = null;
            try
            {
                CommResult comres = Communication.SendRequest(new Uri(tbSensorUrl.Text), WebRequestType.GET);
                msg = comres.data;
                if (msg != null)
                {
                    if (msg.Length > 10)
                        msg = msg.Substring(0, 30);
                    System.Windows.MessageBox.Show(msg, GlobalStrings.Sensor_TestUrl, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show(GlobalStrings.Sensor_UnableToComm, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch(UriFormatException)
            {
                System.Windows.MessageBox.Show(GlobalStrings.Sensor_InvalidURL, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(ArgumentNullException)
            {
                System.Windows.MessageBox.Show(GlobalStrings.Sensor_InvalidURL,GlobalStrings.Error,MessageBoxButton.OK,MessageBoxImage.Error);
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show(GlobalStrings.Sensor_Error,GlobalStrings.Error,MessageBoxButton.OK,MessageBoxImage.Error);
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
