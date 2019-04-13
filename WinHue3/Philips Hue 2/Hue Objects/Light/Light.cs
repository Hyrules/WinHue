using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using WinHue3.Functions.Lights.SupportedDevices;
using WinHue3.Philips_Hue_2.Communication;
using WinHue3.Philips_Hue_2.Hue_Objects.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Comm = WinHue3.Philips_Hue.Communication.Comm;
using CommResult = WinHue3.Philips_Hue.Communication.CommResult;
using Serializer = WinHue3.Philips_Hue.Communication.Serializer;
using WebRequestType = WinHue3.Philips_Hue.Communication.WebRequestType;

namespace WinHue3.Philips_Hue_2.Hue_Objects.Light
{
    public class Light : HueBaseObject
    {
        private string _type;
        private string _manufacturername;
        private string _modelid;
        private string _swversion;
        private string _uniqueid;
        private string _luminaireuniqueid;
        private LightSwUpdate _swUpdate;
        private LightCapabilities _capabilities;
        private LightConfig _config;
        private State _state;
        private string Url => $"{BaseUrl}/lights/{Id}";

        #region CTOR

        public Light(string id, string ip, string apikey) : base(id, ip, apikey)
        {
        }

        #endregion

        #region PROPERTIES

        [JsonProperty("swupdate"), Description("Software Update"), Category("Light Properties")]
        public LightSwUpdate SwUpdate
        {
            get => _swUpdate;
            internal set => SetProperty(ref _swUpdate,value);
        }

        [JsonProperty("type"), Description("Type"), Category("Light Properties")]
        public string Type
        {
            get => _type;
            internal set => SetProperty(ref _type,value);
        }

        [JsonProperty("manufacturername"), Description("Manufacturer Name"), Category("Light Properties")]
        public string ManufacturerName
        {
            get => _manufacturername;
            internal set => SetProperty(ref _manufacturername ,value);
        }

        [JsonProperty("modelid"), Description("Model ID"), Category("Light Properties")]
        public string ModelId
        {
            get => _modelid;
            internal set => SetProperty(ref _modelid ,value);
        }

        [JsonProperty("swversion"), Description("Software Version"), Category("Light Properties")]
        public string SwVersion
        {
            get => _swversion;
            internal set => SetProperty(ref _swversion,value);
        }

        [JsonProperty("uniqueid"), Description("Unique ID"), Category("Light Properties")]
        public string UniqueId
        {
            get => _uniqueid;
            internal set => SetProperty(ref _uniqueid,value);
        }

        [JsonProperty("luminaireuniqueid"), Description("Luminaire Unique ID"), Category("Light Properties")]
        public string LuminaireUniqueId
        {
            get => _luminaireuniqueid;
            internal set => SetProperty(ref _luminaireuniqueid, value);
        }

        [JsonProperty("capabilities"), Description("Light Capabilities"), Category("Light Properties")]
        public LightCapabilities Capabilities
        {
            get => _capabilities;
            internal set => SetProperty(ref _capabilities,value);
        }
        
        [JsonProperty("state"), Description("State of the light"), Category("Light Properties"),ExpandableObject]
        public State State
        {
            get => _state;
            internal set => SetProperty(ref _state,value);
        }

        [JsonProperty("config"), Description("State of the light"), Category("Light Properties"), ExpandableObject]
        public LightConfig Config
        {
            get => _config;
            internal set => SetProperty(ref _config,value);
        }

        #endregion

        #region METHODS

        public override async Task<bool> Rename(string newname)
        {
            if (await RenameObject(newname, Url)) return false;
            Name = newname;
            return true;
        }

        public override async Task Refresh()
        {
            Light l = await GetObjectAsync<Light>(Url);
            if (l == null) return;
            Capabilities = l.Capabilities;
            ModelId = l.ModelId;
            ManufacturerName = l.ManufacturerName;
            Name = l.Name;
            UniqueId = l.UniqueId;
            LuminaireUniqueId = l.LuminaireUniqueId;
            Config = l.Config;
            SwUpdate = l.SwUpdate;
            SwVersion = l.SwVersion;
            Type = l.Type;
            State = l.State;
            RefreshImage();
        }

        private void RefreshImage()
        {
            string modelId = ModelId ?? "DefaultHUE";

            if (modelId != string.Empty && LightImageLibrary.Images.ContainsKey(modelId)) // Check model ID first
            {
                Image = LightImageLibrary.Images[modelId][State.On.GetValueOrDefault()];
            }
            else if (Config.ArcheType != null && LightImageLibrary.Images.ContainsKey(Config.ArcheType)) // Check archetype after model ID, giving model ID priority
            {
                Image = LightImageLibrary.Images[Config.ArcheType][State.On.GetValueOrDefault()];
            }
            else // Neither model ID or archetype are known
            {
                Image = LightImageLibrary.Images["DefaultHUE"][State.On.GetValueOrDefault()];
            }
            
        }

        public override async Task Toggle(bool? on = null)
        {
            bool newstate;
            if (on is null)
            {
                newstate = !State.On.GetValueOrDefault();
            }
            else
            {
                newstate = on.GetValueOrDefault();
            }
      
            CommResult cr = await Comm.SendRequestAsyncTask(new Uri($"{Url}/state"), WebRequestType.Put, Serializer.SerializeJsonObject(new State(){On = newstate}));
            if (cr.Status == WebExceptionStatus.Success)
            {
                
                State.On = newstate;
                RefreshImage();
            }
        }

        public async Task SetState(State state)
        {
            CommResult cr = await Comm.SendRequestAsyncTask(new Uri($"{Url}/state"), WebRequestType.Put, Serializer.ModifyJsonObject(state));
            if (cr.Status == WebExceptionStatus.Success)
            {

                RefreshImage();
            }
        }

        #endregion
    }
}
