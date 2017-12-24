using WinHue3.Utils;

namespace WinHue3.Functions.Lights.Finder
{
    public class AddLightSerialViewModel : ValidatableBindableBase
    {
        private string _listSerials;

        public AddLightSerialViewModel()
        {
            _listSerials = string.Empty;
        }

        public string ListSerials { get => _listSerials; set => SetProperty(ref _listSerials,value); }
    }
}
