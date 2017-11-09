using WinHue3.Validation;
using WinHue3.ViewModels;

namespace WinHue3.Models.BridgeSettings
{
    public class BridgeSettingsNetworkModel : ValidatableBindableBase
    {
        private string _mac;
        private bool _dhcp;
        private string _ip;
        private string _netmask;
        private string _gateway;
        private string _proxy;
        private int _proxyport;


        public BridgeSettingsNetworkModel()
        {
            Dhcp = false;
        }

        public string Mac
        {
            get => _mac;
            set => SetProperty(ref _mac,value);
        }

        public bool Dhcp
        {
            get => _dhcp;
            set { SetProperty(ref _dhcp,value); RaisePropertyChanged("EnableDHCPControls"); }
        }

        public bool EnableDHCPControls => !_dhcp;

        [RequireIPValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Invalid_IP")]
        public string Ip
        {
            get => _ip;
            set => SetProperty(ref _ip,value);
        }

        [RequireIPValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Invalid_IP")]
        public string Netmask
        {
            get => _netmask;
            set => SetProperty(ref _netmask,value);
        }

        [RequireIPValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Invalid_IP")]
        public string Gateway
        {
            get => _gateway;
            set => SetProperty(ref _gateway,value);
        }

        public string Proxy
        {
            get => _proxy;
            set => SetProperty(ref _proxy,value);
        }

        public int Proxyport
        {
            get => _proxyport;
            set => SetProperty(ref _proxyport,value);
        }
    }
}
