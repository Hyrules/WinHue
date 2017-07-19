using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HueLib2.BridgeMessages;
using HueLib2.Objects.HueObject;

namespace HueLib2
{
    public partial class Bridge
    {
        /// <summary>
        /// Set the state of a light or group on the bridge.
        /// </summary>
        /// <typeparam name="T">Any class the derives from CommonProperties</typeparam>
        /// <param name="state">New state of the object.</param>
        /// <param name="id">ID of the specified object.</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<Messages> SetState<T>(CommonProperties state, string id) where T : IHueObject
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/" + (typeof(T) == typeof(Light) ? "lights" : "groups") + $@"/{id}/" + (typeof(T) == typeof(Light) ? "state" : "action")), WebRequestType.PUT, Serializer.SerializeToJson(state));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                    bresult.Success = lastMessages.AllSuccess;
                    bresult.Data = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages(); // TODO: Add Message
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception= comres.ex;
                    break;
                default:
                    lastMessages = new Messages(); // TODO: Add Message
                    bresult.Exception= comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Activate a scene.
        /// </summary>
        /// <param name="id">Id of the scene.</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<Messages> ActivateScene(string id)
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/0/action"), WebRequestType.PUT, "{\"scene\":\"" + id + "\"}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                    bresult.Success = lastMessages.AllSuccess;
                    bresult.Data = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages(); // TODO: Add Message
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception= comres.ex;
                    break;
                default:
                    lastMessages = new Messages(); // TODO: Add Message
                    bresult.Exception= comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Tell the bridge to store the current state of the lights of the scene.
        /// </summary>
        /// <param name="id">ID of the scene.</param>
        /// <returns>BrideCommResult</returns>
        public CommandResult<Messages> StoreCurrentLightState(string id)
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/scenes/{id}"), WebRequestType.PUT, Serializer.SerializeToJson(new Scene() { storelightstate = true }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                    bresult.Success = lastMessages.AllSuccess;
                    bresult.Data = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages(); // TODO: Add Message
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception= comres.ex;
                    break;
                default:
                    lastMessages = new Messages(); // TODO: Add Message
                    bresult.Exception= comres.ex;
                    break;
            }
            return bresult;
        }

