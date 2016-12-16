using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WinHue3
{
    public class ColorFromImageView : View
    {
        private ImageSource _imagesource;
        private bool _canloadrgb;
        private bool _canselectcolor;
        private System.Windows.Media.Color _selectedColor;

        #region CTOR
        public ColorFromImageView()
        {
            _imagesource = GDIManager.CreateImageSourceFromImage(Properties.Resources.rgbcolor);
            _selectedColor = new System.Windows.Media.Color() { R = 255, G = 255, B = 255 };
           
        }
        #endregion

        #region PROPERTIES
        public ImageSource ImageSource
        {
            get
            {
                return _imagesource;
            }
            set
            {
                _imagesource = value;
                OnPropertyChanged();
            }
        }

        public bool CanLoadRGB
        {
            get
            {
                return _canloadrgb;
            }
            set
            {
                _canloadrgb = value;
                OnPropertyChanged();
            }
        }

        public bool CanSelectColor
        {
            get
            {
                return _canselectcolor;
            }
            set
            {
                _canselectcolor = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush SelectedColorBackground
        {
            get
            {
                if (_selectedColor == null) return new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                return new SolidColorBrush(System.Windows.Media.Color.FromRgb(_selectedColor.R, _selectedColor.G, _selectedColor.B));
            }
        }

        
        public System.Windows.Media.Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                _selectedColor = value;
                OnPropertyChanged();
                CanSelectColor = true;
                OnPropertyChanged("SelectedColorBackground");
            }
        }
        #endregion

        #region COMMANDS

        public ICommand LoadImageCommand => new RelayCommand(param => LoadImage());
        public ICommand LoadRGBColorCommand => new RelayCommand(param => LoadRGB());

        #endregion

        #region METHODS
        private void LoadImage()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter =
                "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif|BMP Files | (*.bmp)";
            if (ofd.ShowDialog() == true)
            {
                ImageSource = new BitmapImage(new Uri(ofd.FileName));
                CanLoadRGB = true;
            }
        }
        
        private void LoadRGB()
        {
            if (MessageBox.Show(GlobalStrings.Scene_Load_RGB, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ImageSource = GDIManager.CreateImageSourceFromImage(Properties.Resources.rgbcolor);
                CanLoadRGB = false;
            }

            ImageSource = GDIManager.CreateImageSourceFromImage(Properties.Resources.rgbcolor);
        }

        #endregion
    }
}
