using System;
using System.Collections.Generic;
using HueLib_base;

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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/lights/new"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    newSensors = Serializer.DeserializeSearchResult(message);
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
                newSensors = null;

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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + ID), WebRequestType.DELETE);
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        deleted = true;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                deleted = false;
            }
            return deleted;
        }
        
        /// <summary>
        /// Get the list of sensors available on the bridge.
        /// </summary>
        /// <returns>A list of sensors available on the bridge.</returns>
        public Dictionary<string,Sensor> GetSensorList()
        {
            Dictionary<string, Sensor> sensorsList;
            
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                    sensorsList = Serializer.DeserializeToObject<Dictionary<string, Sensor>>(message);
                //sensorsList = Serializer.DeserializeSensorList<Dictionary<string, Sensor>>(message);
                else
                {
                    lastMessages = new MessageCollection {_bridgeNotResponding};
                    BridgeNotResponding?.Invoke(this, _e);
                    sensorsList = null;
                }
            }
            catch(Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                sensorsList = null;
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
            Sensor sensor;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString()), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                {
                    sensor = Serializer.DeserializeToObject<Sensor>(message);
                    if (sensor == null)
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                }
                else
                {
                    lastMessages = new MessageCollection {_bridgeNotResponding};
                    BridgeNotResponding?.Invoke(this, _e);
                    sensor = null;
                }
            }
            catch(Exception)
            {
                if (!string.IsNullOrEmpty(Communication.lastjson))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                sensor = null;
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
            string result;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors"), WebRequestType.POST, Serializer.SerializeToJson<Sensor>(newSensor));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    result = lastMessages.SuccessCount == 1 ? ((CreationSuccess) _lastmessages[0]).id : null;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    result = null;
                }
            }
            catch (Exception)
            {
                result = null;
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
            try
            {
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
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Sensor>(sensor));
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.FailureCount == 0)
                    {
                        result = true;
                    }
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
        /// Start the reserch for new sensors.
        /// </summary>
        /// <returns>True or false the research has begun.</returns>
        public bool FindNewSensors()
        {
            bool result = false;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors"), WebRequestType.POST);
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
            catch(Exception)
            {
                result = false;
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString() + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<SensorConfig>(newConfig));
                if (!string.IsNullOrEmpty(message))
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch(Exception)
            {
                lastMessages = new MessageCollection();
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
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString()), WebRequestType.PUT, Serializer.SerializeToJson<Sensor>(new Sensor() {name = newName}));
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
        /// Set the flag of a sensor to the desired value.
        /// </summary>
        /// <param name="id">Id of the sensor.</param>
        /// <param name="config">Config of the sensor</param>
        /// <returns></returns>
        public MessageCollection SetSensorFlag(string id,SensorState config)
        {
          
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/sensors/" + id.ToString() + "/state"), WebRequestType.PUT, Serializer.SerializeToJson<SensorState>(config));
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
