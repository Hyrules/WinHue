using HueLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
