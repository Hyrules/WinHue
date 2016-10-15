using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        public CommandResult SetState<T>(CommonProperties state,string id) where T : HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
 
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/" + (typeof(T) == typeof(Light) ? "lights" : "groups") + $@"/{id}/" + (typeof(T) == typeof(Light) ? "state" : "action")), WebRequestType.PUT, Serializer.SerializeToJson(state));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    bresult.Success = lastMessages.FailureCount == 0;
                    bresult.resultobject = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bresult.resultobject = comres.data;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.resultobject = comres.data;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Activate a scene.
        /// </summary>
        /// <param name="id">Id of the scene.</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult ActivateScene(string id)
        {
            CommandResult bresult = new CommandResult() { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/0/action"), WebRequestType.PUT,"{\"scene\":\"" + id + "\"}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    bresult.Success = lastMessages.FailureCount == 0;
                    bresult.resultobject = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bresult.resultobject = comres.data;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.resultobject = comres.data;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Tell the bridge to store the current state of the lights of the scene.
        /// </summary>
        /// <param name="id">ID of the scene.</param>
        /// <returns>BrideCommResult</returns>
        public CommandResult StoreCurrentLightState(string id)
        {
            CommandResult bresult = new CommandResult() {Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/scenes/{id}"), WebRequestType.PUT, Serializer.SerializeToJson(new Scene() {storelightstate = true}));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    bresult.Success = lastMessages.FailureCount == 0;
                    bresult.resultobject = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bresult.resultobject = comres.data;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.resultobject = comres.data;
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
        public CommandResult SetSceneLightState(string sceneid,string lightid, CommonProperties state) 
        {
            CommandResult bresult = new CommandResult() {Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/scenes/{sceneid}/lightstates/{lightid}/state"),WebRequestType.PUT, Serializer.SerializeToJson(state));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    bresult.Success = lastMessages.FailureCount == 0;
                    bresult.resultobject = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bresult.resultobject = comres.data;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.resultobject = comres.data;
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
        public CommandResult RenameObject<T>(string id, string newname) where T : HueObject, new()
        {
            CommandResult bresult = new CommandResult() {Success = false};
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
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Success = lastMessages.FailureCount == 0;
                        bresult.resultobject = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, _e);
                        bresult.resultobject = comres.data;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = comres.data;
                        break;
                }
            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
            }

            return bresult;

        }

        /// <summary>
        /// Create a new object on the bridge. Won't work for Lights.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="newobject">New object to create on the bridge.</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public CommandResult CreateObject<T>(T newobject) where T : HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
            T nobject = newobject;
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}"), WebRequestType.POST, Serializer.SerializeToJson(ClearNotAllowedCreationProperties(nobject)));
                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Success = lastMessages.FailureCount == 0;
                        bresult.resultobject = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, _e);
                        bresult.resultobject = comres.data;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = comres.data;
                        break;
                }
            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
            }
            return bresult;
        }

        /// <summary>
        /// Remove the specified object from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">Id of the object.</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public CommandResult RemoveObject<T>(string id) where T : HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.DELETE);
                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Success = lastMessages.FailureCount == 0;
                        bresult.resultobject = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, _e);
                        bresult.resultobject = comres.data;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = comres.data;
                        break;
                }

            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
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
        public CommandResult ModifyObject<T>(T modifiedobject,string id) where T : HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
            T mobject = modifiedobject;
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.PUT,Serializer.SerializeToJson(ClearNotAllowedModifyProperties(mobject)));
                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Success = lastMessages.FailureCount == 0;
                        bresult.resultobject = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, _e);
                        bresult.resultobject = comres.data;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = comres.data;
                        break;
                }

            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
            }
            return bresult;

        }

        /// <summary>
        /// Change the desired sensor config.
        /// </summary>
        /// <param name="id">ID of the sensor</param>
        /// <param name="newconfig">New config of the sensor</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult ChangeSensorConfig(string id, SensorConfig newconfig)
        {
            CommandResult bresult = new CommandResult();
            SensorConfig sconfig = newconfig;
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/sensors/{id}/config"),WebRequestType.PUT, Serializer.SerializeToJson(ClearNotAllowedModifyProperties(sconfig)));
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    bresult.Success = lastMessages.FailureCount == 0;
                    bresult.resultobject = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bresult.resultobject = comres.data;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.resultobject = comres.data;
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
        public CommandResult ChangeSensorState(string id, SensorState newstate)
        {
            CommandResult bresult = new CommandResult();
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/sensors/{id}/state"),WebRequestType.PUT, Serializer.SerializeToJson(newstate));
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    bresult.Success = lastMessages.FailureCount == 0;
                    bresult.resultobject = lastMessages;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bresult.resultobject = comres.data;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.resultobject = comres.data;
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
            string json = Serializer.SerializeToJson(obj);
            MethodInfo mi = typeof(Serializer).GetMethod("DeserializeToObject");
            MethodInfo generic = mi.MakeGenericMethod(obj.GetType());
            object newobj = generic.Invoke(obj, new object[] { json });
            PropertyInfo[] listproperties = newobj.GetType().GetProperties();
            foreach (PropertyInfo p in listproperties)
            {
                if (!Attribute.IsDefined(p, typeof(HueLibAttribute))) continue;
                HueLibAttribute hla = (HueLibAttribute) Attribute.GetCustomAttribute(p, typeof(HueLibAttribute));
                if (!hla.Modify)
                    p.SetValue(newobj, null);
            }

            return newobj;            
        }

        /// <summary>
        ///  Set to null all properties that are not allow to be set at creation.
        /// </summary>
        /// <param name="hueobject">Object to be parsed</param>
        /// <returns></returns>
        private object ClearNotAllowedCreationProperties(object obj)
        {
            string json = Serializer.SerializeToJson(obj);
            MethodInfo mi = typeof(Serializer).GetMethod("DeserializeToObject");
            MethodInfo generic = mi.MakeGenericMethod(obj.GetType());
            object newobj = generic.Invoke(obj, new object[] { json });
            PropertyInfo[] listproperties = newobj.GetType().GetProperties();
            foreach (PropertyInfo p in listproperties)
            {
                if (!Attribute.IsDefined(p, typeof(HueLibAttribute))) continue;
                HueLibAttribute hla = (HueLibAttribute)Attribute.GetCustomAttribute(p, typeof(HueLibAttribute));
                if (!hla.Create)
                    p.SetValue(newobj, null);
            }

            return newobj;
        }
    }
}
