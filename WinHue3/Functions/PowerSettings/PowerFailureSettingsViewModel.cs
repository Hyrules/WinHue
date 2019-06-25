using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.PowerSettings
{
    public class PowerFailureSettingsViewModel : ValidatableBindableBase
    {
        private List<Light> _listLights;
        private Bridge _bridge;

        public PowerFailureSettingsViewModel()
        {
            ListLights = new List<Light>();
        }

        public ICommand SetPowerFailureCommand => new AsyncRelayCommand(SetPowerFailure);
        public ICommand SetPowerCustomCommand => new RelayCommand(SetPowerCustom);
        public ICommand SetRefreshLightCommand => new AsyncRelayCommand(SetRefreshLight);

        private async Task SetRefreshLight(object obj)
        {
            Button btn = ((RoutedEventArgs)obj).Source as Button;
            Light light = btn.DataContext as Light;
            Light refresh = await _bridge.GetObjectAsync<Light>(light.Id);
            light.config.startup.mode = refresh.config.startup.mode;
            light.config.startup.configured = refresh.config.startup.configured;
            light.config.startup.customsettings = refresh.config.startup.customsettings;
        }

        private void SetPowerCustom(object obj)
        {
            Button btn = ((RoutedEventArgs) obj).Source as Button;
            Light light = btn.DataContext as Light;
            Form_PowerCustomSettings fcs = new Form_PowerCustomSettings(_bridge,light.config.startup.customsettings, light.Id)
            {
                Owner = Application.Current.MainWindow
            };
            bool result = (bool)fcs.ShowDialog();
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            if (_bridge == null) return;
            List<Light> temp = await _bridge.GetListObjectsAsync<Light>();
            ListLights = temp.Where(x => x.config.startup != null).ToList();
        }

        private async Task SetPowerFailure(object obj)
        {
            ComboBox cb = ((RoutedEventArgs)obj).Source as ComboBox;
            string mode = cb.SelectedValue.ToString();
            Light light = cb.DataContext as Light;
            bool result = await _bridge.SetPowerConfigAsyncTask(mode, light.Id);
            light.config.startup.configured = result;
            if (!result)
            {
                _bridge.ShowErrorMessages();
            }
            else
            {
                if (mode == "custom")
                {
                    Light refresh = await _bridge.GetObjectAsync<Light>(light.Id);
                    light.config.startup.customsettings = refresh.config.startup.customsettings;
                }
            }
        }

        public List<Light> ListLights
        {
            get => _listLights;
            set => SetProperty(ref _listLights,value);
        }

    }
}
