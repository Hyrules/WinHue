using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using WinHue3.Utils;

namespace WinHue3.Functions.RoomMap
{
    public class RoomMapViewModel : ValidatableBindableBase
    {
        private OpenFileDialog ofd;

        public RoomMapViewModel()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png";
            ofd.DefaultExt = "*.jpg";
        }

        public ICommand ChooseImageCommand => new RelayCommand(param => ChooseImage());

        private void ChooseImage()
        {
            ofd.ShowDialog();
        }
    }
}
