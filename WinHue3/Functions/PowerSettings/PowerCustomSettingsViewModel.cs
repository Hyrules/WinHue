using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.PowerSettings
{
    class PowerCustomSettingsViewModel : ValidatableBindableBase
    {
        private PowerCustomSettings _customsettings;
        
        public PowerCustomSettingsViewModel()
        {
            
        }

        public PowerCustomSettings Customsettings
        {
            get => _customsettings; 
            set => SetProperty(ref _customsettings, value);
        }
    }
}
