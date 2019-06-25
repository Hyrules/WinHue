using System.Linq;
using System.Windows;
using WinHue3.Interface;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.Communication;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel
    {
        private void Bridge_OnMessageAdded(object sender, MessageAddedEventArgs e)
        {
            Lastmessage = e.Messages.Last().ToString();              
        }
        
        private void Bridge_BridgeNotResponding(object sender, BridgeNotRespondingEventArgs e)
        {
            e.Bridge.BridgeNotResponding -= Bridge_BridgeNotResponding;
            
            MessageBox.Show(GlobalStrings.Error_Bridge_Not_Responding, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            log.Error($"{sender}");
            log.Error(Serializer.SerializeJsonObject(e.Exception.ToString()));
            log.Error($"{sender} : {e}");
            Cursor_Tools.ShowNormalCursor();

            e.Bridge.BridgeNotResponding += Bridge_BridgeNotResponding;
        }






    }
}
