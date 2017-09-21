using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        public async Task<bool> StartNewObjectsSearchAsyncTask(Type objecttype)
        {            
            string typename = objecttype.GetHueType();
            
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + $"/{typename}"), WebRequestType.POST);

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return true;
            }
            ProcessCommandFailure(BridgeUrl + $"/{typename}", comres.Status);
            return false;
        }

        public async Task<bool> TouchLink()
        {
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.PUT,"{\"touchlink\":true}");
            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return true;
            }
            ProcessCommandFailure(BridgeUrl + "/config", comres.Status);
            return false;
        }

        public async Task<bool> FindNewLights(string serials = null)
        {
            string data = null;

            if (serials == null)
            {

                data = "{\"deviceid\":[]}";
            }

            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + $"/lights"), WebRequestType.POST, data);

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return true;
            }
            ProcessCommandFailure(BridgeUrl + $"/lights", comres.Status);
            return false;
        }
    }
}
