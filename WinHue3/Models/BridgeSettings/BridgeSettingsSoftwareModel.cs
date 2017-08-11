using WinHue3.ViewModels;

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
            get => _url;
            set => SetProperty(ref _url,value);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text,value);
        }

        public string Updatestate
        {
            get => _updatestate;
            set => SetProperty(ref _updatestate,value);
        }

        public bool Notify
        {
            get => _notify;
            set => SetProperty(ref _notify,value);
        }
    }
}
