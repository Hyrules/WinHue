using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Validation;

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
            get { return _mac; }
            set { SetProperty(ref _mac,value); }
        }

        public bool Dhcp
        {
            get { return _dhcp; }
            set { SetProperty(ref _dhcp,value); RaisePropertyChanged("EnableDHCPControls"); }
        }

        public bool EnableDHCPControls => !_dhcp;

        [RequireIPValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Invalid_IP")]
        public string Ip
        {
            get { return _ip; }
            set { SetProperty(ref _ip,value); }
        }

        [RequireIPValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Invalid_IP")]
        public string Netmask
        {
            get { return _netmask; }
            set { SetProperty(ref _netmask,value); }
        }

        [RequireIPValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Invalid_IP")]
        public string Gateway
        {
            get { return _gateway; }
            set { SetProperty(ref _gateway,value); }
        }

        public string Proxy
        {
            get { return _proxy; }
            set { SetProperty(ref _proxy,value); }
        }

        public int Proxyport
        {
            get { return _proxyport; }
            set { SetProperty(ref _proxyport,value); }
        }
    }
}
