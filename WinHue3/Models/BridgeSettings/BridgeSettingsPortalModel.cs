using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models.BridgeSettings
{
    public class BridgeSettingsPortalModel : ValidatableBindableBase
    {
        private bool _portalservice;
        private string _connection;
        private string _signedon;
        private string _outgoing;
        private string _incoming;
        private string _communication;

        public BridgeSettingsPortalModel()
        {
            
        }

        public bool Portalservice
        {
            get { return _portalservice; }
            set { SetProperty(ref _portalservice,value); }
        }

        public string Connection
        {
            get { return _connection; }
            set { SetProperty(ref _connection,value); }
        }

        public string Signedon
        {
            get { return _signedon; }
            set { SetProperty(ref _signedon,value); }
        }

        public string Outgoing
        {
            get { return _outgoing; }
            set { SetProperty(ref _outgoing,value); }
        }

        public string Incoming
        {
            get { return _incoming; }
            set { SetProperty(ref _incoming,value); }
        }

        public string Communication
        {
            get { return _communication; }
            set { SetProperty(ref _communication,value); }
        }
    }
}
