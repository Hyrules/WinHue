using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using WinHue3.Models.BridgeSettings;
using System.Windows.Input;

namespace WinHue3.ViewModels
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

        public Bridge Bridge
        {
            get { return _bridge; }
            set
            {
                _bridge = value;
                CommandResult<BridgeSettings> cr = _bridge.GetBridgeSettings();
                if(cr.Success)
                {
                    //****** General Pane **********
                    BridgeSettings brs = cr.Data;
                    GeneralModel.Apiversion = brs.apiversion;
                    GeneralModel.Linkstate = brs.linkbutton == true ? GlobalStrings.Link_Pressed : GlobalStrings.Link_Not_Pressed;
                    GeneralModel.Localtime = brs.localtime;
                    GeneralModel.Name = brs.name;
                    GeneralModel.Swversion = brs.swversion;
                    GeneralModel.Zigbeechannel = brs.zigbeechannel.ToString();
                    GeneralModel.Utc = brs.UTC;

                    CommandResult<List<string>> tz = _bridge.GetTimeZones();
                    if(tz.Success)
                    {
                        GeneralModel.ListTimeZones = tz.Data;
                        GeneralModel.Timezone = brs.timezone;
                    }

                    //****** Network Pane **********
                    NetworkModel.Mac = brs.mac;
                    NetworkModel.Ip = brs.ipaddress;
                    NetworkModel.Netmask = brs.netmask;
                    NetworkModel.Gateway = brs.gateway;
                    NetworkModel.Dhcp = (bool)brs.dhcp;
                    NetworkModel.Proxy = brs.proxyaddress;
                    NetworkModel.Proxyport = (int)brs.proxyport;

                    //****** Portal Status *********

                    PortalModel.Communication = brs.portalstate.communication;
                    PortalModel.Portalservice = (bool)brs.portalservices;
                    PortalModel.Connection = brs.portalconnection;
                    PortalModel.Signedon = brs.portalstate.signedon.ToString();
                    PortalModel.Incoming = brs.portalstate.incoming.ToString();
                    PortalModel.Outgoing = brs.portalstate.outgoing.ToString();

                    //****** Software Pane *********

                    SoftwareModel.Text = brs.swupdate.text;
                    SoftwareModel.Url = brs.swupdate.url;
                    SoftwareModel.Notify = brs.swupdate.notify;
                    SoftwareModel.Updatestate = brs.swupdate.updatestate.ToString();
                    GeneralModel.AcceptChanges();
                    NetworkModel.AcceptChanges();
                }
                
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

        public ICommand ApplyGeneralSettingsCommand => new RelayCommand(param => ApplyGeneralSettings(), (param) => CanApplyGeneralSettings());

        private bool CanApplyGeneralSettings()
        {
            return GeneralModel.IsChanged;
        }

        public ICommand ApplyNetworkSettingsCommand => new RelayCommand(param => ApplyNetworkSettings(), (param) => CanApplyNetworkSettings());

        private bool CanApplyNetworkSettings()
        {
            return NetworkModel.Dhcp && NetworkModel.IsChanged;
        }

        private void ApplyNetworkSettings()
        {
            CommandResult<MessageCollection> cr = _bridge.SetBridgeSettings(new BridgeSettings()
            {
                dhcp = NetworkModel.Dhcp,
                ipaddress = NetworkModel.Ip,
                gateway = NetworkModel.Gateway,
                netmask = NetworkModel.Netmask,
                proxyaddress = NetworkModel.Proxy,
                proxyport = NetworkModel.Proxyport
            });
            if (!cr.Success)
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
                
            }else
            {
                NetworkModel.AcceptChanges();
            }
        }

        private void ApplyGeneralSettings()
        {
            CommandResult<MessageCollection> cr = _bridge.ChangeBridgeName(GeneralModel.Name);
            if (!cr.Success)
                MessageBoxError.ShowLastErrorMessages(_bridge);

            cr = _bridge.SetBridgeSettings(new BridgeSettings() { timezone = GeneralModel.Timezone });
            if (!cr.Success)
                MessageBoxError.ShowLastErrorMessages(_bridge);
        }
    }
}
