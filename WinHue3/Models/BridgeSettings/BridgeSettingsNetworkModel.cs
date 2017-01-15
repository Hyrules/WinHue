using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        }

        public string Mac
        {
            get { return _mac; }
            set { SetProperty(ref _mac,value); }
        }

        public bool Dhcp
        {
            get { return _dhcp; }
            set { SetProperty(ref _dhcp,value); }
        }

        public string Ip
        {
            get { return _ip; }
            set { SetProperty(ref _ip,value); }
        }

        public string Netmask
        {
            get { return _netmask; }
            set { SetProperty(ref _netmask,value); }
        }

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
