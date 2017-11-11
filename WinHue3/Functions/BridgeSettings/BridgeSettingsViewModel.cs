using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Utils;

namespace WinHue3.Functions.BridgeSettings
{
    public class BridgeSettingsViewModel : ValidatableBindableBase
    {
        private BridgeSettingsGeneralModel _generalModel;
        private BridgeSettingsNetworkModel _networkModel;
        private BridgeSettingsPortalModel _portalModel;
        private BridgeSettingsSoftwareModel _softwareModel;
        private Bridge _bridge;

        public BridgeSettingsViewModel()
        {
            _generalModel = new BridgeSettingsGeneralModel();
            _networkModel = new BridgeSettingsNetworkModel();
            _portalModel = new BridgeSettingsPortalModel();
            _softwareModel = new BridgeSettingsSoftwareModel();
        }

        public BridgeSettingsGeneralModel GeneralModel
        {
            get => _generalModel;
            set => SetProperty(ref _generalModel,value);
        }

        public BridgeSettingsNetworkModel NetworkModel
        {
            get => _networkModel;
            set => SetProperty(ref _networkModel,value);
        }

        public BridgeSettingsPortalModel PortalModel
        {
            get => _portalModel;
            set => SetProperty(ref _portalModel,value);
        }

        public BridgeSettingsSoftwareModel SoftwareModel
        {
            get => _softwareModel;
            set => SetProperty(ref _softwareModel,value);
        }

        public ICommand ForceCheckUpdateCommand => new AsyncRelayCommand(param => ForceCheckUpdate());

        private async Task ForceCheckUpdate()
        {
            bool result = await _bridge.CheckOnlineForUpdateAsyncTask();
            MessageBox.Show(result.ToString());

        }

        public ICommand ApplyGeneralSettingsCommand => new AsyncRelayCommand(param => ApplyGeneralSettings(), (param) => CanApplyGeneralSettings());

        private bool CanApplyGeneralSettings()
        {
            return GeneralModel.IsChanged;
        }

        public ICommand ApplyNetworkSettingsCommand => new AsyncRelayCommand(param => ApplyNetworkSettings(), (param) => CanApplyNetworkSettings());

        private bool CanApplyNetworkSettings()
        {
            return NetworkModel.Dhcp && NetworkModel.IsChanged;
        }

        private async Task ApplyNetworkSettings()
        {
            bool cr =  await _bridge.SetBridgeSettingsAsyncTask(new Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings()
            {
                dhcp = NetworkModel.Dhcp,
                ipaddress = NetworkModel.Ip,
                gateway = NetworkModel.Gateway,
                netmask = NetworkModel.Netmask,
                proxyaddress = NetworkModel.Proxy,
                proxyport = NetworkModel.Proxyport
            });

            if (!cr)
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
                
            }else
            {
                NetworkModel.AcceptChanges();
            }
        }

        private async Task ApplyGeneralSettings()
        {
            bool cr = await _bridge.ChangeBridgeNameAsyncTask(GeneralModel.Name);
            if(!cr)
                MessageBoxError.ShowLastErrorMessages(_bridge);

            cr = await _bridge.SetBridgeSettingsAsyncTask(new Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings() { timezone = GeneralModel.Timezone });
            if (!cr)
                MessageBoxError.ShowLastErrorMessages(_bridge);
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            Philips_Hue.BridgeObject.BridgeObjects.BridgeSettings cr = await _bridge.GetBridgeSettingsAsyncTask();
            if (cr != null)
            {
                //****** General Pane **********
                GeneralModel.Apiversion = cr.apiversion;
                GeneralModel.Linkstate = cr.linkbutton == true ? GlobalStrings.Link_Pressed : GlobalStrings.Link_Not_Pressed;
                GeneralModel.Localtime = cr.localtime;
                GeneralModel.Name = cr.name;
                GeneralModel.Swversion = cr.swversion;
                GeneralModel.Zigbeechannel = cr.zigbeechannel.ToString();
                GeneralModel.Utc = cr.UTC;

                List<string> tz = await _bridge.GetTimeZonesAsyncTask();
                if (tz != null)
                {
                    GeneralModel.ListTimeZones = tz;
                    GeneralModel.Timezone = cr.timezone;
                }

                //****** Network Pane **********
                NetworkModel.Mac = cr.mac;
                NetworkModel.Ip = cr.ipaddress;
                NetworkModel.Netmask = cr.netmask;
                NetworkModel.Gateway = cr.gateway;
                NetworkModel.Dhcp = (bool)cr.dhcp;
                NetworkModel.Proxy = cr.proxyaddress;
                NetworkModel.Proxyport = (int)cr.proxyport;

                //****** Portal Status *********

                PortalModel.Communication = cr.portalstate.communication;
                PortalModel.Portalservice = (bool)cr.portalservices;
                PortalModel.Connection = cr.portalconnection;
                PortalModel.Signedon = cr.portalstate.signedon.ToString();
                PortalModel.Incoming = cr.portalstate.incoming.ToString();
                PortalModel.Outgoing = cr.portalstate.outgoing.ToString();

                //****** Software Pane *********

                SoftwareModel.Text = cr.swupdate.text;
                SoftwareModel.Url = cr.swupdate.url;
                SoftwareModel.Notify = cr.swupdate.notify;
                SoftwareModel.Updatestate = cr.swupdate.updatestate.ToString();
                GeneralModel.AcceptChanges();
                NetworkModel.AcceptChanges();
            }

        }
    }
    
}
