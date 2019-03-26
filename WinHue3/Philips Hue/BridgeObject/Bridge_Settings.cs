using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;

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
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                Capabilities cap = Serializer.DeserializeToObject<Capabilities>(comres.Data);
                return cap;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }
    
        /// <summary>
        /// Get Bridge capabilities
        /// </summary>
        /// <returns></returns>
        public Capabilities GetBridgeCapabilities()
        {
            string url = BridgeUrl + "/capabilities";
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                Capabilities cap = Serializer.DeserializeToObject<Capabilities>(comres.Data);
                return cap;
            }
            ProcessCommandFailure(url, comres.Status);
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
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                BasicConfig config = Serializer.DeserializeToObject<BasicConfig>(comres.Data);
                return config;
            }
            ProcessCommandFailure(url, comres.Status);
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
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                BasicConfig config = Serializer.DeserializeToObject<BasicConfig>(comres.Data);
                return config;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;

        }

        /// <summary>
        /// Change the name of the bridge.
        /// </summary>
        /// <param name="name">New name of the bridge.</param>
        /// <returns>BridgeCommResult if the operation is successfull</returns>
        public async Task<bool> ChangeBridgeNameAsyncTask(string name)
        {
            string url = BridgeUrl + "/config";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(new BridgeSettings() { name = name }));

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Change the name of the bridge.
        /// </summary>
        /// <param name="name">New name of the bridge.</param>
        /// <returns>BridgeCommResult if the operation is successfull</returns>
        public bool ChangeBridgeName(string name)
        {
            string url = BridgeUrl + "/config";
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(new BridgeSettings() { name = name }));

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge.</returns>
        public BridgeSettings GetBridgeSettings()
        {
            string url = BridgeUrl + "/config";
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                BridgeSettings bs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (bs != null) return bs;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge.</returns>
        public async Task<BridgeSettings> GetBridgeSettingsAsyncTask()
        {
            string url = BridgeUrl + "/config";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                BridgeSettings bs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (bs != null) return bs;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Allows the user to set some configuration values.
        /// </summary>
        /// <param name="settings">Settings of the bridge.</param>
        /// <return>The new settings of the bridge.</return>
        public async Task<bool> SetBridgeSettingsAsyncTask(BridgeSettings settings)
        {
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.Put, Serializer.ModifyJsonObject(settings));

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(BridgeUrl + "/config",comres.Status);
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
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(settings));

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
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

            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Post, Serializer.CreateJsonObject(newuser));

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success ? LastCommandMessages.LastSuccess.value : null;
            }
            ProcessCommandFailure(url,comres.Status);
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
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Post, Serializer.CreateJsonObject(new User() { devicetype = deviceType }));

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success ? LastCommandMessages.LastSuccess.value : null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Remove a user from the whitelist.
        /// </summary>
        /// <param name="username">Username to remove</param>
        /// <returns>True or false if the user has been removed.</returns>
        public bool RemoveUser(string username)
        {

            string url = BridgeUrl + "/config/whitelist/" + username;
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Delete);

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url,comres.Status);
            return false;
        }

        /// <summary>
        /// Remove a user from the whitelist async.
        /// </summary>
        /// <param name="username">Username to remove</param>
        /// <returns>True or false if the user has been removed.</returns>
        public async Task<bool> RemoveUserAsyncTask(string username)
        {
            string url = BridgeUrl + "/config/whitelist/" + username;
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Delete);

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public Dictionary<string, Whitelist> GetUserList()
        {
            string url = BridgeUrl + "/config";
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (brs != null)
                {
                   return brs.whitelist;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public async Task<Dictionary<string, Whitelist>> GetUserListAsyncTask()
        {
            string url = BridgeUrl + "/config";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                if (brs != null)
                {
                    return brs.whitelist;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        ///  Get all the timezones that are supported by the bridge.
        /// </summary>
        /// <returns>a list of all the timezones supported by the bridge.</returns>
        public List<string> GetTimeZones()
        {
            string url = BridgeUrl + "/info/timezones";
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                List<string> timezones = Serializer.DeserializeToObject<List<string>>(comres.Data);
                if (timezones != null)
                {
                    return timezones;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
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
                CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + "/info/timezones"), WebRequestType.Get);

                if (comres.Status == WebExceptionStatus.Success)
                {
                    List<string> timezones = Serializer.DeserializeToObject<List<string>>(comres.Data);
                    if (timezones != null)
                    {
                        return timezones;
                    }
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return null;
                }
                ProcessCommandFailure(BridgeUrl + "/info/timezones", comres.Status);
                return null;
            }

        }
    }


}
