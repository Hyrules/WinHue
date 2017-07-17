using System;
using System.Windows;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_PowerFailure.xaml
    /// </summary>
    public partial class Form_Daylight : Window
    {
        
        private string _id;

        public DaylightViewModel _dvm => this.DataContext as DaylightViewModel;
        private readonly Bridge _bridge;

        public Form_Daylight(Sensor obj,Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
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

            CommandResult<MessageCollection> bresult = _bridge.ChangeSensorConfig(_id, config);
            if(bresult.Success)
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
