using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Interface
{
    public static class GDIManager
    {
        public static ImageSource CreateImageSourceFromImage(Bitmap img)
        {
            try
            {
                IntPtr pointer = img.GetHbitmap();
                ImageSource imgsource = Imaging.CreateBitmapSourceFromHBitmap(pointer, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                NativeMethods.DeleteObject(pointer);
                return imgsource;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            internal static extern bool DeleteObject(IntPtr hObject);
        }
    }
}
