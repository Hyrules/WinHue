using System.Linq;
using System.Text;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;

namespace WinHue3.Utils
{
    public static class MessageBoxError
    {
        public static void ShowLastErrorMessages(Bridge bridge)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Error m in bridge.LastCommandMessages.ListMessages.OfType<Error>())
            {
                sb.AppendLine(m.ToString());
            }

            MessageBox.Show(sb.ToString(), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
