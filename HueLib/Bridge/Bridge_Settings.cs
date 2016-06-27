using System;
using System.Collections.Generic;

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

            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(new BridgeSettings() {name = name}));
                if (!string.IsNullOrEmpty(message))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                lastMessages = new MessageCollection();
            }
            return lastMessages;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge or null.</returns>
        public BridgeSettings GetBridgeSettings()
        {
            BridgeSettings bridgeSettings;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    bridgeSettings = Serializer.DeserializeToObject<BridgeSettings>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bridgeSettings = null;
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                bridgeSettings = null;      
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(settings));
                if (!string.IsNullOrEmpty(message))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch(Exception)
            {
                lastMessages = new MessageCollection();
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
            try
            {
                string message = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() {devicetype = DeviceType}));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if(lastMessages.SuccessCount == 1)
                        apikey = ((Success) lastMessages[0]).Value;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch(Exception)
            {
                lastMessages = new MessageCollection();
                apikey = string.Empty;
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
            try
            {
                string message = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() { devicetype = ApplicationName }));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        ApiKey = ((Success)lastMessages[0]).Value;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
                
            }
            catch (Exception)
            {
                lastMessages = new MessageCollection();
                ApiKey = string.Empty;
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config/whitelist/" + username), WebRequestType.DELETE);
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));

                    if (lastMessages.SuccessCount == 1)
                        result = true;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public Dictionary<string, Whitelist> GetUserList()
        {
            Dictionary<string, Whitelist> list = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                {
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(message);
                    list = brs.whitelist;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                list = null;
            }
            return list;
        }

        /// <summary>
        ///  Get all the timezones that are supported by the bridge.
        /// </summary>
        /// <returns>a list of all the timezones supported by the bridge.</returns>
        public List<string> GetTimeZones()
        {
            List<string> timezones = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/info/timezones"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    timezones = Serializer.DeserializeToObject<List<string>>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
            }
            return timezones;

        }
    }


}
