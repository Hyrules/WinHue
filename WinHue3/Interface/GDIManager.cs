using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WinHue3
{
    public static class GDIManager
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource CreateImageSourceFromImage(Bitmap img)
        {
            try
            {
                IntPtr pointer = img.GetHbitmap();
                ImageSource imgsource = Imaging.CreateBitmapSourceFromHBitmap(pointer, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(pointer);
                return imgsource;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
