using System;
using System.Collections.Generic;
using HueLib_base;

namespace HueLib
{
    public partial class Bridge
    {
        /// <summary>
        /// Delete the selected light from the bridge.
        /// </summary>
        /// <param name="ID">ID of the light to delete.</param>
        /// <returns>True for success or false for error.</returns>
        public bool DeleteLight(string ID)
        {
            bool id = false;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID), WebRequestType.DELETE);
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        id = lastMessages[0].GetType() == typeof (DeletionSuccess) ? true : false;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                id = false;
            }
            return id;
        }
        
        /// <summary>
        /// Set the state object of a light.
        /// </summary>
        /// <param name="ID">ID of the light to change state.</param>
        /// <param name="newState">The new state of the light to change.</param>
        /// <returns>A list of MessageCollection from the bridge.</returns>
        public MessageCollection SetLightState(string ID, State newState)
        {
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID + "/state"), WebRequestType.PUT, Serializer.SerializeToJson<State>(newState));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                lastMessages = new MessageCollection();
            }
            return lastMessages;
        }

        /// <summary>
        /// Set the desired light name.
        /// </summary>
        /// <param name="ID">ID of the light.</param>
        /// <param name="Name">The name of the light.</param>
        /// <returns>A list of MessageCollection from the bridge.</returns>
        public MessageCollection ChangeLightName(string ID, string Name)
        {      
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID), WebRequestType.PUT, Serializer.SerializeToJson<Light>(new Light() {name = Name}));
                if (!string.IsNullOrEmpty(message))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                lastMessages = new MessageCollection();
            }
            return lastMessages;
        }

        /// <summary>
        /// Gets a list of all lights that have been discovered by the bridge.
        /// </summary>
        /// <returns>Returns a list of all lights connected to the bridge.</returns>
        public Dictionary<string, Light> GetLightList()
        {
            Dictionary<string, Light> lightList;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    lightList = Serializer.DeserializeToObject<Dictionary<string, Light>>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    lightList = null;
                }
            }
            catch (Exception)
            {
                if(!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));

                lightList = null;
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights/new"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    newLights = Serializer.DeserializeSearchResult(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                newLights = null;

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
            Light light = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights/" + ID), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                {
                    light = Serializer.DeserializeToObject<Light>(message);
                    if(light == null)
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                }
                else
                {
                    lastMessages = new MessageCollection {_bridgeNotResponding};
                    BridgeNotResponding?.Invoke(this, _e);
                }

            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                light = null;            
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights"), WebRequestType.POST);
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        result = true;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
