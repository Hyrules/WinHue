using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Utils;

namespace WinHue3.Functions.Application_Settings
{
    public class AppDefaultsModel : ValidatableBindableBase
    {
        private ushort? _allOnTT;
        private ushort? _allOffTT;
        private ushort? _defaultTT;
        private byte _defaultLightBri;
        private byte _defaultGroupBri;
        private bool _lastState;
        private int _slidersBehavior;

        public AppDefaultsModel()
        {
            _allOffTT = WinHueSettings.settings.AllOffTT;
            _allOnTT = WinHueSettings.settings.AllOnTT;
            _defaultTT = WinHueSettings.settings.DefaultTT;
            _defaultLightBri = WinHueSettings.settings.DefaultBriLight;
            _defaultGroupBri = WinHueSettings.settings.DefaultBriGroup;
            _lastState = WinHueSettings.settings.UseLastBriState;
            _slidersBehavior = WinHueSettings.settings.SlidersBehavior;
        }

        public ushort? AllOnTt
        {
            get => _allOnTT;
            set => SetProperty(ref _allOnTT,value);
        }

        public ushort? AllOffTt
        {
            get => _allOffTT;
            set => SetProperty(ref _allOffTT,value);
        }

        public ushort? DefaultTt
        {
            get => _defaultTT;
            set => SetProperty(ref _defaultTT,value);
        }

        public byte DefaultLightBri
        {
            get => _defaultLightBri;
            set => SetProperty(ref _defaultLightBri,value);
        }

        public byte DefaultGroupBri
        {
            get => _defaultGroupBri;
            set => SetProperty(ref _defaultGroupBri,value);
        }

        public bool UseLastBriState
        {
            get => _lastState;
            set => SetProperty(ref _lastState, value);
        }

        public int SlidersBehavior
        {
            get => _slidersBehavior;
            set => SetProperty(ref _slidersBehavior,value);
        }
    }
}
