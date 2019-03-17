using System;
using System.Net;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue.BridgeObject
{

    /// <summary>
    /// Bridge Class.
    /// </summary>
    public partial class Bridge : ValidatableBindableBase
    {
        private Messages _lastCommandMessages;
        private string _apiKey = string.Empty;
        private string _apiversion = string.Empty;
        private string _mac = string.Empty;
        private string _swversion;
        private bool _isdefault;
        private IPAddress _ipAddress;
        private string _name;
        private bool? _requiredUpdate;
        private bool _updateAvailable;
        private bool _virtual;

        /// <summary>
        /// Constructor
        /// </summary>
        public Bridge()
        {
            _ipAddress = IPAddress.None;
            _apiKey = string.Empty;
            _lastCommandMessages = new Messages();
            _updateAvailable = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ip">IP Address of the bridge</param>
        /// <param name="newname"></param>
        /// <param name="apiKey">[Optional] The Api to access the bridge.</param>
        /// <param name="mac"></param>
        public Bridge(IPAddress ip, string mac, string newname, string apiKey = null)
        {
            _ipAddress = ip;
            if (apiKey != null || apiKey != string.Empty)
            {
                _apiKey = apiKey;
            }
            _mac = mac;
            _name = newname;
            _lastCommandMessages = new Messages();
            _updateAvailable = false;
        }

        /// <summary>
        /// Api Key to access the bridge. If the application is not autorized the api key will not be set.
        /// </summary>
        public string ApiKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }

        /// <summary>
        /// Is the default bridge.
        /// </summary>        
        public bool IsDefault
        {
            get => _isdefault;
            set => SetProperty(ref _isdefault,value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// Software version of the bridge.
        /// </summary>
        public string SwVersion
        {
            get => _swversion;
            set => SetProperty(ref _swversion,value);
        }

        /// <summary>
        /// Mac address of the bridge.
        /// </summary>
        public string Mac
        {
            get => _mac;
            set => SetProperty(ref _mac,value);
        }

        /// <summary>
        /// Version of the bridge api
        /// </summary>
        public string ApiVersion
        {
            get => _apiversion;
            set => SetProperty(ref _apiversion,value);
        }

        /// <summary>
        /// IP Address of the bridge.
        /// </summary>
        public IPAddress IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress,value);
        }

        public string LongName => $"{Name} ({IpAddress})";

        /// <summary>
        /// Return the full url with IP Address and Api key to the bridge.
        /// </summary>
        public string BridgeUrl => _apiKey != string.Empty ? $"http://{_ipAddress }/api/{_apiKey}" : $"http://{_ipAddress}/api";

        public Messages LastCommandMessages
        {
            get => _lastCommandMessages;
            set => SetProperty(ref _lastCommandMessages,value);
        }

        public bool? RequiredUpdate
        {
            get => _requiredUpdate;
            set => SetProperty(ref _requiredUpdate,value);
        }

        public bool UpdateAvailable
        {
            get => _updateAvailable;
            set => SetProperty(ref _updateAvailable,value);
        }

        public bool Virtual
        {
            get => _virtual;
            set => SetProperty(ref _virtual,value);
        }



    }
}
