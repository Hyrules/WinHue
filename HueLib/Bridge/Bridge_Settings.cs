using System;
using System.Collections.Generic;
using System.Net;
using HueLib.BridgeMessages.Error;

namespace HueLib
{
    public partial class Bridge
    {

        /// <summary>
        /// Change the Name of the bridge.
        /// </summary>
        /// <param name="name">New name for the bridge</param>
        /// <returns>The BridgeSettings with the new bridge name or null if error.</returns>
        public MessageCollection ChangeBridgeName(string name)
        {

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(new BridgeSettings() { name = name }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return lastMessages;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge or null.</returns>
        public BridgeSettings GetBridgeSettings()
        {
            BridgeSettings bridgeSettings = new BridgeSettings();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    bridgeSettings = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if(bridgeSettings != null) return bridgeSettings;
                    bridgeSettings = new BridgeSettings();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg != null ? new MessageCollection(lstmsg) : new MessageCollection { new UnkownError(comres) };
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return bridgeSettings;
        }

        /// <summary>
        /// Allows the user to set some configuration values.
        /// </summary>
        /// <param name="settings">Settings of the bridge.</param>
        /// <return>The new settings of the bridge.</return>
        public MessageCollection SetBridgeSettings(BridgeSettings settings)
        {

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(settings));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return lastMessages;
        }

        /// <summary>
        /// Creates a new user / Register a new user. The link button on the bridge must be pressed and this command executed within 30 seconds.
        /// </summary>
        /// <returns>Contains a list with a single item that details whether the user was added successfully along with the username parameter. Note: If the requested username already exists then the response will report a success.</returns>
        /// <param name="DeviceType">Description of the type of device associated with this username. This field must contain the name of your app.</param>
        /// <return>The new API Key.</return>
        public string CreateUser(string DeviceType)
        {
            string apikey = string.Empty;

            CommResult comres = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() { devicetype = DeviceType }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            apikey = ((Success)lastMessages[0]).Value;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return apikey;
        }

        /// <summary>
        /// Contact the bridge to register your application and generate an APIKEY.
        /// </summary>
        /// <param name="ApplicationName">Name of your application.</param>
        /// <returns>True or false the Registration has been succesfull. This will automaically populate the ApiKey with the one generated.</returns>
        public bool RegisterApplication(string ApplicationName)
        {

            CommResult comres = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() { devicetype = ApplicationName }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            ApiKey = ((Success)lastMessages[0]).Value;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return ApiKey == string.Empty;
        }

        /// <summary>
        /// Remove a user from the whitelist.
        /// </summary>
        /// <param name="username">Username to remove</param>
        /// <returns>True or false if the user has been removed.</returns>
        public bool RemoveUser(string username)
        {
            bool result = false;

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config/whitelist/" + username), WebRequestType.DELETE);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(lstmsg));
                        if (lastMessages.SuccessCount == 1)
                            result = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return result;
        }


        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public Dictionary<string, Whitelist> GetUserList()
        {
            Dictionary<string, Whitelist> list = new Dictionary<string, Whitelist>();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if(brs != null) return brs.whitelist;
                    list = new Dictionary<string, Whitelist>();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg != null ? new MessageCollection(lstmsg) : new MessageCollection { new UnkownError(comres) };
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return list;
        }

        /// <summary>
        ///  Get all the timezones that are supported by the bridge.
        /// </summary>
        /// <returns>a list of all the timezones supported by the bridge.</returns>
        public List<string> GetTimeZones()
        {
            List<string> timezones = new List<string>();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/info/timezones"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    timezones = Serializer.DeserializeToObject<List<string>>(comres.data);
                    if(timezones != null) return timezones;
                    timezones = new List<string>();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg != null ? new MessageCollection(lstmsg) : new MessageCollection { new UnkownError(comres) };
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return timezones;

        }
    }


}
