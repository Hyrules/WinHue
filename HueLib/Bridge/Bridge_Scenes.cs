using System;
using System.Collections.Generic;
using HueLib_base;

namespace HueLib
{
    public partial class Bridge
    {
        /// <summary>
        /// Get The list of scenes from the bridge.
        /// </summary>
        /// <returns>a list of scenes available in the bridge.</returns>
        public Dictionary<string,Scene> GetScenesList()
        {
            
            Dictionary<string,Scene> listScenes;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/scenes"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    listScenes = Serializer.DeserializeToObject<Dictionary<string,Scene>>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    listScenes = null;
                }
            }
            catch
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                listScenes = null;
            }
            return listScenes;
        }

        /// <summary>
        /// Set the parameters of a light in a scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="lightId">The light ID.</param>
        /// <param name="newState">The desired state of the light.</param>
        /// <returns>A list of MessageCollection from the bridge.</returns>
        public MessageCollection SetSceneLightState(string sceneName, string lightId, State newState)
        {    
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + sceneName + "/lightstates/" + lightId), WebRequestType.PUT, Serializer.SerializeToJson(newState));
                if (!string.IsNullOrEmpty(message))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch
            {
                lastMessages = new MessageCollection();
            }
            return lastMessages;
        }

        /// <summary>
        /// Create a new scene in the bridge.
        /// </summary>
        /// <param name="newScene">New Scene to create.</param>
        /// <returns>The ID of the created scene.</returns>
        public string CreateScene(Scene newScene)
        {
            string sceneId = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/"), WebRequestType.POST,Serializer.SerializeToJson<Scene>(newScene));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    sceneId = lastMessages.FailureCount >= 1 ? "" : ((CreationSuccess) lastMessages[0]).id;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    sceneId = "";
                }
            }
            catch
            {
                sceneId = "";
            }
            return sceneId;
        }

        /// <summary>
        /// Activate a scene on the bridge.
        /// </summary>
        /// <param name="sceneId">ID of the scene to activate.</param>
        /// <returns>True or false if the scene has been applied.</returns>
        public bool ActivateScene(string sceneId)
        {
            bool result = false;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups/0/action"), WebRequestType.PUT, "{\"scene\":\"" + sceneId + "\"}");
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
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Delete a scene.
        /// </summary>
        /// <param name="sceneId">Id of the scene to delete.</param>
        /// <returns>True or false the scene has been deleted.</returns>
        public bool DeleteScene(string sceneId)
        {
            bool result = false;
            string message = Communication.SendRequest(new Uri(BridgeUrl + $@"/scenes/{sceneId}"), WebRequestType.DELETE);
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

            return result;

        }

        /// <summary>
        /// Change the name of a Scene.
        /// </summary>
        /// <param name="id">ID of the scene to change the name.</param>
        /// <param name="newName">New name of the rule.</param>
        /// <returns>A message collection.</returns>
        public MessageCollection ChangeSceneName(string id, string newName)
        {

            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.PUT, Serializer.SerializeToJson<Scene>(new Scene() { name = newName }));
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
        /// Change the desired of a Scene.
        /// </summary>
        /// <param name="id">ID of the scene to change the name.</param>
        /// <param name="newname">New name of the scene.</param>
        /// <param name="newlightsList">New List of lights</param>
        /// <returns>the id of the modified scene or null</returns>
        public string ChangeScene(string id, string newname, List<string> newlightsList)
        {
            string modifiedid = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.PUT, Serializer.SerializeToJson<Scene>(new Scene() {name = newname, lights = newlightsList}));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 2)
                        modifiedid = id;
                }
                else
                {
                    lastMessages = new MessageCollection {_bridgeNotResponding};
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                lastMessages = new MessageCollection();
            }

            return modifiedid;

        }

        /// <summary>
        /// Store current light state in the selected scene.
        /// </summary>
        /// <param name="id">ID of the scene to change the name.</param>
        /// <param name="newlightsList">New List of lights</param>
        /// <returns>the id of the modified scene or null</returns>
        public string SetScene(string id)
        {
            string modifiedid = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.PUT, Serializer.SerializeToJson<Scene>(new Scene() { storelightstate = true}));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 2)
                        modifiedid = id;
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

            return modifiedid;

        }

        /// <summary>
        /// Get a specified scene from the bridge.
        /// </summary>
        /// <param name="id">Id of the scene.</param>
        /// <returns>The requested scene.</returns>
        public Scene GetScene(string id)
        {
            Scene scene = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                {
                    scene = Serializer.DeserializeToObject<Scene>(message);
                    if(scene == null)
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
                scene = null;
            }
            return scene;
        }

  
    }
}
