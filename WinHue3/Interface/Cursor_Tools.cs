using System.Windows;
using System.Windows.Input;

namespace WinHue3.Interface
{
    public static class Cursor_Tools
    {
        public static void ShowWaitCursor()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });
        }

        public static void ShowNormalCursor()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }
    }
}
