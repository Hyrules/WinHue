using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Action = HueLib_base.Action;
using HueLib_base;
using System.Collections.Specialized;

namespace HueLib
{
    public partial class Bridge
    {
        
        
        /// <summary>
        /// Set the desired group name.
        /// </summary>
        /// <param name="ID">ID of the light.</param>
        /// <param name="Name">The name of the group.</param>
        /// <returns>A list of MessageCollection from the bridge.</returns>
        public MessageCollection ChangeGroupName(string ID, string Name)
        {          
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.PUT, Serializer.SerializeToJson<Group>(new Group() { name = Name }));
                if(!string.IsNullOrEmpty(message))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                else
                {
                    lastMessages = new MessageCollection {_bridgeNotResponding};
                    BridgeNotResponding?.Invoke(this,_e);
                }
            }
            catch (Exception)
            {
                lastMessages = new MessageCollection();
            }
            return lastMessages;
        }

        /// <summary>
        /// Gets a list of all groups that have been added to the bridge. 
        /// </summary>
        /// <returns>A list of all groups that have been added to the bridge. </returns>
        public Dictionary<string, Group> GetGroupList()
        {
            Dictionary<string, Group> groupList = new Dictionary<string, Group>();
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    groupList = Serializer.DeserializeToObject<Dictionary<string, Group>>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    groupList = null;
                }
            }
            catch (Exception)
            {
                if(!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                groupList = null;          
            }
            return groupList;
        }

        /// <summary>
        /// Create a new group
        /// </summary>
        /// <param name="Name">Name of the new group.</param>
        /// <param name="lightList">A List of lights in the group.</param>
        /// <return>Return the ID of the group created</return>
        public byte? CreateGroup(string Name, List<string> lightList)
        {
            byte? Id = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups"), WebRequestType.POST, Serializer.SerializeToJson<Group>(new Group() {name = Name, lights = lightList}));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        Id = byte.Parse(((CreationSuccess) lastMessages[0]).id);
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch(Exception)
            {
                Id = null;
            }
            return Id;
        }

        /// <summary>
        /// Create a new group.
        /// </summary>
        /// <param name="newgroup">Group to create.</param>
        /// <returns>Return the ID of the group created.</returns>
        public string CreateGroup(Group newgroup)
        {
            string Id = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups"), WebRequestType.POST, Serializer.SerializeToJson<Group>(newgroup));
                if(!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if(lastMessages.SuccessCount == 1)
                        Id = (((CreationSuccess)lastMessages[0]).id);
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch(Exception)
            {
                Id = null;
            }
            return Id;
        }

        /// <summary>
        /// Send the group Action (State) to the specified group.
        /// </summary>
        /// <param name="ID">ID of the group</param>
        /// <param name="action">The action of the group.</param>
        /// <returns>A list of MessageCollection from the bridge.</returns>
        public MessageCollection SetGroupAction(string ID, Action action)
        {           
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID + "/action"), WebRequestType.PUT, Serializer.SerializeToJson<Action>(action));
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
        /// Get the attributes of the specified group.
        /// </summary>
        /// <param name="ID">ID of the group.</param>
        /// <returns>The requested group.</returns>
        public Group GetGroup(string ID)
        {
            Group result = new Group();
            try
            {

                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                {
                    result = Serializer.DeserializeToObject<Group>(message);
                    if(result == null)
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
                result = null;
                
            }
            return result;
        }

        /// <summary>
        /// Delete a group from the bridge.
        /// </summary>
        /// <param name="ID">Id of the group to delete</param>
        /// <returns>True if success, false if error.</returns>   
        public bool DeleteGroup(string ID)
        {
            bool result = false;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.DELETE);
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        result = lastMessages[0].GetType() == typeof (DeletionSuccess) ? true : false;
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

        /// <summary>
        /// Change an existing group.
        /// </summary>
        /// <param name="id">ID of the group to change.</param>
        /// <param name="newgroup">The new parameters of the existing group.</param>
        /// <returns>A collection of messages.</returns>
        public MessageCollection ChangeGroup(string ID, Group newgroup)
        {
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.PUT, Serializer.SerializeToJson<Group>(newgroup));
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
        /// Change an existing Group.
        /// </summary>
        /// <param name="newgroup">Old group with new properties.</param>
        /// <returns>A collection of messages.</returns>
        public MessageCollection ChangeGroup(Group newgroup)
        {
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + newgroup.Id), WebRequestType.PUT, Serializer.SerializeToJson<Group>(newgroup));
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
    }
}
