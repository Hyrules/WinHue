using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_SelectColorFromImage.xaml
    /// </summary>
    public partial class Form_SelectColorFromImage : Window
    {
        private ColorFromImageView cfiv;

        public Form_SelectColorFromImage()
        {
            InitializeComponent();
            cfiv = new ColorFromImageView();
            DataContext = cfiv;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ImageViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageViewer.Source != null)
            {
                Color c = PickColor(e.GetPosition(this).X,e.GetPosition(this).Y);
                cfiv.SelectedColor = c;
            }

        }

        private Color PickColor(double x, double y)
        {

            BitmapSource bitmapSource = ImageViewer.Source as BitmapSource;
            if (bitmapSource != null)
            {
                // Get color from bitmap pixel.
                // Convert coopdinates from WPF pixels to Bitmap pixels
                // and restrict them by the Bitmap bounds.
                x *= bitmapSource.PixelWidth/ActualWidth;
                if ((int) x > bitmapSource.PixelWidth - 1)
                    x = bitmapSource.PixelWidth - 1;
                else if (x < 0)
                    x = 0;
                y *= bitmapSource.PixelHeight/ActualHeight;
                if ((int) y > bitmapSource.PixelHeight - 1)
                    y = bitmapSource.PixelHeight - 1;
                else if (y < 0)
                    y = 0;

                // Lee Brimelow approach (http://thewpfblog.com/?p=62).
                //byte[] pixels = new byte[4];
                //CroppedBitmap cb = new CroppedBitmap(bitmapSource, 
                //                   new Int32Rect((int)x, (int)y, 1, 1));
                //cb.CopyPixels(pixels, 4, 0);
                //return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);

                // Alternative approach
                if (bitmapSource.Format == PixelFormats.Indexed4)
                {
                    byte[] pixels = new byte[1];
                    int stride = (bitmapSource.PixelWidth*
                                  bitmapSource.Format.BitsPerPixel + 3)/4;
                    bitmapSource.CopyPixels(new Int32Rect((int) x, (int) y, 1, 1),
                        pixels, stride, 0);
                    return bitmapSource.Palette.Colors[pixels[0] >> 4];
                }
                else if (bitmapSource.Format == PixelFormats.Indexed8)
                {
                    byte[] pixels = new byte[1];
                    int stride = (bitmapSource.PixelWidth*
                                  bitmapSource.Format.BitsPerPixel + 7)/8;
                    bitmapSource.CopyPixels(new Int32Rect((int) x, (int) y, 1, 1),
                        pixels, stride, 0);
                    return bitmapSource.Palette.Colors[pixels[0]];
                }
                else
                {
                    byte[] pixels = new byte[4];
                    int stride = (bitmapSource.PixelWidth*
                                  bitmapSource.Format.BitsPerPixel + 7)/8;
                    bitmapSource.CopyPixels(new Int32Rect((int) x, (int) y, 1, 1),
                        pixels, stride, 0);

                    return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
                }
                
            }
            return Color.FromArgb(255,0,0,0);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public Color GetSelectedColor()
        {
            return cfiv.SelectedColor;
        }

    }
}
