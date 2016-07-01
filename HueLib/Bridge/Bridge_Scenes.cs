using System;
using System.Collections.Generic;
using System.Net;
using HueLib.BridgeMessages.Error;
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
            
            Dictionary<string,Scene> listScenes = new Dictionary<string, Scene>(); 
   
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/scenes"), WebRequestType.GET);
            
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    listScenes = Serializer.DeserializeToObject<Dictionary<string, Scene>>(comres.data);
                    if (listScenes == null)
                    {
                        listScenes = new Dictionary<string, Scene>();
                        List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                        lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres) } : new MessageCollection(lstmsg);

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
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + sceneName + "/lightstates/" + lightId), WebRequestType.PUT, Serializer.SerializeToJson(newState));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    lastMessages = lstmsg == null ? new MessageCollection {new UnkownError(comres)} : new MessageCollection(lstmsg);
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection{new UnkownError(comres)};
                    break;
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
            string sceneId = "";
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/"), WebRequestType.POST, Serializer.SerializeToJson<Scene>(newScene));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        sceneId = lastMessages.FailureCount >= 1 ? "" : ((CreationSuccess)lastMessages[0]).id;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/0/action"), WebRequestType.PUT, "{\"scene\":\"" + sceneId + "\"}");

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
                            result = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $@"/scenes/{sceneId}"), WebRequestType.DELETE);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            result = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.PUT, Serializer.SerializeToJson<Scene>(new Scene() { name = newName }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres)} : new MessageCollection(lstmsg);
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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
            string modifiedid = "";
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.PUT, Serializer.SerializeToJson<Scene>(new Scene() { name = newname, lights = newlightsList }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 2)
                            modifiedid = id;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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
            string modifiedid = "";
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.PUT, Serializer.SerializeToJson<Scene>(new Scene() { storelightstate = true }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                    lastMessages = new MessageCollection(lstmsg);
                    if (lastMessages.SuccessCount == 2)
                        modifiedid = id;

                    }
                    lastMessages = new MessageCollection();
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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
            Scene scene = new Scene();
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/scenes/" + id), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    scene = Serializer.DeserializeToObject<Scene>(comres.data);
                    if (scene == null)
                    {
                        List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                        if (lstmsg == null)
                        {
                            scene = new Scene();
                            goto default;
                        }
                        else
                        {
                            lastMessages = new MessageCollection(lstmsg);
                        }
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return scene;
        }

  
    }
}
