using WinHue3.MainForm;
using WinHue3.Utils;

namespace WinHue3.Functions.Application_Settings
{
    public class AppViewSettingsModel : ValidatableBindableBase
    {
        private WinHueSortOrder _sort;
        private bool _showId;
        private bool _wrap;

        public AppViewSettingsModel()
        {
            _sort = WinHueSortOrder.Default;
            _showId = false;
            _wrap = false;
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
    }
}
