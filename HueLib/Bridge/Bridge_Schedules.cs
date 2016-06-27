using System;
using System.Collections.Generic;
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id), WebRequestType.DELETE);
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


        public bool UpdateSchedule(string id, Schedule schedule)
        {
            bool result = false;
            try
            {
                string message = (Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id), WebRequestType.PUT, Serializer.SerializeToJson<Schedule>(schedule)));
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

        /// <summary>
        /// Get a list of all schedules.
        /// </summary>
        /// <returns>Returns a list of all schedules or NULL when error.</returns>
        public Dictionary<string,Schedule> GetScheduleList()
        {
            Dictionary<string, Schedule> result = new Dictionary<string, Schedule>();
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/schedules"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    result = Serializer.DeserializeToObject<Dictionary<string, Schedule>>(message);
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                if(!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));            

                result = null;
                
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
            string result = null;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/schedules"), WebRequestType.POST, Serializer.SerializeToJson<Schedule>(schedule));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        result = (((CreationSuccess) lastMessages[0]).id);
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = null;
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
            Schedule result;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                {
                    result = Serializer.DeserializeToObject<Schedule>(message);
                    if (result == null)
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                }
                else
                {
                    lastMessages = new MessageCollection {_bridgeNotResponding};
                    BridgeNotResponding?.Invoke(this, _e);
                    result = null;
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
        /// Change the name of Schedule.
        /// </summary>
        /// <param name="id">ID of the schedule to rename.</param>
        /// <param name="newName">New name of the schedule.</param>
        /// <returns>A collection of messages.</returns>
        public MessageCollection ChangeScheduleName(string id, string newName)
        {
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/schedules/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Schedule>(new Schedule() { name = newName }));
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
