using System;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.Daylight;
using WinHue3.Utils;

namespace WinHue3.Functions.Sensors.Daylight
{
    /// <summary>
    /// Interaction logic for Form_PowerFailure.xaml
    /// </summary>
    public partial class Form_Daylight : Window
    {
        
        private string _id;
        private Bridge _bridge;
        private DaylightViewModel _dvm => this.DataContext as DaylightViewModel;

        public Form_Daylight(Bridge bridge,Sensor obj)
        {
            InitializeComponent();
            _bridge = bridge;
            _dvm.SetDaylight(obj); 
            _id = obj.Id;

        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {

            DaylightSensorConfig config = new DaylightSensorConfig
            {
                @long = _dvm.Daylight.Longitude,
                lat = _dvm.Daylight.Latitude,
                sunriseoffset = Convert.ToSByte(_dvm.Daylight.SunriseOffset),
                sunsetoffset = Convert.ToSByte(_dvm.Daylight.SunsetOffset)
            };

            bool bresult = _bridge.ChangeSensorConfig(_id, config);
            if(bresult)
            {
                this.Close();
            }
            else
            {
               
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
 

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
