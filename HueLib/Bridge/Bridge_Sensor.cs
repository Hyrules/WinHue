using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using HueLib.BridgeMessages.Error;
using HueLib2;

namespace HueLib
{
    public partial class Bridge
    {

        /// <summary>
        /// Get a list of newly discovered sensors.
        /// </summary>
        /// <returns>A list of sensors.</returns>
        public SearchResult GetNewSensors()
        {
            SearchResult newSensors = new SearchResult();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/lights/new"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    newSensors = Serializer.DeserializeSearchResult(comres.data);
                    if (newSensors != null) return newSensors;
                    newSensors = new SearchResult();
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


            return newSensors;
        } 

        /// <summary>
        /// Delete the selected sensor from the bridge.
        /// </summary>
        /// <param name="ID">ID of the sensor to delete.</param>
        /// <returns>The ID of the deleted sensor or null when error.</returns>
        public bool DeleteSensor(string ID)
        {
            bool deleted = false;

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + ID), WebRequestType.DELETE);

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
                            deleted = true;
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

            return deleted;
        }
        
        /// <summary>
        /// Get the list of sensors available on the bridge.
        /// </summary>
        /// <returns>A list of sensors available on the bridge.</returns>
        public Dictionary<string,Sensor> GetSensorList()
        {
            Dictionary<string, Sensor> sensorsList = new Dictionary<string, Sensor>();
            
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    sensorsList = Serializer.DeserializeToObject<Dictionary<string, Sensor>>(comres.data);
                    if(sensorsList !=null) return sensorsList;
                    sensorsList = new Dictionary<string, Sensor>();
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

            return sensorsList;
        }

        /// <summary>
        /// Get a sensor by it's ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The requested sensor.</returns>
        public Sensor GetSensor(string id)
        {
            Sensor sensor = new Sensor();

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString()), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    sensor = Serializer.DeserializeToObject<Sensor>(comres.data);
                    if (sensor != null) return sensor;
                    sensor = new Sensor();
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

            return sensor;
        }

        /// <summary>
        /// Create a new sensor on the bridge.
        /// </summary>
        /// <param name="newSensor"></param>
        /// <returns>The created sensor id or null.</returns>
        public string CreateSensor(Sensor newSensor)
        {
            string result = "";
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors"), WebRequestType.POST, Serializer.SerializeToJson<Sensor>(newSensor));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        result = lastMessages.SuccessCount == 1 ? ((CreationSuccess)_lastmessages[0]).id : "";
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
        /// Rename a sensor.
        /// </summary>
        /// <param name="id">ID of the sensor to rename.</param>
        /// <param name="sensor">The new value of the sensor.</param>
        /// <returns>True or False the sensor has been renamed.</returns>
        public bool UpdateSensor(string id, Sensor sensor)
        {
            bool result = false;

            sensor.state.lastupdated = null;
            sensor.name = null;
            sensor.modelid = null;
            sensor.type = null;
            sensor.manufacturername = null;
            sensor.swversion = null;
            sensor.uniqueid = null;
            if (sensor.config != null)
            {
                sensor.config.on = null;
                sensor.config.reachable = null;
            }

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Sensor>(sensor));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection(lstmsg);
                        if (lastMessages.FailureCount == 0)
                        {
                            result = true;
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

            return result;
        }

        /// <summary>
        /// Start the reserch for new sensors.
        /// </summary>
        /// <returns>True or false the research has begun.</returns>
        public bool FindNewSensors()
        {
            bool result = false;

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors"), WebRequestType.POST);


            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    else
                    {
                        lastMessages = new MessageCollection();
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
        /// Change the config of an existing sensor.
        /// </summary>
        /// <param name="id">ID of the sensor.</param>
        /// <param name="newConfig">New sensor config of the sensor.</param>
        /// <returns>The new sensor config.</returns>
        public MessageCollection ChangeSensorConfig(string id, SensorConfig newConfig)
        {
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString() + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<SensorConfig>(newConfig));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
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
        /// Change the name of a sensor.
        /// </summary>
        /// <param name="id">ID of the sensor to change the name.</param>
        /// <param name="newName">New name of the sensor.</param>
        /// <returns>A collection of messages.</returns>
        public MessageCollection ChangeSensorName(string id, string newName)
        {

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Sensor>(new Sensor() { name = newName }));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
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
        /// Set the flag of a sensor to the desired value.
        /// </summary>
        /// <param name="id">Id of the sensor.</param>
        /// <param name="config">Config of the sensor</param>
        /// <returns></returns>
        public MessageCollection SetSensorFlag(string id,SensorState config)
        {
          
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString() + "/state"), WebRequestType.PUT, Serializer.SerializeToJson<SensorState>(config));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if (lstmsg == null)
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
