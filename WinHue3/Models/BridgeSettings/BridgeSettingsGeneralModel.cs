using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models.BridgeSettings
{
    public class BridgeSettingsGeneralModel : ValidatableBindableBase
    {
        private string _name;
        private List<string> _listTimeZones;
        private string _timezone;
        private string _utc;
        private string _localtime;
        private string _swversion;
        private string _zigbeechannel;
        private string _apiversion;
        private string _linkstate;

        public BridgeSettingsGeneralModel()
        {
            _listTimeZones = new List<string>();
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name,value); }
        }

        public List<string> ListTimeZones
        {
            get { return _listTimeZones; }
            set { SetProperty(ref _listTimeZones,value); }
        }

        public string Timezone
        {
            get { return _timezone; }
            set { SetProperty(ref _timezone,value); }
        }

        public string Utc
        {
            get { return _utc; }
            set { SetProperty(ref _utc,value); }
        }

        public string Localtime
        {
            get { return _localtime; }
            set { SetProperty(ref _localtime,value); }
        }

        public string Swversion
        {
            get { return _swversion; }
            set { SetProperty(ref _swversion,value); }
        }

        public string Zigbeechannel
        {
            get { return _zigbeechannel; }
            set { SetProperty(ref _zigbeechannel,value); }
        }

        public string Apiversion
        {
            get { return _apiversion; }
            set { SetProperty(ref _apiversion,value); }
        }

        public string Linkstate
        {
            get { return _linkstate; }
            set { SetProperty(ref _linkstate,value); }
        }
    }
}
