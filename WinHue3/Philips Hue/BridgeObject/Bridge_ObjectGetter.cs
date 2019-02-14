using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        public async Task<List<IHueObject>> GetAllObjectsAsync()
        {
            List<IHueObject> huelist = new List<IHueObject>();
            string url = BridgeUrl + $"/";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                T data = Serializer.DeserializeToObject<DataStore>(comres.Data);
                if (data != null) return data;
                data.Id = id;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return default(T);
            }
            ProcessCommandFailure(url, comres.Status);

            return huelist;
        }


        /// <summary>
        /// Get the specified object freom the bridge in async.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<T> GetObjectAsync<T>(string id) where T : IHueObject
        {

            string typename = typeof(T).GetHueType();
            if (typename == null) return default(T);
            string url = BridgeUrl + $"/{typename}/{id}";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url),WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                T data = Serializer.DeserializeToObject<T>(comres.Data);
                if (data != null) return data;
                data.Id = id;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return default(T);
            }
            ProcessCommandFailure(url, comres.Status);
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

            string typename = typeof(T).GetHueType();
            if (typename == null) return default(T);
            string url = BridgeUrl + $"/{typename}/{id}";
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                T data = Serializer.DeserializeToObject<T>(comres.Data);
                if (data != null) return data;
                data.Id = id;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return default(T);
            }
            ProcessCommandFailure(url, comres.Status);
            return default(T);
        }

        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public IHueObject GetObject(string id, Type objecttype) 
        {

            string typename = objecttype.GetHueType();
            if (typename == null) return null;
            string url = BridgeUrl + $"/{typename}/{id}";
            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                IHueObject data = (IHueObject)Serializer.DeserializeToObject(comres.Data,objecttype);
                if (data != null) return data;
                data.Id = id;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<IHueObject> GetObjectAsync(string id, Type objecttype)
        {

            string typename = objecttype.GetHueType();
            if (typename == null) return null;
            string url = BridgeUrl + $"/{typename}/{id}";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                IHueObject data = (IHueObject)Serializer.DeserializeToObject(comres.Data, objecttype);
                if (data != null) return data;
                data.Id = id;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public async Task<T> GetObjectAsync<T>(string id, Type objecttype) where T : IHueObject
        {

            string typename = objecttype.GetHueType();
            if (typename == null) return default(T);
            string url = BridgeUrl + $"/{typename}/{id}";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                T data = Serializer.DeserializeToObject<T>(comres.Data);
                if (data != null) return data;
                data.Id = id;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return default(T);
            }
            ProcessCommandFailure(url, comres.Status);
            return default(T);
        }

        /// <summary>
        /// Get a list of specified objects from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <returns>BridgeCommResult</returns>
        public Dictionary<string, T> GetListObjects<T>() where T : IHueObject
        {

            string typename = typeof(T).GetHueType();
            string url = BridgeUrl + $"/{typename}";
            CommResult comres = Comm.SendRequest(new Uri(url),WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                Dictionary<string, T> data = Serializer.DeserializeToObject<Dictionary<string, T>>(comres.Data);
                if (data != null) return data;
                foreach(KeyValuePair<string,T> t in data)
                {
                    t.Value.Id = t.Key;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url,comres.Status);
            return null;
        }

        /// <summary>
        /// Get a list of specified objects from the bridge async.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <returns>BridgeCommResult</returns>
        public async Task<List<T>> GetListObjectsAsync<T>() where T : IHueObject
        {

            string typename = typeof(T).GetHueType();
            string url = BridgeUrl + $"/{typename}";
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                Dictionary<string, T> data = Serializer.DeserializeToObject<Dictionary<string, T>>(comres.Data);
                if (data != null) return data;
                foreach (KeyValuePair<string, T> t in data)
                {
                    t.Value.Id = t.Key;
                }
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Get the newly detected lights or sensors. This will not work on other HueObject Types.
        /// </summary>
        /// <typeparam name="T">Type of the object to detect.</typeparam>
        /// <returns>BridgeCommResult</returns>
        public SearchResult GetNewObjects<T>() where T : IHueObject
        {
            string typename = typeof(T).GetHueType();
            string url = BridgeUrl + $"/{typename}";

            CommResult comres = Comm.SendRequest(new Uri(url), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                SearchResult sr = Serializer.DeserializeToObject<SearchResult>(comres.Data);
                if (sr != null) return sr;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url,comres.Status);
            return null;
        }

        /// <summary>
        /// Get the newly detected lights or sensors async. This will not work on other HueObject Types.
        /// </summary>
        /// <typeparam name="T">Type of the object to detect.</typeparam>
        /// <returns>BridgeCommResult</returns>
        public async Task<SearchResult> GetNewObjectsAsync<T>() where T : IHueObject
        {
            string typename = typeof(T).GetHueType();
            string url = BridgeUrl + $"/{typename}";

            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url + "/new"), WebRequestType.Get);

            if (comres.Status == WebExceptionStatus.Success)
            {
                SearchResult sr = Serializer.DeserializeToObject<SearchResult>(comres.Data);
                if (sr != null) return sr;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return null;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }
    }
}
