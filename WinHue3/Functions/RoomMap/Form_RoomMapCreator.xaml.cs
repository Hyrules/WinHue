using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHue3.Functions.RoomMap
{
    /// <summary>
    /// Interaction logic for Form_RoomMap.xaml
    /// </summary>
    public partial class Form_RoomMapCreator : Window
    {
        private RoomMapViewModel _rmvm;

        public Form_RoomMapCreator(List<IHueObject> lights)
        {
            InitializeComponent();
            _rmvm = DataContext as RoomMapViewModel;
            ObservableCollection<HueElement> he = new ObservableCollection<HueElement>();

            foreach (Light l in lights.OfType<Light>())
            {
                he.Add(new HueElement(l));
            }

            foreach (Group g in lights.OfType<Group>())
            {
                he.Add(new HueElement(g));
            }

            _rmvm.ListLights = he;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
