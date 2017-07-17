using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using HueLib2;
using HueLib2.Objects.HueObject;
using WinHue3.Utils;

namespace WinHue3.ViewModels
{
    public partial class MainFormViewModel
    {
        private void Bridge_OnMessageAdded(object sender, EventArgs e)
        {
            if (SelectedBridge.lastMessages == null) return;
            log.Info(SelectedBridge.lastMessages);
            if (SelectedBridge.lastMessages.Count > 0)
                Lastmessage = SelectedBridge.lastMessages[SelectedBridge.lastMessages.Count - 1].ToString();
        }

        private void Bridge_BridgeNotResponding(object sender, EventArgs e)
        {

            MessageBox.Show(GlobalStrings.Error_Bridge_Not_Responding, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            if (e is BridgeNotRespondingEventArgs)
            {
                log.Error($"{sender}");
                log.Error(Serializer.SerializeToJson(((BridgeNotRespondingEventArgs)e).ex?.ex?.ToString()));
            }
            log.Error($"{sender} : {e.ToString()}");
            Cursor_Tools.ShowNormalCursor();
            _ctm.Stop();
            //rfm.Stop();

        }

        private void _findlighttimer_Tick(object sender, EventArgs e)
        {
            _findlighttimer.Stop();
            log.Info("Done searching for new lights.");
            List<IHueObject> hr = HueObjectHelper.GetBridgeNewLights(SelectedBridge);
            if (hr == null) return;
            List<IHueObject> newlights = hr;
            log.Info($"Found {newlights.Count} new lights.");
            ListBridgeObjects.AddRange(newlights);
            CommandManager.InvalidateRequerySuggested();
        }

        private void _findsensortimer_Tick(object sender, EventArgs e)
        {
            _findsensortimer.Stop();
            log.Info("Done searching for new sensors.");
            List<IHueObject> hr = HueObjectHelper.GetBridgeNewSensors(SelectedBridge);
            if (hr == null) return;
            List<IHueObject> newsensors = hr;
            log.Info($"Found {newsensors.Count} new sensors.");
            ListBridgeObjects.AddRange(newsensors);
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
