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
        
        private string id;

        public DaylightViewModel dvm => this.DataContext as DaylightViewModel;

        public Form_Daylight(Sensor obj)
        {
            InitializeComponent();
            dvm.SetDaylight(obj); 
            id = obj.Id;

        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {

            DaylightSensorConfig config = new DaylightSensorConfig
            {
                @long = dvm.Daylight.Longitude,
                lat = dvm.Daylight.Latitude,
                sunriseoffset = Convert.ToSByte(dvm.Daylight.SunriseOffset),
                sunsetoffset = Convert.ToSByte(dvm.Daylight.SunsetOffset)
            };

            CommandResult bresult = BridgeStore.SelectedBridge.ChangeSensorConfig(id, config);
            if(bresult.Success)
            {
                this.Close();
            }
            else
            {
               
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
 

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
