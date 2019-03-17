using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.MainForm;
using WinHue3.Utils;

namespace WinHue3.Functions.Application_Settings
{
    public class AppViewSettingsModel : ValidatableBindableBase
    {
        private WinHueSortOrder _sort;
        private bool _showId;
        private bool _wrap;
        private bool _showFloorPlanTab;

        public AppViewSettingsModel()
        {
            _sort = WinHueSettings.settings.Sort;
            _showId = WinHueSettings.settings.ShowID;
            _wrap = WinHueSettings.settings.WrapText;
            _showFloorPlanTab = WinHueSettings.settings.ShowFloorPlanTab;
        }

        public WinHueSortOrder Sort
        {
            get => _sort;
            set => SetProperty(ref _sort,value);
        }

        public bool ShowId
        {
            get => _showId;
            set => SetProperty(ref _showId,value);
        }

        public bool Wrap
        {
            get => _wrap;
            set => SetProperty(ref _wrap,value);
        }

        public bool ShowFloorPlanTab
        {
            get => _showFloorPlanTab;
            set => SetProperty(ref _showFloorPlanTab,value);
        }
    }
}
