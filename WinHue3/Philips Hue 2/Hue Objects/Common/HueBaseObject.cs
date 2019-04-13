using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Philips_Hue_2.BridgeMessages;
using WinHue3.Philips_Hue_2.Communication;
using WinHue3.Utils;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Common
{
    public abstract class HueBaseObject : ValidatableBindableBase
    {
        private string _id;
        private string _name;
        private ImageSource _image;
        private bool _visible;
        private readonly string _apikey;
        private readonly string _ip;

        #region CTOR

        protected HueBaseObject(string id, string ip, string apikey)
        {
            Id = id;
            _apikey = apikey;
            _ip = ip;
        }

        #endregion

        #region PROPERTIES
        [Description("ID of the object"), Category("Common Properties")]
        public string Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        [JsonProperty("name"), Description("Name of the object"), Category("Common Properties")]
        public string Name
        {
            get => _name;
            protected set => SetProperty(ref _name, value);
        }

        [Browsable(false)]
        public ImageSource Image
        {
            get => _image;
            protected set => SetProperty(ref _image, value);
        }

        [Description("Visibility of the object"), Category("Common Properties")]
        public bool Visible
        {
            get => _visible; 
            set => SetProperty(ref _visible,value);
        }

        [Description("Url of the object"), Category("Common Properties")]
        protected string BaseUrl => $"http://{_ip}/api/{_apikey}";

        [Browsable(false)]
        public string Ip => _ip;

        #endregion

        #region METHODS

        public object Clone()
        {
            return MemberwiseClone();
        }

        protected static async Task<bool> RenameObject(string newname, string url)
        {
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, $@"{{""name"":""{newname}""}}");

            if (comres.Status != WebExceptionStatus.Success) return false;
            Messages msg = new Messages(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
            return msg.Success;

        }

        public async Task<T> GetObjectAsync<T>(string url) where T : HueBaseObject
        {
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                T data = Serializer.DeserializeToObject<T>(comres.Data);
                if (data != null)
                {
                    return data;
                }
                
            }
            
            return null;
        }

        public abstract Task<bool> Rename(string newname);
        public abstract Task Refresh();
        public abstract Task Toggle(bool? on = null);
        #endregion
    }
}