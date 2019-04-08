using WinHue3.Utils;

namespace WinHue3.Philips_Hue.HueObjects.GroupObject
{
    public class Stream : ValidatableBindableBase
    {
        private string _proxymode;
        private string _proxynode;
        private bool _active;
        private string _owner;


        public string proxymode
        {
            get => _proxymode;
            set => SetProperty(ref _proxymode ,value);
        }

        public string proxynode
        {
            get => _proxynode;
            set => SetProperty(ref _proxynode, value);
        }

        public bool active
        {
            get => _active;
            set => SetProperty(ref _active ,value);
        }

        public string owner
        {
            get => _owner;
            set => SetProperty(ref _owner ,value);
        }

        public override string ToString()
        {
            return $"Proxymode : {proxymode}, Proxynode : {proxynode}, Active : {active}, Owner : {owner}";
        }
    }
}
