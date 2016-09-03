using System.Windows;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_PowerFailure.xaml
    /// </summary>
    public partial class Form_Daylight : Window
    {
        private Bridge br;
        private DaylightView daylightView;
        private string id;

        public Form_Daylight(Bridge bridge,Sensor obj)
        {
            InitializeComponent();
            br = bridge;
            id = obj.Id;
            daylightView = new DaylightView(obj);
            DataContext = daylightView;
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            CommandResult bresult = br.ModifyObject<Sensor>(daylightView.GetSensor(), id);
            if(bresult.Success)
            {
                this.Close();
            }
            else
            {
               
                MessageBoxError.ShowLastErrorMessages(br);
            }
 

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