        /// <summary>
        /// Set the light state of a scene.
        /// </summary>
        /// <param name="sceneid">Id of the scene.</param>
        /// <param name="lightid">Id of the light.</param>
        /// <param name="state">State of the light.</param>
        /// <returns>BrideCommResult</returns>
        public CommandResult<Messages> SetSceneLightState(string sceneid, string lightid, CommonProperties state)
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/scenes/{sceneid}/lightstates/{lightid}"), WebRequestType.PUT, Serializer.SerializeToJson(state));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                    bresult.Success = lastMessages.AllSuccess;
                    bresult.Data = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages(); // TODO: Add Message
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception= comres.ex;
                    break;
                default:
                    lastMessages = new Messages(); // TODO: Add Message
                    bresult.Exception= comres.ex;
                    break;
            }

            return bresult;

        }

        /// <summary>
        /// Rename a specified object on the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">ID of the specified object to rename.</param>
        /// <param name="newname">New name of the object.</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<Messages> RenameObject<T>(string id, string newname) where T : IHueObject, new()
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };
            T hueobj = new T();
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                PropertyInfo pi = hueobj.GetType().GetProperty("name");
                pi.SetValue(hueobj, newname);
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.PUT, Serializer.SerializeToJson(hueobj));

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                        bresult.Success = lastMessages.AllSuccess;
                        bresult.Data = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages(); // TODO: Add Message
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.Exception= comres.ex;
                        break;
                    default:
                        lastMessages = new Messages(); // TODO: Add Message
                        bresult.Exception= comres.ex;
                        break;
                }
            }
            else
            {
                bresult.Exception = new NullReferenceException("Type of object cannot be null");
            }

            return bresult;

        }

        /// <summary>
        /// Create a new object on the bridge. Won't work for Lights.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="newobject">New object to create on the bridge.</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public CommandResult<Messages> CreateObject<T>(T newobject) where T : IHueObject
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };
            T nobject = newobject;
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}"), WebRequestType.POST, Serializer.SerializeToJson(ClearNotAllowedCreationProperties(nobject)));
                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                        bresult.Success = lastMessages.AllSuccess;
                        bresult.Data = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages(); // TODO: Add Message
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.Exception= comres.ex;
                        break;
                    default:
                        lastMessages = new Messages(); // TODO: Add Message
                        bresult.Exception= comres.ex;
                        break;
                }
            }
            else
            {
                bresult.Exception = new NullReferenceException("Type of object cannot be null");
            }
            return bresult;
        }

        /// <summary>
        /// Remove the specified object from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">Id of the object.</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public CommandResult<Messages> RemoveObject<T>(string id) where T : IHueObject
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.DELETE);
                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                        bresult.Success = lastMessages.AllSuccess;
                        bresult.Data = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages(); // TODO: Add Message
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.Exception= comres.ex;
                        break;
                    default:
                        lastMessages = new Messages(); // TODO: Add Message
                        bresult.Exception= comres.ex;
                        break;
                }

            }
            else
            {
                bresult.Exception = new NullReferenceException("Type of object cannot be null");
            }
            return bresult;
        }

        /// <summary>
        /// Modify the specified object in the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="modifiedobject">The new modified object.</param>
        /// <param name="id">Id of the object.</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<Messages> ModifyObject<T>(T modifiedobject, string id) where T : IHueObject
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() { Success = false };
            T mobject = modifiedobject;
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.PUT, Serializer.SerializeToJson(ClearNotAllowedModifyProperties(mobject)));
                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                        bresult.Success = lastMessages.AllSuccess;
                        bresult.Data = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages(); // TODO: Add Message
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.Exception= comres.ex;
                        break;
                    default:
                        lastMessages = new Messages(); // TODO: Add Message
                        bresult.Exception= comres.ex;
                        break;
                }

            }
            else
            {
                bresult.Exception = new NullReferenceException("Type of object cannot be null");
            }
            return bresult;

        }

        /// <summary>
        /// Change the desired sensor config.
        /// </summary>
        /// <param name="id">ID of the sensor</param>
        /// <param name="newconfig">New config of the sensor</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<Messages> ChangeSensorConfig(string id, SensorConfig newconfig)
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>();
            SensorConfig sconfig = newconfig;
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/sensors/{id}/config"), WebRequestType.PUT, Serializer.SerializeToJson(ClearNotAllowedModifyProperties(sconfig)));
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                    bresult.Success = lastMessages.AllSuccess;
                    bresult.Data = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages(); // TODO: Add Message
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception= comres.ex;
                    break;
                default:
                    lastMessages = new Messages(); // TODO: Add Message
                    bresult.Exception= comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Change the sensor's state.
        /// </summary>
        /// <param name="id">id of the sensor</param>
        /// <param name="newstate">New state of the sensor</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult<Messages> ChangeSensorState(string id, SensorState newstate)
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>();
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/sensors/{id}/state"), WebRequestType.PUT, Serializer.SerializeToJson(newstate));
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = Serializer.DeserializeToObject<Messages>(comres.data);
                    bresult.Success = lastMessages.AllSuccess;
                    bresult.Data = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new Messages(); // TODO: Add Message
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Exception= comres.ex;
                    break;
                default:
                    lastMessages = new Messages(); // TODO: Add Message
                    bresult.Exception= comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Set to null all properties that are not allow to be set at modification.
        /// </summary>
        /// <param name="obj">Object to be parsed.</param>
        /// <returns></returns>
        private object ClearNotAllowedModifyProperties(object obj)
        {
            PropertyInfo[] listproperties = obj.GetType().GetProperties();
            foreach (PropertyInfo p in listproperties)
            {
                if (Attribute.IsDefined(p, typeof(CreateOnlyAttribute)) || Attribute.IsDefined(p, typeof(ReadOnlyAttribute)))
                    p.SetValue(obj, null);
            }

            return obj;
        }

        /// <summary>
        ///  Set to null all properties that are not allow to be set at creation.
        /// </summary>
        /// <param name="hueobject">Object to be parsed</param>
        /// <returns></returns>
        private object ClearNotAllowedCreationProperties(object obj)
        {
            PropertyInfo[] listproperties = obj.GetType().GetProperties();
            foreach (PropertyInfo p in listproperties)
            {
                if (Attribute.IsDefined(p, typeof(ReadOnlyAttribute)))
                    p.SetValue(obj, null);
            }

            return obj;
        }
    }
}