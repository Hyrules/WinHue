using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using HueLib2;

namespace WinHue3
{
    public class BridgeSettingsView : View
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Bridge _br;
        private BridgeSettings _brs;
        private List<string> _listtimezones;
        private ObservableCollection<Whitelist> _listusers;
        private Whitelist _selecteduser;
        private string _appname;
        private string _devicename;

        #region CTOR

        public BridgeSettingsView(Bridge selectedbridge)
        {
            _br = selectedbridge;
            _brs = _br.GetBridgeSettings();
            _listtimezones = _br.GetTimeZones();
            _listusers = new ObservableCollection<Whitelist>(HueObjectHelper.GetBridgeUsers(_br));
            int winhue = _listusers.FindIndex(x => x.id == selectedbridge.ApiKey);
            _listusers.RemoveAt(winhue);
        }

        #endregion

        #region PROPERTIES

        #region GENERAL SETTINGS

        public string BridgeName
        {
            get
            {
                return _brs.name;
            }
            set
            {
                _brs.name = value;
                OnPropertyChanged();

            }
        }

        public List<string> BridgeTimeZoneList => _listtimezones ?? new List<string>();

        public string BridgeSelectedTimeZone
        {
            get
            {
                return _brs.timezone ?? string.Empty;
            }
            set { _brs.timezone = value; }

        } 

        public string BridgeUTC => _brs.UTC ?? string.Empty;

        public string BridgeLocalTime => _brs.localtime ?? string.Empty;

        public string BridgeSwVersion => _brs.swversion ?? string.Empty;

        public string BridgeApiVersion => _brs.apiversion ?? string.Empty;

        public string BridgeZigBeeChannel => _brs.zigbeechannel.ToString() ?? string.Empty;

        public string BridgeLinkState => (_brs.linkbutton ?? false) ? "has been pressed" : "not pressed.";

        #endregion

        #region NETWORK SETTINGS

        public string BridgeMac => _brs?.mac ?? string.Empty;

        public string BridgeIpAddress
        {
            get
            {
                return _brs?.ipaddress ?? string.Empty;
            }
            set
            {
                _brs.ipaddress = value;
                IPAddress ip;
                if (IPAddress.TryParse(value, out ip))
                {                 
                    OnPropertyChanged();
                    RemoveError(GlobalStrings.Invalid_IP);
                }
                else
                {
                    SetError(GlobalStrings.Invalid_IP);
                }

            }
        }

        public string BridgeNetmaskAddress
        {
            get
            {
                return _brs?.netmask ?? string.Empty;
            }
            set
            {
                _brs.netmask = value;
                IPAddress ip;
                if (IPAddress.TryParse(value, out ip))
                {                
                    OnPropertyChanged();
                    RemoveError(GlobalStrings.Invalid_IP);
                }
                else
                {
                    SetError(GlobalStrings.Invalid_IP);
                }
            }
        }

        public string BridgeProxyAddress
        {
            get
            {
                return _brs?.proxyaddress ?? string.Empty;
            }
            set
            {
                _brs.proxyaddress = value;
                IPAddress ip;
                if (IPAddress.TryParse(value, out ip))
                {
                    OnPropertyChanged();
                    RemoveError(GlobalStrings.Invalid_IP);
                }
                else
                {
                    SetError(GlobalStrings.Invalid_IP);
                }
            }
        }

        public string BridgeGatewayAddress
        {
            get
            {
                return _brs?.gateway ?? string.Empty;
            }
            set
            {
                _brs.gateway = value;
                IPAddress ip;
                if (IPAddress.TryParse(value, out ip))
                {

                    OnPropertyChanged();
                    RemoveError(GlobalStrings.Invalid_IP);
                }
                else
                {
                    SetError(GlobalStrings.Invalid_IP);
                }
            }
        }

        public bool BridgeEnableDHCP => !_brs.dhcp ?? true;


        public int BridgeProxyPort
        {
            get
            {
                return _brs?.proxyport ?? 0;
            }
            set
            {
                _brs.proxyport = value;
                if (value == 0 && !(bool)_brs.dhcp)
                {
                    SetError(GlobalStrings.Error_InvalidPort);
                }
                else
                {
                    RemoveError(GlobalStrings.Error_InvalidPort);
                }
            }
        }

        public bool BridgeDHCPEnable
        {
            get
            {
                return _brs?.dhcp ?? true;
            }
            set
            {
                _brs.dhcp = value;
                OnPropertyChanged();
                OnPropertyChanged("BridgeEnableDHCP");
                if (value)
                {
                    RemoveError(GlobalStrings.Error_InvalidPort, "BridgeProxyPort");
                    RemoveError(GlobalStrings.Invalid_IP, "BridgeProxyAddress");
                    RemoveError(GlobalStrings.Invalid_IP, "BridgeGatewayAddress");
                    RemoveError(GlobalStrings.Invalid_IP, "BridgeIpAddress");
                    RemoveError(GlobalStrings.Invalid_IP, "BridgeNetmaskAddress");
                }

            }
        }

        #endregion

        #region PORTAL STATUS

        public bool BridgePortalService => _brs?.portalservices ?? false;

        public string BridgeConnectionStatus => _brs?.portalconnection ?? string.Empty;     

        public string BridgeSignedOn => _brs?.portalstate?.signedon.ToString() ?? string.Empty;

        public string BridgeIncoming => _brs?.portalstate?.incoming.ToString() ?? string.Empty;

        public string BridgeOutgoing => _brs?.portalstate?.outgoing.ToString() ?? string.Empty;

        public string BridgeCommunication => _brs?.portalstate?.communication ?? string.Empty;

        #endregion

        #region SOFTWARE UPDATE INFORMATION

