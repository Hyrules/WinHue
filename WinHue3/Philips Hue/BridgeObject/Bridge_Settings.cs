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
        /// <summary>
        /// Get Bridge Capabilities Async
        /// </summary>
        /// <returns></returns>
        public async Task<Capabilities> GetBridgeCapabilitiesAsyncTask()
        {
            Version api = Version.Parse(ApiVersion);
            Version limit = Version.Parse("1.15.0");
            if (api < limit) return null;
            string url = BridgeUrl + "/capabilities";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                Capabilities cap = Serializer.DeserializeToObject<Capabilities>(comres.Data);
                return cap;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }
    
        /// <summary>
        /// Get Bridge capabilities
        /// </summary>
        /// <returns></returns>
        public Capabilities GetBridgeCapabilities()
        {
            string url = BridgeUrl + "/capabilities";
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                Capabilities cap = Serializer.DeserializeToObject<Capabilities>(comres.Data);
                return cap;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Try to contact the specified bridge to get the basic config.
        /// </summary>
        /// <param name="BridgeIP">IP Address of the bridge.</param>
        /// <returns>The basic configuration of the bridge.</returns>
        public BasicConfig GetBridgeBasicConfig()
        {
            string url = BridgeUrl + "/config";
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                BasicConfig config = Serializer.DeserializeToObject<BasicConfig>(comres.Data);
                return config;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;

        }

        /// <summary>
        /// Try to contact the specified bridge to get the basic config.
        /// </summary>
        /// <param name="BridgeIP">IP Address of the bridge.</param>
        /// <returns>The basic configuration of the bridge.</returns>
        public async Task<BasicConfig> GetBridgeBasicConfigAsyncTask()
        {
            string url = BridgeUrl + "/config";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                BasicConfig config = Serializer.DeserializeToObject<BasicConfig>(comres.Data);
                return config;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;

        }

        /// <summary>
        /// Change the name of the bridge.
        /// </summary>
        /// <param name="name">New name of the bridge.</param>
        /// <returns>BridgeHttpResult if the operation is successfull</returns>
        public async Task<bool> ChangeBridgeNameAsyncTask(string name)
        {
            string url = BridgeUrl + "/config";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(new BridgeSettings() { name = name }));

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Change the name of the bridge.
        /// </summary>
        /// <param name="name">New name of the bridge.</param>
        /// <returns>BridgeHttpResult if the operation is successfull</returns>
        public bool ChangeBridgeName(string name)
        {
            string url = BridgeUrl + "/config";
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(new BridgeSettings() { name = name }));

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge.</returns>
        public BridgeSettings GetBridgeSettings()
        {
            string url = BridgeUrl + "/config";
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                BridgeSettings bs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (bs != null) return bs;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge.</returns>
        public async Task<BridgeSettings> GetBridgeSettingsAsyncTask()
        {
            string url = BridgeUrl + "/config";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                BridgeSettings bs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (bs != null) return bs;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Allows the user to set some configuration values.
        /// </summary>
        /// <param name="settings">Settings of the bridge.</param>
        /// <return>The new settings of the bridge.</return>
        public async Task<bool> SetBridgeSettingsAsyncTask(BridgeSettings settings)
        {
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.Put, Serializer.ModifyJsonObject(settings));

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/config", WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Allows the user to set some configuration values.
        /// </summary>
        /// <param name="settings">Settings of the bridge.</param>
        /// <return>The new settings of the bridge.</return>
        public bool SetBridgeSettings(BridgeSettings settings)
        {
            string url = BridgeUrl + "/config";
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(settings));

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Creates a new user / Register a new user. The link button on the bridge must be pressed and this command executed within 30 seconds.
        /// </summary>
        /// <returns>Contains a list with a single item that details whether the user was added successfully along with the username parameter. Note: If the requested username already exists then the response will report a success.</returns>
        /// <param name="deviceType">Description of the type of device associated with this username. This field must contain the name of your app.</param>
        /// <return>The new API Key.</return>
        public string CreateUser(string deviceType, bool? generatesteamkey = null)
        {
            string url = "http://" + _ipAddress + "/api";
            User newuser = new User() { devicetype = deviceType, generateclientkey = generatesteamkey };
            Version current = new Version(ApiVersion);
            if(current < Version.Parse("1.22"))
            {
                newuser.generateclientkey = null;
            }

            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Post, Serializer.CreateJsonObject(newuser));

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success ? LastCommandMessages.LastSuccess.value : null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Creates a new user / Register a new user. The link button on the bridge must be pressed and this command executed within 30 seconds.
        /// </summary>
        /// <returns>Contains a list with a single item that details whether the user was added successfully along with the username parameter. Note: If the requested username already exists then the response will report a success.</returns>
        /// <param name="deviceType">Description of the type of device associated with this username. This field must contain the name of your app.</param>
        /// <return>The new API Key.</return>
        public async Task<string> CreateUserAsyncTask(string deviceType)
        {
            string url = "http://" + _ipAddress + "/api";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Post, Serializer.CreateJsonObject(new User() { devicetype = deviceType }));

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success ? LastCommandMessages.LastSuccess.value : null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Remove a user from the whitelist.
        /// </summary>
        /// <param name="username">Username to remove</param>
        /// <returns>True or false if the user has been removed.</returns>
        [Obsolete]
        public bool RemoveUser(string username)
        {

            string url = BridgeUrl + "/config/whitelist/" + username;
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Delete);

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Remove a user from the whitelist async.
        /// </summary>
        /// <param name="username">Username to remove</param>
        /// <returns>True or false if the user has been removed.</returns>
        [Obsolete]
        public async Task<bool> RemoveUserAsyncTask(string username)
        {
            string url = BridgeUrl + "/config/whitelist/" + username;
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Delete);

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public Dictionary<string, Whitelist> GetUserList()
        {
            string url = BridgeUrl + "/config";
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (brs != null)
                {
                   return brs.whitelist;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public async Task<Dictionary<string, Whitelist>> GetUserListAsyncTask()
        {
            string url = BridgeUrl + "/config";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (brs != null)
                {
                    return brs.whitelist;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        ///  Get all the timezones that are supported by the bridge.
        /// </summary>
        /// <returns>a list of all the timezones supported by the bridge.</returns>
        public List<string> GetTimeZones()
        {
            string url = BridgeUrl + "/info/timezones";
            HttpResult comres =HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                List<string> timezones = Serializer.DeserializeToObject<List<string>>(comres.Data);
                if (timezones != null)
                {
                    return timezones;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;

        }

        /// <summary>
        ///  Get all the timezones that are supported by the bridge.
        /// </summary>
        /// <returns>a list of all the timezones supported by the bridge.</returns>
        public async Task<List<string>> GetTimeZonesAsyncTask()
        {
            Version api = Version.Parse(ApiVersion);
            Version limit = Version.Parse("1.15.0");
            if (api > limit)
            {
                Capabilities cap = await GetBridgeCapabilitiesAsyncTask();
                return cap?.timezones.values;
            }
            else
            {
                HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(BridgeUrl + "/info/timezones"), WebRequestType.Get);

                if (comres.Success)
                {
                    List<string> timezones = Serializer.DeserializeToObject<List<string>>(comres.Data);
                    if (timezones != null)
                    {
                        return timezones;
                    }
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return null;
                }
                BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, BridgeUrl + "/info/timezones", WebExceptionStatus.NameResolutionFailure));
                return null;
            }

        }
    }


}
