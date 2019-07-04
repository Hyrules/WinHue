using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.Entertainment_API;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.Communication2;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
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
        /// <returns>BridgeHttpResult</returns>
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

            HttpResult comres;
            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(state));
                if (comres.Success)
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


            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Set the state of a light or group on the bridge.
        /// </summary>
        /// <typeparam name="T">Any class the derives from CommonProperties</typeparam>
        /// <param name="state">New state of the object.</param>
        /// <param name="id">ID of the specified object.</param>
        /// <returns>BridgeHttpResult</returns>
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

            HttpResult comres;
            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(state));
                if (comres.Success)
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


            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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
            HttpResult comres;

            string url = BridgeUrl + $"/lights/{id}/config";
            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, "{\"startup\" : {\"mode\" : \""+ powermode+ "\"}}");
                if (comres.Success)
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
            HttpResult comres;

            string url = BridgeUrl + $"/lights/{id}/config/";
            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, "{\"startup\": { \"customsettings\" : " + Serializer.SerializeJsonObject(state) + "}}");
                if (comres.Success)
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
        /// <returns>BridgeHttpResult</returns>
        public bool ActivateScene(string id)
        {
            string url = BridgeUrl + "/groups/0/action";
            HttpResult comres;

            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, "{\"scene\":\"" + id + "\"}");
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Activate a scene async.
        /// </summary>
        /// <param name="id">Id of the scene.</param>
        /// <returns>BridgeHttpResult</returns>
        public async Task<bool> ActivateSceneAsyncTask(string id)
        {
            string url = BridgeUrl + "/groups/0/action";
            HttpResult comres;

            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, "{\"scene\":\"" + id + "\"}");
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;

        }

        /// <summary>
        /// Tell the bridge to store the current state of the lights of the scene.
        /// </summary>
        /// <param name="id">ID of the scene.</param>
        /// <returns>BrideHttpResult</returns>
        public async Task<bool> StoreCurrentLightStateAsyncTask(string id)
        {
            string url = BridgeUrl + $"/scenes/{id}";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(new Scene() {storelightstate = true}));

            if (comres.Success)
            {
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return LastCommandMessages.Success;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Set the light state of a scene.
        /// </summary>
        /// <param name="sceneid">Id of the scene.</param>
        /// <param name="lightid">Id of the light.</param>
        /// <param name="state">State of the light.</param>
        /// <returns>BrideHttpResult</returns>
        public bool SetSceneLightState(string sceneid, string lightid, IBaseProperties state)
        {

            string url = BridgeUrl + $"/scenes/{sceneid}/lightstates/{lightid}";
            HttpResult comres;
            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(state));

                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Set the light state of a scene async.
        /// </summary>
        /// <param name="sceneid">Id of the scene.</param>
        /// <param name="lightid">Id of the light.</param>
        /// <param name="state">State of the light.</param>
        /// <returns>BrideHttpResult</returns>
        public async Task<bool> SetSceneLightStateAsyncTask(string sceneid, string lightid, IBaseProperties state)
        {
            string url = BridgeUrl + $"/scenes/{sceneid}/lightstates/{lightid}";
            HttpResult comres;
            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(state));

                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Rename a specified object on the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">ID of the specified object to rename.</param>
        /// <param name="newname">New name of the object.</param>
        /// <returns>BridgeHttpResult</returns>
        public bool RenameObject(IHueObject hueobj)
        {
            string typename = hueobj.GetType().Name.ToLower() + "s";
            string url = BridgeUrl + $@"/{typename}/{hueobj.Id}";

            HttpResult comres;

            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, $@"{{""name"":""{hueobj.name}""}}");
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Rename a specified object on the bridge async.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">ID of the specified object to rename.</param>
        /// <param name="newname">New name of the object.</param>
        /// <returns>BridgeHttpResult</returns>
        public async Task<bool> RenameObjectASyncTask(IHueObject hueobj)
        {
            string typename = hueobj.GetType().Name.ToLower() + "s";
            string url = BridgeUrl + $@"/{typename}/{hueobj.Id}";

            HttpResult comres;

            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(hueobj));
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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
            string typename = newobject.GetType().Name.ToLower() + "s";
            IHueObject clone = (IHueObject)newobject.Clone();
            string url = BridgeUrl + $@"/{typename}";

            HttpResult comres;

            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Post, Serializer.CreateJsonObject(clone));
                if (comres.Success)
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
            
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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
            string typename = newobject.GetType().Name.ToLower() + "s";
            IHueObject clone = (IHueObject)newobject.Clone();
            string url = BridgeUrl + $@"/{typename}";

            HttpResult comres;

            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Post, Serializer.CreateJsonObject(clone));
                if (comres.Success)
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

            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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
            string typename = obj.GetType().Name.ToLower() + "s";
            string url = BridgeUrl + $@"/{typename}/{obj.Id}";
            HttpResult comres;

            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Delete);
                if (comres.Success)
                {
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Deleted Virtual object : {obj.ToString()}" });
                return LastCommandMessages.Success;
            }

            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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

            HttpResult comres;

            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Delete);
                if (comres.Success)
                {
                    return LastCommandMessages.Success;
                }

            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Deleted Virtual object : {Id},{type.ToString()}" });
                return LastCommandMessages.Success;
            }

            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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
            string typename = obj.GetType().Name.ToLower() + "s";
            string url = BridgeUrl + $@"/{typename}/{obj.Id}";
            HttpResult comres;

            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Delete);
                if (comres.Success)
                {
                    return LastCommandMessages.Success;
                }
            }
            else
            {
                LastCommandMessages.AddMessage(new Success() { Address = url, value = $"Deleted Virtual object : {obj.ToString()}" });
                return LastCommandMessages.Success;
            }

            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Modify the specified object in the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="modifiedobject">The new modified object.</param>
        /// <param name="id">Id of the object.</param>
        /// <returns>BridgeHttpResult</returns>
        public bool ModifyObject(IHueObject modifiedobject)
        {
            string typename = modifiedobject.GetType().Name.ToLower() + "s";
            IHueObject clone = (IHueObject)modifiedobject.Clone();
            string url = BridgeUrl + $@"/{typename}/{modifiedobject.Id}";
            if (typename == null) return false;

            HttpResult comres;

            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(clone));
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;

        }

        /// <summary>
        /// Modify the specified object in the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="modifiedobject">The new modified object.</param>
        /// <param name="id">Id of the object.</param>
        /// <returns>BridgeHttpResult</returns>
        public async Task<bool> ModifyObjectAsyncTask(IHueObject modifiedobject)
        {
            string typename = modifiedobject.GetType().Name.ToLower() + "s";
            IHueObject clone = (IHueObject)modifiedobject.Clone();
            string url = BridgeUrl + $@"/{typename}/{modifiedobject.Id}";
            if (typename == null) return false;

            HttpResult comres;

            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(modifiedobject));
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Change the desired sensor config.
        /// </summary>
        /// <param name="id">ID of the sensor</param>
        /// <param name="newconfig">New config of the sensor</param>
        /// <returns>BridgeHttpResult</returns>
        public bool ChangeSensorConfig(string id, object newconfig)
        {
            string url = BridgeUrl + $@"/sensors/{id}/config";
            HttpResult comres;

            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(newconfig));
                if (comres.Success)
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

            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }

        /// <summary>
        /// Change the desired sensor config async.
        /// </summary>
        /// <param name="id">ID of the sensor</param>
        /// <param name="newconfig">New config of the sensor</param>
        /// <returns>BridgeHttpResult</returns>
        public async Task<bool> ChangeSensorConfigAsyncTask(string id, object newconfig)
        {
            string url = BridgeUrl + $@"/sensors/{id}/config";
            HttpResult comres;

            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(newconfig));
                if (comres.Success)
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

            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }


        /// <summary>
        /// Change the sensor's state.
        /// </summary>
        /// <param name="id">id of the sensor</param>
        /// <param name="newstate">New state of the sensor</param>
        /// <returns>BridgeHttpResult</returns>
        public bool ChangeSensorState(string id, object newstate)
        {
            string url = BridgeUrl + $@"/sensors/{id}/state";
            HttpResult comres;
            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(newstate));
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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
            HttpResult comres;
            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, Serializer.ModifyJsonObject(newstate));
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
        }


        /// <summary>
        /// Set Entertrainment Group light location
        /// </summary>
        /// <param name="id">ID of the group</param>
        /// <param name="loc">Location information</param>
        /// <returns></returns>
        public bool SetEntertrainementLightLocation(string id, Location loc)
        {
            string url = BridgeUrl + $@"/groups/{id}";
            HttpResult comres;
            if (!Virtual)
            {
                comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Put, Serializer.SerializeJsonObject(loc));
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
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

            HttpResult comres;
            if (!Virtual)
            {
                comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Put, "{\"stream\":{\"active\":"+ status.ToString().ToLower()+ "}}");
                if (comres.Success)
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
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return false;
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