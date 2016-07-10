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
        /// Delete the specified Schedule.
        /// </summary>
        /// <param name="id">ID of the schedule to delete.</param>
        /// <returns>The ID of the deleted schedule or NULL when error.</returns>
        public bool DeleteSchedule(string id)
        {
            bool result = false;
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id), WebRequestType.DELETE);

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
        /// Update the specified Schedule
        /// </summary>
        /// <param name="id">Id of the scedule to update</param>
        /// <param name="schedule">new schedule settings</param>
        /// <returns>true or false of the update was applied</returns>
        public bool UpdateSchedule(string id, Schedule schedule)
        {
            bool result = false;

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id), WebRequestType.PUT,Serializer.SerializeToJson<Schedule>(schedule));

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
        /// Get a list of all schedules.
        /// </summary>
        /// <returns>Returns a list of all schedules or NULL when error.</returns>
        public Dictionary<string,Schedule> GetScheduleList()
        {
            Dictionary<string, Schedule> result = new Dictionary<string, Schedule>();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/schedules"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    result = Serializer.DeserializeToObject<Dictionary<string, Schedule>>(comres.data);
                    if (result != null) return result;
                    result = new Dictionary<string, Schedule>();
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
        /// Allows the user to create new schedules. The bridge can store up to 100 schedules.
        /// </summary>
        /// <param name="schedule">The new schedule to create.</param>
        /// <returns>ID of the newly created schedule.</returns>
        public string CreateSchedule(Schedule schedule)
        {
            string result = "";
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/schedules"), WebRequestType.POST, Serializer.SerializeToJson<Schedule>(schedule));

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
                            result = (((CreationSuccess)lastMessages[0]).id);
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
        /// Get the specified schedule attributes.
        /// </summary>
        /// <param name="id">Id of the light.</param>
        /// <returns>Return the requested schedule.</returns>
        public Schedule GetSchedule(string id)
        {
            Schedule result = new Schedule();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    result = Serializer.DeserializeToObject<Schedule>(comres.data);
                    if (result != null) return result;
                    result = new Schedule();
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
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

            return result;
        }

        /// <summary>
        /// Change the name of Schedule.
        /// </summary>
        /// <param name="id">ID of the schedule to rename.</param>
        /// <param name="newName">New name of the schedule.</param>
        /// <returns>A collection of messages.</returns>
        public MessageCollection ChangeScheduleName(string id, string newName)
        {
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Schedule>(new Schedule() { name = newName }));

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
