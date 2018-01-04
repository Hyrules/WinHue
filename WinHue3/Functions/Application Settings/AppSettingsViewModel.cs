using System.Windows;
using System.Windows.Input;

using WinHue3.Utils;

namespace WinHue3.Functions.Application_Settings
{
    public class AppSettingsViewModel : ValidatableBindableBase
    {
        public AppViewSettingsModel _appViewSettingsModel;
        public AppDefaultsModel _appDefaultsModel;
        public AppMainSettingsModel _appMainSettingsModel;

        public AppSettingsViewModel()
        {
            _appViewSettingsModel = new AppViewSettingsModel();
            _appDefaultsModel = new AppDefaultsModel();
            _appMainSettingsModel = new AppMainSettingsModel();
        }

        public AppMainSettingsModel MainSettingsModel
        {
            get => _appMainSettingsModel;
            set => SetProperty(ref _appMainSettingsModel,value);
        }

        public AppDefaultsModel DefaultModel
        {
            get => _appDefaultsModel;
            set => SetProperty(ref _appDefaultsModel,value);
        }

        public AppViewSettingsModel ViewSettingsModel
        {
            get => _appViewSettingsModel;
            set => SetProperty(ref _appViewSettingsModel,value);
        }

        public ICommand ChangeThemeCommand => new RelayCommand(param => ChangeThemeColor());

        private void ChangeThemeColor()
        {
            MahApps.Metro.ThemeManager.ChangeAppStyle(Application.Current, MahApps.Metro.ThemeManager.GetAccent(MainSettingsModel.Themecolor), MahApps.Metro.ThemeManager.GetAppTheme(MainSettingsModel.Theme));
            //Fluent.ThemeManager.ChangeAppStyle(Application.Current, Fluent.ThemeManager.GetAccent(MainSettingsModel.Themecolor), Fluent.ThemeManager.GetAppTheme(MainSettingsModel.Theme));

        }
    }
}
