using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HueLib;
using HueLib_base;

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

        public Form_Daylight(Bridge bridge,HueObject obj)
        {
            InitializeComponent();
            br = bridge;
            id = obj.Id;
            daylightView = new DaylightView(br.GetSensor("1"));
            DataContext = daylightView;
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {

            if(br.UpdateSensor(id, daylightView.GetSensor()))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("One or more error occured : \r\n\r\n" + br.lastMessages.ToString(), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
 

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
