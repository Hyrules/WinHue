using WinHue3.ViewModels;

namespace WinHue3.Models.AppSettings
{
    public class AppViewSettingsModel : ValidatableBindableBase
    {
        private WinHueSortOrder _sort;
        private bool _showId;
        private bool _wrap;

        public AppViewSettingsModel()
        {
            Sort = WinHueSortOrder.Default;
            ShowId = false;
            Wrap = false;
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
