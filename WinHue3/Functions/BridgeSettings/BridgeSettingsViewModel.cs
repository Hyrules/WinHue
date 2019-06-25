using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Functions.BridgeSettings
{
    public class BridgeSettingsViewModel : ValidatableBindableBase
    {
        private BridgeSettingsGeneralModel _generalModel;
        private BridgeSettingsNetworkModel _networkModel;
        private BridgeSettingsPortalModel _portalModel;
        private BridgeSettingsSoftwareModel _softwareModel;
        private BridgeSettingsHiddenObjects _hiddenObjects;

        private Capabilities _caps;
        private bool _canAutoInstall;
        private bool _canClose;
        private readonly DispatcherTimer _updateTimer;
        private readonly DispatcherTimer _updateProgressTimer;

        private int _updateProgress;

        private Bridge _bridge;

        public BridgeSettingsViewModel()
        {
            _generalModel = new BridgeSettingsGeneralModel();
            _networkModel = new BridgeSettingsNetworkModel();
            _portalModel = new BridgeSettingsPortalModel();
            _softwareModel = new BridgeSettingsSoftwareModel();
            _hiddenObjects = new BridgeSettingsHiddenObjects();

            
            _updateTimer = new DispatcherTimer(){ Interval = new TimeSpan(0, 0, 180)};
            _updateProgressTimer = new DispatcherTimer() { Interval = new TimeSpan(0,0,1)};
            _updateProgressTimer.Tick += _updateProgressTimer_Tick;
            _updateTimer.Tick += _updateTimer_Tick;
            UpdateProgress = 0;
            CanClose = true;
        }
        /*
        private async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            List<IHueObject> lo = await _bridge.GetAllObjectsAsync(true);
            HiddenObjects.ListObjects = new ObservableCollection<IHueObject>(lo);
            foreach (Tuple<string,string> t in WinHueSettings.bridges.BridgeInfo[_bridge.Mac].hiddenobjects)
            {
                if (HiddenObjects.ListObjects.Any(x => x.Id == t.Item1 && x.GetType().Name == t.Item2))
                {
                    IHueObject obj = HiddenObjects.ListObjects.FirstOrDefault(x => x.Id == t.Item1 && x.GetType().Name == t.Item2);
                    HiddenObjects.ListObjects.Remove(obj);
                    HiddenObjects.HiddenObjects.Add(obj);
                }   
            }

            HiddenObjects.AcceptChanges();
        }
        */
        public ICommand SaveHiddenObjectsCommand => new RelayCommand(param => SaveHiddenObjects(), (param) => CanSaveHiddenObjects());

        private bool CanSaveHiddenObjects()
        {
            return HiddenObjects.IsChanged;
        }

        private void SaveHiddenObjects()
        {
            WinHueSettings.bridges.BridgeInfo[_bridge.Mac].hiddenobjects.Clear();
            foreach (IHueObject l in HiddenObjects.HiddenObjects)
            {
                if (!WinHueSettings.bridges.BridgeInfo[_bridge.Mac].hiddenobjects.Any(x => x.Item1 == l.Id && x.Item2 == l.GetType().Name))
                {
                    WinHueSettings.bridges.BridgeInfo[_bridge.Mac].hiddenobjects.Add(new Tuple<string, string>(l.Id, l.GetType().Name));
                }
                
            }

            WinHueSettings.SaveBridges();
            HiddenObjects.AcceptChanges();
        }


        private void _updateProgressTimer_Tick(object sender, EventArgs e)
        {
            UpdateProgress++;
        }

        private void _updateTimer_Tick(object sender, EventArgs e)
        {
            _updateTimer.Stop();
            _updateProgressTimer.Stop();
            CanClose = true;
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

        public ICommand ForceCheckUpdateCommand => new AsyncRelayCommand(param => ForceCheckUpdate(), param => CanCheckForUpdate());
        public ICommand ApplyUpdateSettingsCommand => new AsyncRelayCommand(param => ApplyUpdateSettings());
        public ICommand UpdateBridgeFirmwareCommand => new AsyncRelayCommand(param => UpdateBridgeFirmware(), param => CanUpdateFirmware());
        public ICommand InitializeCommand => new AsyncRelayCommand(param => Initialize(_bridge));

        private bool CanCheckForUpdate()
        {
            if (SoftwareModel.Updatestate == "2" || SoftwareModel.Updatestate == "3") return false;
            if (SoftwareModel.Updatestate == "installing" || SoftwareModel.Updatestate == "anyreadytoinstall" ||
                SoftwareModel.Updatestate == "allreadytoinstall") return false;
            return true;
        }


        private bool CanUpdateFirmware()
        {
            if (SoftwareModel.Updatestate == "2") return true;
            return SoftwareModel.Updatestate != "noupdates" && 
                   SoftwareModel.Updatestate != "notupdatable" && 
                   SoftwareModel.Updatestate != "unknown" && 
                   SoftwareModel.Updatestate != "transferring" && 
                   SoftwareModel.Updatestate != "installing";
        }

        private async Task UpdateBridgeFirmware()
        {
            if (MessageBox.Show(GlobalStrings.Update_Confirmation, GlobalStrings.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            
            bool result = await _bridge.UpdateBridgeAsyncTask();
            if (result)
            {
                CanClose = false;
                UpdateProgress = 0;
                _updateProgressTimer.Start();
                _updateTimer.Start();
            }
            else
            {
                _bridge.ShowErrorMessages();
            }

        }

        private async Task ApplyUpdateSettings()
        {
            bool result = await _bridge.SetAutoInstallAsyncTask(new autoinstall()
            {
                on = SoftwareModel.AutoUpdate,
                updatetime = SoftwareModel.UpdateTime.ToString("\\THH:mm:ss")
            });
            if (result) return;
            _bridge.ShowErrorMessages();
        }

        private async Task ForceCheckUpdate()
        {
            bool result = await _bridge.CheckOnlineForUpdateAsyncTask();
            if (result)
            {
                MessageBox.Show(GlobalStrings.CheckingForUpdate, GlobalStrings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _bridge.ShowErrorMessages();
            }

        }

        public ICommand ApplyGeneralSettingsCommand => new AsyncRelayCommand(param => ApplyGeneralSettings(), param => CanApplyGeneralSettings());

        private bool CanApplyGeneralSettings()
        {
            return GeneralModel.IsChanged;
        }

        public ICommand ApplyNetworkSettingsCommand => new AsyncRelayCommand(param => ApplyNetworkSettings(), param => CanApplyNetworkSettings());

        public Capabilities Capabilities
        {
            get => _caps;
            set => SetProperty(ref _caps,value);
        }

        public bool CanAutoInstall
        {
            get => _canAutoInstall;
            set => SetProperty(ref _canAutoInstall,value);
        }

        public bool CanClose
        {
            get => _canClose;
            set => SetProperty(ref _canClose,value);
        }

        public int UpdateProgress
        {
            get => _updateProgress;
            set => SetProperty(ref  _updateProgress, value);
        }

        public BridgeSettingsHiddenObjects HiddenObjects
        {
            get => _hiddenObjects;
            set => SetProperty(ref _hiddenObjects,value);
        }

        private bool CanApplyNetworkSettings()
        {
            return NetworkModel.IsChanged;
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
                CanAutoInstall = cr.swupdate2 != null;
                SoftwareModel.AutoUpdate = cr.swupdate2?.autoinstall.@on ?? false;
                SoftwareModel.Updatestate = cr.swupdate2 != null ? cr.swupdate2.state : cr.swupdate.updatestate.ToString();
                SoftwareModel.UpdateTime = cr.swupdate2 != null ? DateTime.Parse(cr.swupdate2.autoinstall.updatetime.Replace("T","")) : DateTime.Parse("00:00:00");
                SoftwareModel.LastChange = cr.swupdate2 != null ? cr.swupdate2.lastchange : string.Empty;
                SoftwareModel.LastUpdate = cr.swupdate2 != null ? cr.swupdate2.bridge.lastinstall : string.Empty;
                GeneralModel.AcceptChanges();
                NetworkModel.AcceptChanges();
            }

            Capabilities = await _bridge.GetBridgeCapabilitiesAsyncTask();

        }
        
    }
    
}
