using System;
using System.Collections.Generic;
using System.Net;
using HueLib.BridgeMessages.Error;
using HueLib;

namespace HueLib
{
    public partial class Bridge
    {
        /// <summary>
        /// Delete the selected light from the bridge.
        /// </summary>
        /// <param name="ID">ID of the light to delete.</param>
        /// <returns>True for success or false for error.</returns>
        public HueResult DeleteLight(string ID)
        {
            HueResult result = new HueResult();
            
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID),WebRequestType.DELETE);
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            result.Success = lastMessages[0].GetType() == typeof(DeletionSuccess) ? true : false;

                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection {new UnkownError(comres)};
                    
                    break;
            }
            result.messages = lastMessages;
            return result;
        }
        
        /// <summary>
        /// Set the state object of a light.
        /// </summary>
        /// <param name="ID">ID of the light to change state.</param>
        /// <param name="newState">The new state of the light to change.</param>
        /// <returns>A list of MessageCollection from the bridge.</returns>
        public HueResult SetLightState(string ID, State newState)
        {
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID + "/state"),WebRequestType.PUT, Serializer.SerializeToJson<State>(newState));
            HueResult result = new HueResult();
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres) } : new MessageCollection(lstmsg);
                    result.Success = true;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection{new UnkownError(comres)};
                    break;
            }
            result.messages = lastMessages;
            return result;
        }

        /// <summary>
        /// Set the desired light name.
        /// </summary>
        /// <param name="ID">ID of the light.</param>
        /// <param name="Name">The name of the light.</param>
        /// <returns>A list of MessageCollection from the bridge.</returns>
        public HueResult ChangeLightName(string ID, string Name)
        {
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID), WebRequestType.PUT, Serializer.SerializeToJson<Light>(new Light() { name = Name }));
            HueResult result = new HueResult();
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    lastMessages = lstmsg == null ? new MessageCollection {new UnkownError(comres)} : new MessageCollection(lstmsg);
                    result.Success = true;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection{new UnkownError(comres)};
                    break;
            }
            return result;
        }

        /// <summary>
        /// Gets a list of all lights that have been discovered by the bridge.
        /// </summary>
        /// <returns>Returns a list of all lights connected to the bridge.</returns>
        public Dictionary<string, Light> GetLightList()
        {
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights"), WebRequestType.GET);
            Dictionary<string, Light> lightList = new Dictionary<string, Light>();

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lightList = Serializer.DeserializeToObject<Dictionary<string, Light>>(comres.data);
                    if (lightList != null) return lightList;
                    lightList = new Dictionary<string, Light>();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres)} : new MessageCollection(lstmsg);
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection{new UnkownError(comres)};
                    break;
            }

            return lightList;
        }

        /// <summary>
        /// Gets a list of lights that were discovered the last time a search for new lights was performed. The list of new lights is always deleted when a new search is started.
        /// </summary>
        /// <returns>Returns a list of new lights detected otherwise will return null</returns>
        public SearchResult GetNewLights()
        {
            SearchResult newLights = new SearchResult();
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights/new"), WebRequestType.GET);
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    newLights = Serializer.DeserializeSearchResult(comres.data);
                    if (newLights != null) return newLights;
                    newLights = new SearchResult();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres)} : new MessageCollection(lstmsg);
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection{new UnkownError(comres)};
                    break;
            }

            return newLights;
        }

        /// <summary>
        /// Gets the attributes and state of a given light.
        /// </summary>
        /// <param name="ID">ID of the light.</param>
        /// <returns>The requested light.</returns>
        public Light GetLight(string ID)
        {
            Light light = new Light();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    light = Serializer.DeserializeToObject<Light>(comres.data);
                    if (light != null) return light;
                    light = new Light();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);               
                    lastMessages = lstmsg != null ? new MessageCollection(lstmsg) : new MessageCollection {new UnkownError(comres)};
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection{new UnkownError(comres)};
                    break;
            }

            return light;
        }

        /// <summary>
        /// Send the command to the bridge to search for new lights.
        /// </summary>
        /// <returns>True OR False command sent succesfully</returns>
        public bool SearchNewLights()
        {
            bool result = false;

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights"), WebRequestType.POST);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);

                    if (lstmsg != null)
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            result = true;
                    }
                    else
                    {
                        lastMessages = new MessageCollection {new UnkownError(comres)};
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection
                    {
                        new UnkownError(comres)
                    };
                    break;
            }
            return result;
        }
    }
}
