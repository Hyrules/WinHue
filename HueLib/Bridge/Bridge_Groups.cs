using System;
using System.Collections.Generic;
using Action = HueLib2.Action;
using HueLib2;
using System.Net;
using System.Windows.Documents.DocumentStructures;
using HueLib.BridgeMessages.Error;

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
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.PUT, Serializer.SerializeToJson<Group>(new Group() { name = Name }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
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

            return lastMessages;
        }

        /// <summary>
        /// Gets a list of all groups that have been added to the bridge. 
        /// </summary>
        /// <returns>A list of all groups that have been added to the bridge. </returns>
        public Dictionary<string, Group> GetGroupList()
        {
            Dictionary<string, Group> groupList = new Dictionary<string, Group>();
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    groupList = Serializer.DeserializeToObject<Dictionary<string, Group>>(comres.data);
                    if (groupList != null) return groupList;
                    groupList = new Dictionary<string, Group>();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg == null ? new MessageCollection { new UnkownError(comres) } : new MessageCollection(lstmsg);
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return groupList;
        }

        /// <summary>
        /// Create a new group
        /// </summary>
        /// <param name="Name">Name of the new group.</param>
        /// <param name="lightList">A List of lights in the group.</param>
        /// <return>Return the ID of the group created</return>
        public byte CreateGroup(string Name, List<string> lightList)
        {
            byte Id = 0;
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups"), WebRequestType.POST, Serializer.SerializeToJson<Group>(new Group() { name = Name, lights = lightList }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
                    {
                        goto default;
                    }
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.SuccessCount == 1)
                            Id = byte.Parse(((CreationSuccess)lastMessages[0]).id);
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

            return Id;
        }

        /// <summary>
        /// Create a new group.
        /// </summary>
        /// <param name="newgroup">Group to create.</param>
        /// <returns>Return the ID of the group created.</returns>
        public string CreateGroup(Group newgroup)
        {
            string Id = "";
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups"), WebRequestType.POST, Serializer.SerializeToJson<Group>(newgroup));

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
                            Id = (((CreationSuccess)lastMessages[0]).id);
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
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID + "/action"), WebRequestType.PUT, Serializer.SerializeToJson<Action>(action));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
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
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    result = Serializer.DeserializeToObject<Group>(comres.data);
                    if (result != null) return result;
                    result = new Group();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                    lastMessages = lstmsg != null ? new MessageCollection(lstmsg) : new MessageCollection { new UnkownError(comres) };
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
        /// Delete a group from the bridge.
        /// </summary>
        /// <param name="ID">Id of the group to delete</param>
        /// <returns>True if success, false if error.</returns>   
        public bool DeleteGroup(string ID)
        {
            bool result = false;
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.DELETE);

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
                            result = lastMessages[0].GetType() == typeof(DeletionSuccess) ? true : false;
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
        /// Change an existing group.
        /// </summary>
        /// <param name="id">ID of the group to change.</param>
        /// <param name="newgroup">The new parameters of the existing group.</param>
        /// <returns>A collection of messages.</returns>
        public MessageCollection ChangeGroup(string ID, Group newgroup)
        {
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + ID), WebRequestType.PUT, Serializer.SerializeToJson<Group>(newgroup));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
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

            return lastMessages;

        }

        /// <summary>
        /// Change an existing Group.
        /// </summary>
        /// <param name="newgroup">Old group with new properties.</param>
        /// <returns>A collection of messages.</returns>
        public MessageCollection ChangeGroup(Group newgroup)
        {

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/groups/" + newgroup.Id), WebRequestType.PUT, Serializer.SerializeToJson<Group>(newgroup));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
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

            return lastMessages;
        }
    }
}
