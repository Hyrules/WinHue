using WinHue3.ViewModels;

namespace WinHue3.Addons.Model
{
    public class AlertCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private string _description;
        private string _url;
        private bool _enable;

        public AlertCreatorModel()
        {
            Name = string.Empty;
            Description = string.Empty;
            Url = string.Empty;
            Enable = true;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description,value);
        }

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url,value);
        }

        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable,value);
        }
    }
}
