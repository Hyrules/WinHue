using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HueLib2.BridgeMessages;
using HueLib2.Objects.HueObject;
using HueLib2.Objects.Interfaces;

namespace HueLib2
{
    public partial class Bridge
    {

        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<T> GetObject<T>(string id) where T : IHueObject
        {
            CommandResult<T> bresult = new CommandResult<T>() {Success = false};
            string ns = typeof(T).Namespace;

            if (ns != null)
            {
                
                //string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                
                HueType ht = typeof(T).GetCustomAttribute<HueType>();
                string typename = ht?.HueObjectType;
                if (ht == null) return bresult;

                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}/{id}"),WebRequestType.GET);
               
                switch (comres.status)
                {
                    case WebExceptionStatus.Success:

                        bresult.Data = Serializer.DeserializeToObject<T>(comres.data);
                        bresult.Success = bresult.Data != null;
                        if (bresult.Data == null)
                        {
                            lastMessages = new Messages(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                        }
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages();
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}/{id}", description = "A Timeout occured.", type = 65535 });
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() {ex = comres});

                        bresult.Exception = comres.ex;
                        break;
                    default:
                        lastMessages = new Messages(); 
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}/{id}", description = "An unkown error occured.", type = 65535 });
                        bresult.Exception = comres.ex;
                        break;
                }
            }
            else
            {
                bresult.Success = false;
                bresult.Exception = new NullReferenceException("Type cannot be null.");
            }

            return bresult;
        }

        /// <summary>
        /// Get a list of specified objects from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<Dictionary<string, T>> GetListObjects<T>() where T : IHueObject
        {
            CommandResult<Dictionary<string,T>> bresult = new CommandResult<Dictionary<string, T>>() {Success = false};
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
 
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}"),WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        bresult.Data = Serializer.DeserializeToObject<Dictionary<string, T>>(comres.data);
                        bresult.Success = bresult.Data != null;
                        if (bresult.Data == null)
                        {
                            lastMessages = new Messages(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                        }
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages();
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}", description = "A Timeout occured.", type = 65535 });
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.Exception = comres.ex;
                        break;
                    default:
                        lastMessages = new Messages();
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}", description = "An unkown error occured.", type = 65535 });
                        bresult.Exception = comres.ex;
                        break;
                }

            }
            else
            {
                bresult.Exception = new NullReferenceException("Type cannot be null.");
            }
            return bresult;
        }

        /// <summary>
        /// Get the newly detected lights or sensors. This will not work on other HueObject Types.
        /// </summary>
        /// <typeparam name="T">Type of the object to detect.</typeparam>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<SearchResult> GetNewObjects<T>() where T : IHueObject
        {
            CommandResult<SearchResult> bresult = new CommandResult<SearchResult>() {Success = false};
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}/new"), WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        bresult.Data = Serializer.DeserializeSearchResult(comres.data);
                        bresult.Success = bresult.Data != null;
                        if (bresult.Data == null)
                        {
                            lastMessages = new Messages(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                        }
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages();
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}/new", description = "A Timeout occured.", type = 65535 });
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.Exception = comres.ex;
                        break;
                    default:
                        lastMessages = new Messages();
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}/new", description = "An unkown error occured.", type = 65535 });
                        bresult.Exception = comres.ex;
                        break;
                }

            }
            else
            {
                bresult.Exception = new NullReferenceException("Type cannot be null.");
            }
            return bresult;
        }
    }
}
