using System.Text;
using System.Windows;
using HueLib2;

namespace WinHue3
{
    public static class MessageBoxError
    {
        public static void ShowLastErrorMessages(Bridge bridge)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Message m in bridge.lastMessages)
            {
                sb.AppendLine(m.ToString());
            }

            MessageBox.Show(sb.ToString(), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
