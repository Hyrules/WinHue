using System;
using System.Collections.Generic;
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
    public class CreateFloorPlanViewModel : ValidatableBindableBase
    {
        private Stretch _stretchMode;
        private bool _useImageSize;
        private double _height;
        private double _width;
        private string _imagePath;
        private string _floorPlanName;
        private ImageSource _image;
        private OpenFileDialog ofd;

        public CreateFloorPlanViewModel()
        {
            StretchMode = Stretch.None;
            Width = 800;
            Height = 600;
            FloorPlanName = "NewPlan01";
            Image = null;
            ofd = new OpenFileDialog {Filter = "All Supported Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png", DefaultExt = "*.jpg;*.png;*.bmp"};
        }

        public ICommand LoadImageCommand => new RelayCommand(param => LoadImage());
        public ICommand UseImageSizeCommand => new RelayCommand(param => SetImageSize());

        private void SetImageSize()
        {
            if (Image == null) return;
            if (!UseImageSize) return;
            Height = Image.Height;
            Width = Image.Width;
        }

        private void LoadImage()
        {
            if (ofd.ShowDialog() == true)
            {
                Image = new BitmapImage(new Uri(ofd.FileName));
                ImagePath = ofd.FileName;
                SetImageSize();
            }
        }

        public Stretch StretchMode
        {
            get => _stretchMode; 
            set => SetProperty(ref _stretchMode,value);
        }

        public bool UseImageSize
        {
            get => _useImageSize;
            set => SetProperty(ref _useImageSize,value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height,value);
        }

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width,value);
        }

        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath,value);
        }

        public string FloorPlanName
        {
            get => _floorPlanName;
            set => SetProperty(ref _floorPlanName,value);
        }

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }
    }
}
