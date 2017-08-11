using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WinHue3.Interface
{
    public static class ScreenPixelHandler 
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        static Bitmap screenPixel;

        static ScreenPixelHandler()
        {
            screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
         //   HookManager.MouseClick+= 

        }

      

        public static System.Drawing.Color GetColorAtCursor()
        {
            Point cursor = new Point();
            GetCursorPos(ref cursor);
            
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, cursor.X, cursor.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
           
            return screenPixel.GetPixel(0, 0);
        }


    }
}
