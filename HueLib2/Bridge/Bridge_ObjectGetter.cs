using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HueLib2.Objects.HueObject;

namespace HueLib2
{
    public partial class Bridge
    {

        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<T> GetObject<T>(string id) where T : IHueObject
        {
            CommandResult<T> bresult = new CommandResult<T>() {Success = false};
            string ns = typeof(T).Namespace;

            if (ns != null)
            {
                
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}/{id}"),WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        MethodInfo method = typeof(Serializer).GetMethod("DeserializeToObject");
                        MethodInfo generic = method.MakeGenericMethod(typeof(T));
                        object result = generic.Invoke(this, new object[] { comres.data });
                        bresult.Data = (T)result;
                        bresult.Success = bresult.Data != null;
                        if (bresult.Data == null)
                        {
                            lastMessages = result as MessageCollection;
                        }
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() {ex = comres});
                        bresult.Exception = comres.ex;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
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
                        bresult.Success = true;
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
                        bresult.Success = true;
                        bresult.Data = Serializer.DeserializeSearchResult(comres.data);
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

            }
            else
            {
                bresult.Exception = new NullReferenceException("Type cannot be null.");
            }
            return bresult;
        }
    }
}
