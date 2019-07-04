using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.Communication2;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {

        #region OBJECT LIST GETTER

        /// <summary>
        /// Get a list of specified objects from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <returns>BridgeCommResult</returns>
        public List<T> GetListObjects<T>(bool showhidden = false) where T : IHueObject
        {

            string typename = typeof(T).Name.ToLower() + "s";
            string url = BridgeUrl + $"/{typename}";
            HttpResult comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                Dictionary<string, T> data = Serializer.DeserializeToObject<Dictionary<string, T>>(comres.Data);
                if (data != null)
                {
                    List<T> listdata = data.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList();
                    if (!showhidden)
                        RemoveHiddenObjects(ref listdata, WinHueSettings.bridges.BridgeInfo[Mac].hiddenobjects);
                    return listdata;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this,url,WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Get a list of specified objects from the bridge async.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <returns>BridgeCommResult</returns>
        public async Task<List<T>> GetListObjectsAsync<T>(bool showmyhidden = false, bool getgroupzero = false ) where T : IHueObject
        {

            string typename = typeof(T).Name.ToLower() + "s";
            string url = BridgeUrl + $"/{typename}";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                Dictionary<string, T> data = Serializer.DeserializeToObject<Dictionary<string, T>>(comres.Data);
                if (data != null)
                {
                    List<T> listdata = data.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList();
                    if (!showmyhidden)
                        RemoveHiddenObjects(ref listdata, WinHueSettings.bridges.BridgeInfo[Mac].hiddenobjects);

                    if(typeof(T) == typeof(Group) && getgroupzero)
                    {
                        listdata.Add(await GetObjectAsync<T>("0"));
                    }

                    if(typeof(T) == typeof(Scene) && !WinHueSettings.settings.ShowHiddenScenes)
                    {
                        listdata.RemoveAll(x => x.GetType() == typeof(Scene) && x.name.StartsWith("HIDDEN"));
                    }

                    return data.Select(x => { x.Value.Id = x.Key; return x.Value; }).ToList();
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this,new BridgeNotRespondingEventArgs(this,url,WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Get All objects from the bridge
        /// </summary>
        /// <param name="showmyhidden">Show users hidden objects</param>
        /// <param name="getgroupzero">Show group zero</param>
        /// <returns></returns>
        public async Task<List<IHueObject>> GetAllObjectsAsync(bool showmyhidden = false, bool getgroupzero = false)
        {
            List<IHueObject> huelist = new List<IHueObject>();
            string url = BridgeUrl + $"/";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                DataStore data = Serializer.DeserializeToObject<DataStore>(comres.Data);
                if (data != null) {
                    List<IHueObject> listdata = data.ToList();
                    if (!showmyhidden)
                        RemoveHiddenObjects(ref listdata, WinHueSettings.bridges.BridgeInfo[Mac].hiddenobjects);

                    if (getgroupzero)
                        listdata.Add(await GetObjectAsync<Group>("0"));

                    if (!WinHueSettings.settings.ShowHiddenScenes)
                    {
                        listdata.RemoveAll(x => x.GetType() == typeof(Scene) && x.name.StartsWith("HIDDEN"));
                    }

                    return listdata;
                }
                
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));

            return huelist;
        }

        public List<IHueObject> GetAllObjects(bool showhidden = false, bool getgroupzero = false)
        {
            List<IHueObject> huelist = new List<IHueObject>();
            string url = BridgeUrl + $"/";
            HttpResult comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                DataStore data = Serializer.DeserializeToObject<DataStore>(comres.Data);
                if (data != null)
                {
                    List<IHueObject> listdata = data.ToList();
                    if (!showhidden)
                        RemoveHiddenObjects(ref listdata, WinHueSettings.bridges.BridgeInfo[Mac].hiddenobjects);

                    if (getgroupzero)
                        listdata.Add(GetObject<Group>("0"));

                    if (!WinHueSettings.settings.ShowHiddenScenes)
                    {
                        listdata.RemoveAll(x => x.GetType() == typeof(Scene) && x.name.StartsWith("HIDDEN"));
                    }

                    return listdata.ToList();
                }

                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));

            return huelist;
        }

        #endregion

        #region OBJECT GETTER

        /// <summary>
        /// Get the specified object freom the bridge in async.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<T> GetObjectAsync<T>(string id) where T : IHueObject
        {

            string typename = typeof(T).Name.ToLower() + "s";
            if (typename == string.Empty || typename == "s") return default(T);
            string url = BridgeUrl + $"/{typename}/{id}";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url),WebRequestType.Get);

            if (comres.Success)
            {
                T data = Serializer.DeserializeToObject<T>(comres.Data);
                if (data != null)
                {
                    data.Id = id;
                    return data;
                }
                
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return default(T);
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return default(T);
        }

        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public T GetObject<T>(string id) where T : IHueObject
        {

            string typename = typeof(T).Name.ToLower() + "s";
            if (typename == string.Empty || typename == "s") return default(T);
            string url = BridgeUrl + $"/{typename}/{id}";
            HttpResult comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                T data = Serializer.DeserializeToObject<T>(comres.Data);
                if (data != null)
                {
                    data.Id = id;
                    return data;
                }
                
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return default;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return default;
        }


        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <param name="objecttype">Type of the object</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<IHueObject> GetObjectAsync(string id, Type objecttype)
        {
            string typename = objecttype.Name.ToLower() + "s";
            if (typename == string.Empty || typename == "s") return null;
            string url = BridgeUrl + $"/{typename}/{id}";
            HttpResult comres =  await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                IHueObject data = (IHueObject) Serializer.DeserializeToObject(comres.Data,objecttype);
                if (data != null)
                {
                    data.Id = id;
                    return data;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }


        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <param name="objecttype">Type of the object.</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<T> GetObjectAsync<T>(string id, Type objecttype) where T : IHueObject
        {

            string typename = typeof(T).Name.ToLower() + "s";
            if (typename == string.Empty || typename == "s") return default(T);
            string url = BridgeUrl + $"/{typename}/{id}";
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                T data = Serializer.DeserializeToObject<T>(comres.Data);
                if (data != null)
                {
                    data.Id = id;
                    return data;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return default(T);
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return default(T);
        }


        #endregion

        #region NEW OBJECT GETTER

        /// <summary>
        /// Get the newly detected lights or sensors. This will not work on other HueObject Types.
        /// </summary>
        /// <typeparam name="T">Type of the object to detect.</typeparam>
        /// <returns>BridgeCommResult</returns>
        public SearchResult GetNewObjects<T>() where T : IHueObject
        {
            string typename = typeof(T).Name.ToLower() + "s";
            string url = BridgeUrl + $"/{typename}";

            HttpResult comres = HueHttpClient.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Success)
            {
                SearchResult sr = Serializer.DeserializeToObject<SearchResult>(comres.Data);
                if (sr != null) return sr;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Get the newly detected lights or sensors async. This will not work on other HueObject Types.
        /// </summary>
        /// <typeparam name="T">Type of the object to detect.</typeparam>
        /// <returns>BridgeCommResult</returns>
        public async Task<SearchResult> GetNewObjectsAsync<T>() where T : IHueObject
        {
            string typename = typeof(T).Name.ToLower() + "s";
            string url = BridgeUrl + $"/{typename}";

            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url + "/new"), WebRequestType.Get);

            if (comres.Success)
            {
                SearchResult sr = Serializer.DeserializeToObject<SearchResult>(comres.Data);
                if (sr != null) return sr;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        #endregion
    }
}
