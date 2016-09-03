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

            CommResult result = Communication.SendRequest(new Uri(BridgeUrl + $@"/" + (typeof(T) == typeof(Light) ? "lights" : "groups") + $@"/{id}/" + (typeof(T) == typeof(Light) ? "state" : "action")), WebRequestType.PUT, Serializer.SerializeToJson(state));
   
            if (result.status == WebExceptionStatus.Success)
            {
                MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(result.data));
                bresult.Success = mc.FailureCount == 0;
                bresult.resultobject = mc;
            }
            else
            {
                bresult.resultobject = result.data;
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
            
            if (comres.status == WebExceptionStatus.Success)
            {
                MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                bresult.Success = mc.FailureCount == 0;
                bresult.resultobject = mc;
            }
            else
            {
                bresult.resultobject = comres.data;
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
            if (comres.status == WebExceptionStatus.Success)
            {
                MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                bresult.Success = mc.FailureCount == 0;
                bresult.resultobject = mc;
            }
            else
            {
                bresult.resultobject = comres.data;
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

            if (comres.status == WebExceptionStatus.Success)
            {
                MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                bresult.Success = mc.FailureCount == 0;
                bresult.resultobject = mc;
            }
            else
            {
                bresult.resultobject = comres.data;
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
                CommResult result = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.PUT, Serializer.SerializeToJson(hueobj));
                

                if (result.status == WebExceptionStatus.Success)
                {
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(result.data));
                    bresult.Success = mc.FailureCount == 0;
                    bresult.resultobject = mc;
                }
                else
                {
                    bresult.resultobject = result.data;
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
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult result = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}"), WebRequestType.POST, Serializer.SerializeToJson(ClearNotAllowedCreationProperties(newobject)));
                if (result.status == WebExceptionStatus.Success)
                {
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(result.data));
                    bresult.Success = mc.FailureCount == 0;
                    bresult.resultobject = mc;
                }
                else
                {
                    bresult.resultobject = result.data;
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
                CommResult result = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.DELETE);
                if (result.status == WebExceptionStatus.Success)
                {
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(result.data));
                    bresult.Success = mc.FailureCount == 0;
                    bresult.resultobject = true;
                }
                else
                {
                    bresult.resultobject = result.data;
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
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult result = Communication.SendRequest(new Uri(BridgeUrl + $@"/{typename}/{id}"), WebRequestType.PUT,Serializer.SerializeToJson(ClearNotAllowedModifyProperties(modifiedobject)));
                if (result.status == WebExceptionStatus.Success)
                {
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(result.data));
                    bresult.Success = mc.FailureCount == 0;
                    bresult.resultobject = mc;
                }
                else
                {
                    bresult.resultobject = result.data;
                }

            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
            }
            return bresult;

        }

        /// <summary>
        /// Set to null all properties that are not allow to be set at modification.
        /// </summary>
        /// <param name="hueobject">Object to be parsed.</param>
        /// <returns></returns>
        private HueObject ClearNotAllowedModifyProperties(HueObject hueobject)
        {
            PropertyInfo[] listproperties = hueobject.GetType().GetProperties();
            foreach (PropertyInfo p in listproperties)
            {
                HueLibAttribute hla = (HueLibAttribute)Attribute.GetCustomAttribute(p, typeof(HueLibAttribute));
                if(!hla.Modify)
                    p.SetValue(hueobject,null);
            }

            return hueobject;            
        }

        /// <summary>
        ///  Set to null all properties that are not allow to be set at creation.
        /// </summary>
        /// <param name="hueobject">Object to be parsed</param>
        /// <returns></returns>
        private HueObject ClearNotAllowedCreationProperties(HueObject hueobject)
        {
            PropertyInfo[] listproperties = hueobject.GetType().GetProperties();
            foreach (PropertyInfo p in listproperties)
            {
                HueLibAttribute hla = (HueLibAttribute)Attribute.GetCustomAttribute(p, typeof(HueLibAttribute));
                if (!hla.Create)
                    p.SetValue(hueobject, null);
            }

            return hueobject;
        }
    }
}
