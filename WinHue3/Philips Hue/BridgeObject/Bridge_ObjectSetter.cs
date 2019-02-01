using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.Entertainment_API;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.Philips_Hue.BridgeObject
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
        public async Task<bool> SetStateAsyncTask(IBaseProperties state, string id)
        {
            string typename = null;
            string type = null;
            if (state is State)
            {
                typename = "lights";
                type = "state";
            }
            if (state is Action)
            {
                typename = "groups";
                type = "action";
            }
            if (typename == null) return false;
            string url = BridgeUrl + $@"/{typename}/{id}/{type}";

            CommResult comres;
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(state));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success(){Address = url, value = state.ToString()});
                return LastCommandMessages.Success;
            }


            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Set the state of a light or group on the bridge.
        /// </summary>
        /// <typeparam name="T">Any class the derives from CommonProperties</typeparam>
        /// <param name="state">New state of the object.</param>
        /// <param name="id">ID of the specified object.</param>
        /// <returns>BridgeCommResult</returns>
        public bool SetState(IBaseProperties state, string id)
        {

            string typename = null;
            string type = null;
            if (state is State)
            {
                typename = "lights";
                type = "state";
            }
            if (state is Action)
            {
                typename = "groups";
                type = "action";
            }
            if (typename == null) return false;
            string url = BridgeUrl + $@"/{typename}/{id}/{type}";

            CommResult comres;
            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(state));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = state.ToString() });
                return LastCommandMessages.Success;
            }


            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Set the Power configuration mode of the light.
        /// </summary>
        /// <param name="powermode">Power mode : powerfail or Safety</param>
        /// <param name="id">Id of the bulb</param>
        /// <returns>True or false</returns>
        public async Task<bool> SetPowerConfigAsyncTask(string powermode, string id)
        {
            CommResult comres;

            string url = BridgeUrl + $"/lights/{id}/config";
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, "{\"startup\" : {\"mode\" : \""+ powermode+ "\"}}");
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = powermode });
                return LastCommandMessages.Success;
            }

            return false;
        }

        public async Task<bool> SetPowerCustomSettingsAsyncTask(PowerCustomSettings state, string id)
        {
            CommResult comres;

            string url = BridgeUrl + $"/lights/{id}/config/";
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, "{\"startup\": { \"customsettings\" : " + Serializer.SerializeToJson(state) + "}}");
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = state.ToString() });
                return LastCommandMessages.Success;
            }

            return false;

        }

        /// <summary>
        /// Activate a scene.
        /// </summary>
        /// <param name="id">Id of the scene.</param>
        /// <returns>BridgeCommResult</returns>
        public bool ActivateScene(string id)
        {
            string url = BridgeUrl + "/groups/0/action";
            CommResult comres;

            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, "{\"scene\":\"" + id + "\"}");
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Set Virtual scene : {id}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Activate a scene async.
        /// </summary>
        /// <param name="id">Id of the scene.</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<bool> ActivateSceneAsyncTask(string id)
        {
            string url = BridgeUrl + "/groups/0/action";
            CommResult comres;

            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, "{\"scene\":\"" + id + "\"}");
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Set Virtual scene : {id}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;

        }

        /// <summary>
        /// Tell the bridge to store the current state of the lights of the scene.
        /// </summary>
        /// <param name="id">ID of the scene.</param>
        /// <returns>BrideCommResult</returns>
        public async Task<bool> StoreCurrentLightStateAsyncTask(string id)
        {
            string url = BridgeUrl + $"/scenes/{id}";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(new Scene() {storelightstate = true}));

            if (comres.Status == WebExceptionStatus.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Set the light state of a scene.
        /// </summary>
        /// <param name="sceneid">Id of the scene.</param>
        /// <param name="lightid">Id of the light.</param>
        /// <param name="state">State of the light.</param>
        /// <returns>BrideCommResult</returns>
        public bool SetSceneLightState(string sceneid, string lightid, IBaseProperties state)
        {

            string url = BridgeUrl + $"/scenes/{sceneid}/lightstates/{lightid}";
            CommResult comres;
            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(state));

                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Set Virtual scene state : {sceneid}, {lightid}, {state.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Set the light state of a scene async.
        /// </summary>
        /// <param name="sceneid">Id of the scene.</param>
        /// <param name="lightid">Id of the light.</param>
        /// <param name="state">State of the light.</param>
        /// <returns>BrideCommResult</returns>
        public async Task<bool> SetSceneLightStateAsyncTask(string sceneid, string lightid, IBaseProperties state)
        {
            string url = BridgeUrl + $"/scenes/{sceneid}/lightstates/{lightid}";
            CommResult comres;
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(state));

                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Set Virtual scene state : {sceneid}, {lightid}, {state.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Rename a specified object on the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">ID of the specified object to rename.</param>
        /// <param name="newname">New name of the object.</param>
        /// <returns>BridgeCommResult</returns>
        public bool RenameObject(IHueObject hueobj)
        {
            string typename = hueobj.GetHueType();
            string url = BridgeUrl + $@"/{typename}/{hueobj.Id}";

            CommResult comres;

            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, $@"{{""name"":""{hueobj.name}""}}");
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Rename object : {hueobj.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Rename a specified object on the bridge async.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">ID of the specified object to rename.</param>
        /// <param name="newname">New name of the object.</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<bool> RenameObjectASyncTask(IHueObject hueobj)
        {
            string typename = hueobj.GetHueType();
            string url = BridgeUrl + $@"/{typename}/{hueobj.Id}";

            CommResult comres;

            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(hueobj));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Rename Virtual object : {hueobj.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Create a new object on the bridge. Won't work for Lights.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="newobject">New object to create on the bridge.</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public bool CreateObject(IHueObject newobject)
        {
            string typename = newobject.GetHueType();
            IHueObject clone = (IHueObject)newobject.Clone();
            string url = BridgeUrl + $@"/{typename}";
            if (typename == null) return false;

            CommResult comres;

            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Post, Serializer.SerializeToJson(ClearNotAllowedCreationProperties(clone)));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Created Virtual object : {newobject.ToString()}" });
                return LastCommandMessages.Success;
            }
            
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Create a new object on the bridge async. Won't work for Lights.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="newobject">New object to create on the bridge.</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public async Task<bool> CreateObjectAsyncTask(IHueObject newobject)
        {
            string typename = newobject.GetHueType();
            IHueObject clone = (IHueObject)newobject.Clone();
            string url = BridgeUrl + $@"/{typename}";
            if (typename == null) return false;

            CommResult comres;

            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Post, Serializer.SerializeToJson(ClearNotAllowedCreationProperties(clone)));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Created Virtual object : {newobject.ToString()}" });
                return LastCommandMessages.Success;
            }

            ProcessCommandFailure(url, comres.Status);
            return false;

        }

        /// <summary>
        /// Remove the specified object from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="obj">Object to modify</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public bool RemoveObject(IHueObject obj)
        {
            string typename = obj.GetHueType();
            string url = BridgeUrl + $@"/{typename}/{obj.Id}";
            CommResult comres;

            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Delete);
                if (comres.Status == WebExceptionStatus.Success)
                {
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Deleted Virtual object : {obj.ToString()}" });
                return LastCommandMessages.Success;
            }

            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Remove the specified object from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="obj">Object to modify</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public bool RemoveObject(string Id, HueObjectType type)
        {
            string typename = type.ToString();
            string url = BridgeUrl + $@"/{typename}/{Id}";

            CommResult comres;

            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Delete);
                if (comres.Status == WebExceptionStatus.Success)
                {
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Deleted Virtual object : {Id},{type.ToString()}" });
                return LastCommandMessages.Success;
            }

            ProcessCommandFailure(url, comres.Status);
            return false;

        }

        /// <summary>
        /// Remove the specified object from the bridge async.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="obj">Object to modify</param>
        /// <returns>HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</returns>
        public async Task<bool> RemoveObjectAsyncTask(IHueObject obj)
        {
            string typename = obj.GetHueType();
            string url = BridgeUrl + $@"/{typename}/{obj.Id}";
            CommResult comres;

            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Delete);
                if (comres.Status == WebExceptionStatus.Success)
                {
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Deleted Virtual object : {obj.ToString()}" });
                return LastCommandMessages.Success;
            }

            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Modify the specified object in the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="modifiedobject">The new modified object.</param>
        /// <param name="id">Id of the object.</param>
        /// <returns>BridgeCommResult</returns>
        public bool ModifyObject(IHueObject modifiedobject)
        {
            string typename = modifiedobject.GetHueType();
            IHueObject clone = (IHueObject)modifiedobject.Clone();
            string url = BridgeUrl + $@"/{typename}/{modifiedobject.Id}";
            if (typename == null) return false;

            CommResult comres;

            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(ClearNotAllowedModifyProperties(clone)));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Modified Virtual object : {modifiedobject.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;

        }

        /// <summary>
        /// Modify the specified object in the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="modifiedobject">The new modified object.</param>
        /// <param name="id">Id of the object.</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<bool> ModifyObjectAsyncTask(IHueObject modifiedobject)
        {
            string typename = modifiedobject.GetHueType();
            IHueObject clone = (IHueObject)modifiedobject.Clone();
            string url = BridgeUrl + $@"/{typename}/{modifiedobject.Id}";
            if (typename == null) return false;

            CommResult comres;

            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(modifiedobject));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Modified Virtual object : {modifiedobject.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Change the desired sensor config.
        /// </summary>
        /// <param name="id">ID of the sensor</param>
        /// <param name="newconfig">New config of the sensor</param>
        /// <returns>BridgeCommResult</returns>
        public bool ChangeSensorConfig(string id, object newconfig)
        {
            string url = BridgeUrl + $@"/sensors/{id}/config";
            CommResult comres;

            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(ClearNotAllowedCreationProperties(newconfig)));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Modified Virtual sensor config : {id},{newconfig.ToString()}" });
                return LastCommandMessages.Success;
            }

            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Change the desired sensor config async.
        /// </summary>
        /// <param name="id">ID of the sensor</param>
        /// <param name="newconfig">New config of the sensor</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<bool> ChangeSensorConfigAsyncTask(string id, object newconfig)
        {
            string url = BridgeUrl + $@"/sensors/{id}/config";
            CommResult comres;

            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(newconfig));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Modified Virtual sensor config : {id},{newconfig.ToString()}" });
                return LastCommandMessages.Success;
            }

            ProcessCommandFailure(url, comres.Status);
            return false;
        }


        /// <summary>
        /// Change the sensor's state.
        /// </summary>
        /// <param name="id">id of the sensor</param>
        /// <param name="newstate">New state of the sensor</param>
        /// <returns>BridgeCommResult</returns>
        public bool ChangeSensorState(string id, object newstate)
        {
            string url = BridgeUrl + $@"/sensors/{id}/state";
            CommResult comres;
            if (!Virtual)
            {
                comres = Comm.SendRequest(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(ClearNotAllowedModifyProperties(newstate)));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Modified Virtual sensor state : {id},{newstate.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Change the sensor's state.
        /// </summary>
        /// <param name="id">id of the sensor</param>
        /// <param name="newstate">New state of the sensor</param>
        /// <returns>Success or error</returns>
        public async Task<bool> ChangeSensorStateAsyncTask(string id, object newstate)
        {

            string url = BridgeUrl + $@"/sensors/{id}/state";
            CommResult comres;
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(ClearNotAllowedModifyProperties(newstate)));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Modified Virtual sensor state : {id},{newstate.ToString()}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Create an entertainment Area
        /// </summary>
        /// <param name="entertain">Entertainment Area definition</param>
        /// <returns>Success or error</returns>
        public async Task<bool> CreateEntertainmentArea(Entertainment entertain)
        {
            string url = BridgeUrl + $@"/groups";
            CommResult comres;
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Post, Serializer.SerializeToJson(entertain));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Created Entertrainement Area" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Set Entertrainment Group light location
        /// </summary>
        /// <param name="id">ID of the group</param>
        /// <param name="loc">Location information</param>
        /// <returns></returns>
        public async Task<bool> SetEntertrainementLightLocation(string id, Location loc)
        {
            string url = BridgeUrl + $@"/groups/{id}";
            CommResult comres;
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.SerializeToJson(loc));
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Update Light location for group : {id}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Set Entertrainment Group stream status
        /// </summary>
        /// <param name="id">ID of the group</param>
        /// <param name="status">Status of the stream</param>
        /// <returns></returns>
        public async Task<bool> SetEntertrainementGroupStreamStatus(string id, bool status)
        {
            string url = BridgeUrl + $@"/groups/{id}";
            CommResult comres;
            if (!Virtual)
            {
                comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, $"\"stream\":{status}");
                if (comres.Status == WebExceptionStatus.Success)
                {
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Update Light location for group : {id}" });
                return LastCommandMessages.Success;
            }
            ProcessCommandFailure(url, comres.Status);
            return false;
        }

        /// <summary>
        /// Set to null all properties that are not allow to be set at modification.
        /// </summary>
        /// <param name="obj">Object to be parsed.</param>
        /// <returns></returns>
        private object ClearNotAllowedModifyProperties(object obj)
        {
            PropertyInfo[] listproperties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
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
            PropertyInfo[] listproperties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo p in listproperties)
            {
                if (Attribute.IsDefined(p, typeof(ReadOnlyAttribute)))
                    p.SetValue(obj, null);
            }

            return obj;
        }

        public async Task<bool> SendStreamPacketAsync(StreamMessage packet)
        {
            UdpClient udpClient = new UdpClient();
            IPEndPoint clientIp = new IPEndPoint(IpAddress, 2100);
 
            bool success = false;
            Exception exception = null;

            try
            {
                await udpClient.SendAsync(packet, packet.Length, clientIp);
                udpClient.Close();
                success = true;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return success;
        }

    }
    
}