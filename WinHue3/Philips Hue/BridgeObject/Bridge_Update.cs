using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.Communication2;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        public async Task<bool> SetAutoInstallAsyncTask(autoinstall autoinstall)
        {
            Version api = Version.Parse(ApiVersion);
            Version limit = Version.Parse("1.20.0");
            if (api < limit) return false;
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.Put, "{\"swupdate2\": {\"autoinstall\" : "+Serializer.SerializeJsonObject(autoinstall)+"}}");
            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));
            return false;
        }
        /// <summary>
        /// Force the bridge to check online for an update.
        /// </summary>
        /// <returns>True or false if the operation is successful (does not return if there is an update)</returns>
        public bool CheckOnlineForUpdate()
        {
            Version api = Version.Parse(ApiVersion);
            Version limit = Version.Parse("1.20.0");
            string swu = "swupdate";
            if (api > limit)
            {
                swu = swu + "2";
            }
            HttpResult comres = HueHttpClient.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.Put, "{\"" + swu + "\": {\"checkforupdate\":true}}");
            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));
            return false;
        }


        /// <summary>
        /// Force the bridge to check online for an update async.
        /// </summary>
        /// <returns>True or false if the operation is successful (does not return if there is an update)</returns>
        public async Task<bool> CheckOnlineForUpdateAsyncTask()
        {
            Version api = Version.Parse(ApiVersion);
            Version limit = Version.Parse("1.20.0");
            string swu = "swupdate";

            if (api > limit)
            {
                swu = swu + "2";
            }
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.Put, "{\"" + swu + "\": {\"checkforupdate\": true}}");
            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));

            return false;
        }

        /// <summary>
        /// Check if there is an update available on the bridge . (Does not force the bridge to check for an update)
        /// </summary>
        /// <returns>Software Update or null.</returns>
        public bool CheckUpdateAvailable()
        {
            HttpResult comres = HueHttpClient.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.Get);
            if (comres.Success)
            {
                BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (brs != null)
                {
                    return brs.swupdate.updatestate == 2;
                }
                else
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(HueHttpClient.LastJson));
                }
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Check if there is an update available on the bridge async. (Does not force the bridge to check for an update)
        /// </summary>
        /// <returns>Software Update or null.</returns>
        public async Task<bool> CheckUpdateAvailableAsyncTask()
        {
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.Get);
            if (comres.Success)
            {
                BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (brs != null)
                {
                    if (brs.swupdate.updatestate == 2)
                    {
                        UpdateAvailable = true;
                        return true;
                    }
                    return false;
                }
                else
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(HueHttpClient.LastJson));
                }
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Update the bridge firmware.
        /// </summary>
        /// <returns>True or False command sent succesfully.</returns>
        public bool UpdateBridge()
        {
            Version api = Version.Parse(ApiVersion);
            Version limit = Version.Parse("1.20.0");

            string updatestring = "";
            updatestring = api > limit ? "{\"swupdate2\": {\"install\": true}}" : "{\"swupdate\":{\"updatestate\":3}}";
            HttpResult comres = HueHttpClient.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.Put, updatestring);
            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Update the bridge firmware async.
        /// </summary>
        /// <returns>True or False command sent succesfully.</returns>
        public async Task<bool> UpdateBridgeAsyncTask()
        {
            Version api = Version.Parse(ApiVersion);
            Version limit = Version.Parse("1.20.0");

            string updatestring = "";
            updatestring = api > limit ? "{\"swupdate2\": {\"install\": true}}" : "{\"swupdate\":{\"updatestate\":3}}";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.Put, updatestring);
            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));

            return false;
        }

        /// <summary>
        /// Set notify to false once the update has been done.
        /// </summary>
        /// <returns>True or false if the operation was successful or not</returns>
        public bool SetNotify()
        {
            
            HttpResult comres = HueHttpClient.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.Put, "{\"swupdate\": {\"notify\":false}}");
            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));

            return false;
        }

        /// <summary>
        /// Set notify to false once the update has been done.
        /// </summary>
        /// <returns>True or false if the operation was successful or not</returns>
        public async Task<bool> SetNotifyAsyncTask()
        {

            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.Put, "{\"swupdate\": {\"notify\":false}}");
            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));
            return false;
        }

    }
}
