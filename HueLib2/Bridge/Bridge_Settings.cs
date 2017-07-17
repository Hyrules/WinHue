using System;
using System.Collections.Generic;
using System.Net;
using HueLib2.BridgeMessages;

namespace HueLib2
{
    public partial class Bridge
    {

        /// <summary>
        /// Change the name of the bridge.
        /// </summary>
        /// <param name="name">New name of the bridge.</param>
        /// <returns>BridgeCommResult if the operation is successfull</returns>
        public CommandResult<MessageCollection> ChangeBridgeName(string name)
        {
            CommandResult<MessageCollection> bresult = new CommandResult<MessageCollection> { Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(new BridgeSettings() { name = name }));
           
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                    lastMessages = mc;
                    if(mc.FailureCount == 0)
                    {
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }
            bresult.Data= lastMessages;
            return bresult;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge.</returns>
        public CommandResult<BridgeSettings> GetBridgeSettings()
        {
            
            CommandResult<BridgeSettings> bresult = new CommandResult<BridgeSettings> {Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);
           
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    BridgeSettings bridgeSettings = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if (bridgeSettings != null)
                    {
                        bresult.Data= bridgeSettings;
                        bresult.Success = true;
                    }
                    else
                    {
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));  
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }
            
            return bresult;
        }

        /// <summary>
        /// Allows the user to set some configuration values.
        /// </summary>
        /// <param name="settings">Settings of the bridge.</param>
        /// <return>The new settings of the bridge.</return>
        public CommandResult<MessageCollection> SetBridgeSettings(BridgeSettings settings)
        {
            CommandResult<MessageCollection> bresult = new CommandResult<MessageCollection> { Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(settings));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                    lastMessages = mc;
                    if (mc.FailureCount ==0)
                    {
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }
            bresult.Data = lastMessages;
            return bresult;
        }

        /// <summary>
        /// Creates a new user / Register a new user. The link button on the bridge must be pressed and this command executed within 30 seconds.
        /// </summary>
        /// <returns>Contains a list with a single item that details whether the user was added successfully along with the username parameter. Note: If the requested username already exists then the response will report a success.</returns>
        /// <param name="DeviceType">Description of the type of device associated with this username. This field must contain the name of your app.</param>
        /// <return>The new API Key.</return>
        public CommandResult<string> CreateUser(string DeviceType)
        {
            string apikey = string.Empty;
            CommandResult<string> bresult = new CommandResult<string> {Success = false};
            CommResult comres = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() { devicetype = DeviceType }));
            
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                    lastMessages = mc;
                    if (mc.FailureCount == 0)
                    {                       
                        apikey = ((Success) lastMessages[0]).Value;
                        bresult.Data = apikey;
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Contact the bridge to register your application and generate an APIKEY.
        /// </summary>
        /// <param name="ApplicationName">Name of your application.</param>
        /// <returns>True or false the Registration has been succesfull. This will automaically populate the ApiKey with the one generated.</returns>
        public CommandResult<string> RegisterApplication(string ApplicationName)
        {
            CommandResult<string> bresult = new CommandResult<string> { Success = false };
            CommResult comres = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() { devicetype = ApplicationName }));
  
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<IMessage> msg = Serializer.DeserializeToObject<List<IMessage>>(comres.data);

                    MessageCollection mc = new MessageCollection();
                    lastMessages = mc;
                    if (mc.FailureCount == 0)
                    {
                        ApiKey = ((Success) lastMessages[0]).Value;
                        bresult.Data = ApiKey;
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Remove a user from the whitelist.
        /// </summary>
        /// <param name="username">Username to remove</param>
        /// <returns>True or false if the user has been removed.</returns>
        public CommandResult<bool> RemoveUser(string username)
        {
            
            CommandResult<bool> bresult = new CommandResult<bool> { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config/whitelist/" + username), WebRequestType.DELETE);
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                    lastMessages = mc;
                    if (mc.FailureCount == 0)
                    {
                        bresult.Data = true;
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }
            return bresult;
        }


        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public CommandResult<Dictionary<string, Whitelist>> GetUserList()
        {
            Dictionary<string, Whitelist> list = new Dictionary<string, Whitelist>();
            CommandResult<Dictionary<string, Whitelist>> bresult = new CommandResult<Dictionary<string, Whitelist>> { Success = false };

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if (brs != null)
                    {
                        bresult.Success = true;
                        bresult.Data = brs.whitelist;                        
                    }
                    else
                    {
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        ///  Get all the timezones that are supported by the bridge.
        /// </summary>
        /// <returns>a list of all the timezones supported by the bridge.</returns>
        public CommandResult<List<string>> GetTimeZones()
        {
            
            CommandResult<List<string>> bresult = new CommandResult<List<string>> { Success = false };

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/info/timezones"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<string> timezones = Serializer.DeserializeToObject<List<string>>(comres.data);
                    if (timezones != null)
                    {
                        bresult.Success = true;
                        bresult.Data = timezones;
                    }
                    else
                    {                     
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                    }

                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Exception = comres.ex;
                    break;
            }

            return bresult;

        }
    }


}