        public bool BridgeUpdateNotify => _brs?.swupdate?.notify ?? false;

        public string BridgeUpdateUrl => _brs?.swupdate?.url ?? string.Empty;

        public string BridgeUpdateText => _brs?.swupdate.text ?? string.Empty;

        public string BridgeUpdateState
        {
            get
            {
                if (_brs?.swupdate?.updatestate == null) return string.Empty;
                switch (_brs.swupdate.updatestate)
                {
                    case 0:
                        return GlobalStrings.NoFirmwareUpdate;
                    case 1:
                        return GlobalStrings.IncomingUpdate;
                    case 2:
                        return GlobalStrings.UpdateAvailable;
                    case 3:
                        return GlobalStrings.ApplyingUpdate;
                    default:
                        return GlobalStrings.Update_Error;
                }
            }
        }

        #endregion

        #region WHITE LIST / USERS MANAGEMENT

        public ObservableCollection<Whitelist> BridgeListUsers => _listusers ?? new ObservableCollection<Whitelist>();

        public string BridgeWhiteListLastUseDate => _selecteduser == null ? string.Empty : _selecteduser.LastUseDate;

        public string BridgeWhiteListCreateDate => _selecteduser == null ? string.Empty : _selecteduser.CreateDate;

        public bool BridgeWhiteListCanCreate => _selecteduser == null;

        public Whitelist BridgeListUsersSelectedUser
        {
            get
            {
                return _selecteduser;
            }
            set
            {
                _selecteduser = value;
                OnPropertyChanged();
                OnPropertyChanged("BridgeWhiteListAppName");
                OnPropertyChanged("BridgewhiteListDevName");
                OnPropertyChanged("BridgeWhiteListCreateDate");
                OnPropertyChanged("BridgeWhiteListLastUseDate");
                OnPropertyChanged("BridgeWhiteListUserId");
                OnPropertyChanged("CanDeleteUser");
                OnPropertyChanged("BridgeWhiteListCanCreate");
            }
        }

        public string BridgeWhiteListAppName
        {
            get
            {
                if (_selecteduser == null)
                    return _appname;
                return _selecteduser.Name.Contains("#") ? _selecteduser.Name.Split('#')[0] : _selecteduser.Name;
            }
            set
            {
                _appname = value;
                OnPropertyChanged();
            }
        }

        public string BridgeWhiteListUserId => _selecteduser == null ? string.Empty : _selecteduser.id;


        public string BridgeWhiteListDevName
        {
            get
            {
                if (_selecteduser == null)
                    return _devicename;
                return _selecteduser.Name.Contains("#") ? _selecteduser.Name.Split('#')[1] : string.Empty;
            }
            set
            {
                _devicename = value;
                OnPropertyChanged();
            }
        }

        public bool CanDeleteUser => _selecteduser != null;

        #endregion

        #endregion

        #region METHODS

        private void RemoveBridgeUser()
        {
            if (_selecteduser == null) return;
            if (!_listusers.Contains(_selecteduser)) return;
            if (!_br.RemoveUser(_selecteduser.id)) return;
            log.Info($"Removed user {_selecteduser.Name}");
            _listusers.Remove(_selecteduser);
        }

        private void ClearWhiteListFields()
        {
            BridgeListUsersSelectedUser = null;
            BridgeWhiteListAppName = string.Empty;
            BridgeWhiteListDevName = string.Empty;
        }

        private void CreateUser()
        {
            string result = _br.CreateUser($"{_appname}#{_devicename}");
            if(result == string.Empty)
            {
                MessageBoxError.ShowLastErrorMessages(_br);
            }
            else
            {
                BridgeWhiteListAppName = string.Empty;
                BridgeWhiteListDevName = string.Empty;
                BridgeListUsersSelectedUser = null;
                _listusers = new ObservableCollection<Whitelist>(HueObjectHelper.GetBridgeUsers(_br));
                OnPropertyChanged("BridgeListUsers");
            }
        }

        private void ApplyNetworkSettings()
        {
            BridgeSettings brs = new BridgeSettings();
            if (BridgeDHCPEnable)
            {
                brs.ipaddress = null;
                brs.gateway = null;
                brs.proxyport = null;
                brs.proxyaddress = null;
                brs.netmask = null;
                brs.dhcp = true;
            }
            else
            {
                brs.ipaddress = _brs.ipaddress;
                brs.gateway = _brs.gateway;
                brs.proxyport = _brs.proxyport;
                brs.proxyaddress = _brs.proxyaddress;
                brs.netmask = _brs.netmask;
                brs.dhcp = false;
            }
            if (_br.SetBridgeSettings(brs).FailureCount <= 0) return;
            MessageBoxError.ShowLastErrorMessages(_br);
        }

        private void ApplyGeneralSettings()
        {
            BridgeSettings brs = new BridgeSettings() { name = _brs.name, timezone = _brs.timezone };
            WinHueSettings.settings.BridgeInfo[_brs.mac].name = _brs.name;
            WinHueSettings.Save();
            if (_br.SetBridgeSettings(brs).FailureCount <= 0) return;
            MessageBoxError.ShowLastErrorMessages(_br);

        }

        #endregion

        #region EVENTS

        #endregion

        #region COMMANDS

        public ICommand RemoveBridgeUserCommand => new RelayCommand(param => RemoveBridgeUser());
        public ICommand ClearWhiteListFieldsCommand => new RelayCommand(param => ClearWhiteListFields());
        public ICommand CreateUserCommand => new RelayCommand(param => CreateUser());

        public ICommand ApplyNetworkSettingsCommand => new RelayCommand(param => ApplyNetworkSettings());
        public ICommand ApplyGeneralSettingsCommand => new RelayCommand(param => ApplyGeneralSettings());

        #endregion
    }
}

