using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WinHue3.Utils;

namespace WinHue3.Functions.RoomMap
{
    public class RoomMapViewModel : ValidatableBindableBase
    {
        private OpenFileDialog ofd;
        private ImageSource _floorPlan;

        public RoomMapViewModel()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png";
            ofd.DefaultExt = "*.jpg";
        }

        public ICommand ChooseImageCommand => new RelayCommand(param => ChooseImage());

        public ImageSource FloorPlan
        {
            get { return _floorPlan; }
            set { SetProperty(ref _floorPlan,value); }
        }

        private void ChooseImage()
        {
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                FloorPlan = new BitmapImage(new Uri(ofd.FileName));
            }
        }
    }
}
