using System.Text;
using System.Windows;
using HueLib2;
using HueLib2.BridgeMessages;

namespace WinHue3
{
    public static class MessageBoxError
    {
        public static void ShowLastErrorMessages(Bridge bridge)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Error m in bridge.lastMessages.ErrorMessages)
            {
                sb.AppendLine(m.ToString());
            }

            MessageBox.Show(sb.ToString(), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
