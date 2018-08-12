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
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHue3.Functions.RoomMap
{
    /// <summary>
    /// Interaction logic for Form_RoomMap.xaml
    /// </summary>
    public partial class Form_RoomMapCreator : Window
    {
        private RoomMapViewModel _rmvm;

        public Form_RoomMapCreator(List<Light> lights)
        {
            InitializeComponent();
            _rmvm = DataContext as RoomMapViewModel;
            _rmvm.ListLights = new ObservableCollection<Light>(lights);
        }


    }
}
