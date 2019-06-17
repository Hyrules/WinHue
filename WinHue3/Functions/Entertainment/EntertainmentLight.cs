using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Entertainment
{
    public class EntertainmentLight : ValidatableBindableBase
    {
        private Light _light;
        decimal[] _location;

        public EntertainmentLight(Light l, decimal[] location)
        {
            Light = l;
            _location = location;

        }

        public decimal[] Location
        {
            get => _location;
            set => SetProperty(ref _location ,value);
        }

        public Light Light
        {
            get => _light;
            set => SetProperty(ref _light,value);
        }
    }
}
