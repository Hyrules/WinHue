using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Models.AppSettings;

namespace WinHue3.ViewModels
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
            get { return _appMainSettingsModel; }
            set { SetProperty(ref _appMainSettingsModel,value); }
        }

        public AppDefaultsModel DefaultModel
        {
            get { return _appDefaultsModel; }
            set { SetProperty(ref _appDefaultsModel,value); }
        }

        public AppViewSettingsModel ViewSettingsModel
        {
            get { return _appViewSettingsModel; }
            set { SetProperty(ref _appViewSettingsModel,value); }
        }
    }
}
