using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using WinHue3.Models.BridgeSettings;

namespace WinHue3.ViewModels
{
    public class BridgeSettingsViewModel : ValidatableBindableBase
    {
        private BridgeSettingsGeneralModel _generalModel;
        private BridgeSettingsNetworkModel _networkModel;
        private BridgeSettingsPortalModel _portalModel;
        private BridgeSettingsSoftwareModel _softwareModel;

        public BridgeSettingsViewModel()
        {
            _generalModel = new BridgeSettingsGeneralModel();
            _networkModel = new BridgeSettingsNetworkModel();
            _portalModel = new BridgeSettingsPortalModel();
            _softwareModel = new BridgeSettingsSoftwareModel();
        }

        public BridgeSettings Settings
        {
            set
            {
                BridgeSettings bs = value;

            }
        }

        public BridgeSettingsGeneralModel GeneralModel
        {
            get { return _generalModel; }
            set { SetProperty(ref _generalModel,value); }
        }

        public BridgeSettingsNetworkModel NetworkModel
        {
            get { return _networkModel; }
            set { SetProperty(ref _networkModel,value); }
        }

        public BridgeSettingsPortalModel PortalModel
        {
            get { return _portalModel; }
            set { SetProperty(ref _portalModel,value); }
        }

        public BridgeSettingsSoftwareModel SoftwareModel
        {
            get { return _softwareModel; }
            set { SetProperty(ref _softwareModel,value); }
        }
    }
}
