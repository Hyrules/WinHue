using System;
using System.Runtime.InteropServices;

namespace WinHue3.Colors
{
    class ColorPicker
    {

        public System.Drawing.Color GetColorAt(int x, int y)
        {
            IntPtr desk = NativeMethods.GetDesktopWindow();
            IntPtr dc = NativeMethods.GetWindowDC(desk);
            int a = (int)NativeMethods.GetPixel(dc, x, y);
            NativeMethods.ReleaseDC(desk, dc);
            return System.Drawing.Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);


        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetWindowDC(IntPtr window);
            [DllImport("gdi32.dll", SetLastError = true)]
            internal static extern uint GetPixel(IntPtr dc, int x, int y);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern int ReleaseDC(IntPtr window, IntPtr dc);

        }
    }
}
