using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using HueLib2.BridgeMessages;
using Newtonsoft.Json;

namespace HueLib2
{
    /// <summary>
    /// Bridge Class.
    /// </summary>
    public partial class Bridge : INotifyPropertyChanged
    {
        
        private string _apiKey = string.Empty;
        readonly EventArgs _e = null;
        Messages _lastmessages;
        private string _apiversion = string.Empty;
        private string _mac = string.Empty;
        private string _swversion;
        private bool _isdefault = false;
        private IPAddress _ipAddress;
        private string _name;
        private readonly Error _bridgeNotResponding;

        /// <summary>
        /// Api Key to access the bridge. If the application is not autorized the api key will not be set.
        /// </summary>
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                _apiKey = value;
                OnPropertyChanged();
            }
        }

        public string LongName => $@"{_name} ({_ipAddress})";

        /// <summary>
        /// Is the default bridge.
        /// </summary>        
        public bool IsDefault
        {
            get { return _isdefault; }
            set { _isdefault = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Software version of the bridge.
        /// </summary>
        public string SwVersion
        {
            get { return _swversion; }
            set { _swversion = value; OnPropertyChanged();}
        }

        /// <summary>
        /// Mac address of the bridge.
        /// </summary>
        public string Mac
        {
            get { return _mac; }
            set
            {
                _mac = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Version of the bridge api
        /// </summary>
        public string ApiVersion
        {
            get { return _apiversion; }
            set { _apiversion = value; OnPropertyChanged();}
        }



        /// <summary>
        /// IP Address of the bridge.
        /// </summary>
        public IPAddress IpAddress
        {
            get { return _ipAddress; }
            set
            {
                _ipAddress = value;
                OnPropertyChanged();
                _bridgeNotResponding.address = _ipAddress.ToString();
            }
        }

        /// <summary>
        /// Last JSON string sent to the bridge.
        /// </summary>
        public string lastJson => Communication.lastjson;

        public Messages lastMessages
        {
            get 
            { 
                return _lastmessages; 
            }
            set 
            {
                _lastmessages = value;
                OnMessageAdded?.Invoke(this, _e);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Return the full url with IP Address and Api key to the bridge.
        /// </summary>
        public string BridgeUrl => _apiKey != string.Empty ? $"http://{_ipAddress }/api/{_apiKey}" : $"http://{_ipAddress}/api";

        /// <summary>
        /// Constructor
        /// </summary>
        public Bridge()
        {
            _ipAddress = IPAddress.None;
            _apiKey = string.Empty;
            _lastmessages = new Messages();
            _bridgeNotResponding = new Error() { address = $"{_ipAddress}", description = "Bridge is not responding", type = -999 };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ip">IP Address of the bridge</param>
        /// <param name="apiKey">[Optional] The Api to access the bridge.</param>
        public Bridge(IPAddress ip, string mac, string apiversion, string swversion,string name, string apiKey = null)
        {
            _ipAddress = ip;
            if (apiKey != "")
            {
                _apiKey = apiKey;
            }
            _mac = mac;
            _apiversion = apiversion;
            _swversion = swversion;
            _name = name;
            OnPropertyChanged("Mac");
            OnPropertyChanged("ApiVersion");
            _bridgeNotResponding = new Error() { address = $"{_ipAddress}", description = "Bridge is not responding", type = -999 };
        }

        /// <summary>
        /// Send a raw json command to the bridge without altering the bridge lastmessages
        /// </summary>
        /// <param name="url">url to send the command to.</param>
        /// <param name="data">raw json data string</param>
        /// <param name="type">type of command.</param>
        /// <returns>json test resulting of the command.</returns>
        public CommResult SendRawCommand(string url,string data, WebRequestType type)
        {
            CommResult comres = Communication.SendRequest(new Uri(url), type, data);
            return comres;
        }

        /// <summary>
        /// Send a raw json to the bridge updating the bridge last messages.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public CommandResult<string> SendCommand(string url, string data, WebRequestType type)
        {
            CommResult comres = Communication.SendRequest(new Uri(url), type, data);
            CommandResult<string> bresult = new CommandResult<string>() { Success = false };

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = type != WebRequestType.GET ? Serializer.DeserializeToObject<Messages>(comres.data) : new Messages();
                    bresult.Success = lastMessages.Success;
                    if (type == WebRequestType.GET)
                    {
                        dynamic json = JsonConvert.DeserializeObject(comres.data);
                        bresult.Data = JsonConvert.SerializeObject(json, Formatting.Indented);
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages();
                    lastMessages.ListMessages.Add(new Error(){ address = url, description = "A Timeout occured." ,type = 65535});
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new Messages();
                    lastMessages.ListMessages.Add(new Error() { address = url, description = "A unknown error occured.", type = 65535 });
                    bresult.Exception = comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Get all objects from the bridge.
        /// </summary>
        /// <returns>A DataStore of objects from the bridge.</returns>
        public CommandResult<DataStore> GetBridgeDataStore()
        {
            DataStore listObjets = new DataStore();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    listObjets = Serializer.DeserializeToObject<DataStore>(comres.data);
                    if (listObjets != null) return new CommandResult<DataStore>() {Success = true, Data = listObjets};
                    lastMessages = Serializer.DeserializeToObject<Messages>(Communication.lastjson);
                    
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages();
                    lastMessages.ListMessages.Add(new Error() { address = BridgeUrl, description = "A Timeout occured." });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new Messages();
                    lastMessages.ListMessages.Add(new Error() { address = BridgeUrl, description = "A unknown error occured." });
                    break;
            }

            return new CommandResult<DataStore>() {Success = false};
        }

        public bool CheckAuthorization()
        {
            bool authorization = false;
            if (ApiKey == string.Empty) return false;
            CommandResult<BridgeSettings> cr = GetBridgeSettings();
            if (!cr.Success) return false;
            BridgeSettings settings = cr.Data;
            if (settings.portalservices != null)
            {
                authorization = true;                     
            }

            return authorization;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public override string ToString()
        {
            // return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
            return $@"ipaddress : {IpAddress}, IsDefault : {IsDefault}, SwVersion : {SwVersion}, Mac : {Mac}, ApiVersion : {ApiVersion}, ApiKey : {ApiKey}, BridgeUrl : {BridgeUrl} ";
        }


    }

    public class BridgeNotRespondingEventArgs : EventArgs
    {
        public CommResult ex;
    }
}
