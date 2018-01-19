using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WinHue3.Interface
{
    public static class ScreenPixelHandler 
    {


        static readonly Bitmap screenPixel;

        static ScreenPixelHandler()
        {
            screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
         //   HookManager.MouseClick+= 

        }

      

        public static System.Drawing.Color GetColorAtCursor()
        {
            Point cursor = new Point();
            NativeMethods.GetCursorPos(ref cursor);
            
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = NativeMethods.BitBlt(hDC, 0, 0, 1, 1, hSrcDC, cursor.X, cursor.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
           
            return screenPixel.GetPixel(0, 0);
        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            internal static extern bool GetCursorPos(ref Point lpPoint);

            [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
            internal static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        }
    }
}
