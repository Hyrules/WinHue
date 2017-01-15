using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models.BridgeSettings
{
    public class BridgeSettingsSoftwareModel : ValidatableBindableBase
    {
        private string _url;
        private string _text;
        private string _updatestate;
        private bool _notify;

        public BridgeSettingsSoftwareModel()
        {
            
        }

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url,value); }
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text,value); }
        }

        public string Updatestate
        {
            get { return _updatestate; }
            set { SetProperty(ref _updatestate,value); }
        }

        public bool Notify
        {
            get { return _notify; }
            set { SetProperty(ref _notify,value); }
        }
    }
}
