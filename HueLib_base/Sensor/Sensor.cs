using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib_base
{
    /// <summary>
    /// Sensor Class.
    /// </summary>
    [DataContract,ExpandableObject,JsonConverter(typeof(SensorJsonConverter))]
    public class Sensor : HueObject
    {
        private string _name;
        /// <summary>
        /// Name of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Name of the sensor")]
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Model id of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("ModelID of the sensor")]
        public string modelid { get; set; }
        /// <summary>
        /// Software version of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Software version of the sensor")]
        public string swversion { get; set; }
        /// <summary>
        /// Type of sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Type of the sensor")]
        public string type { get; set; }
        /// <summary>
        /// Manufacturer name of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Manufacturer name of the sensor")]
        public string manufacturername { get; set; }

        /// <summary>
        /// Unique ID of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Unique ID of the sensor")]
        public string uniqueid { get; set; }

        /// <summary>
        /// Sensor config.
        /// </summary>
        [DataMember,ExpandableObject, Category("Configuration"), Description("Configuration of the sensor")]
        public SensorConfig config { set; get; }

        /// <summary>
        /// Sensor state.
        /// </summary>
        [DataMember,ExpandableObject, Category("State"), Description("State of the sensor")]
        public SensorState state { set; get; }

        /// <summary>
        /// To String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }

    /// <summary>
    /// Sensor Serializer.
    /// </summary>
    public class SensorJsonConverter : JsonConverter
    {
        /// <summary>
        /// read a json string and transform to a class.
        /// </summary>
        /// <param name="reader">Json reader.</param>
        /// <param name="objectType">Type of object.</param>
        /// <param name="existingValue">Existing value.</param>
        /// <param name="serializer">Json Serializer.</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            JObject obj = (JObject)serializer.Deserialize(reader);

            if (objectType == typeof(Dictionary<string, Sensor>))
            {
                Dictionary<string, Sensor> sensorslist = new Dictionary<string, Sensor>();
                foreach (KeyValuePair<string, JToken> o in obj)
                {
                    Sensor sensor = new Sensor();
                    if(o.Value["manufacturername"] != null)
                        sensor.manufacturername = o.Value["manufacturername"].Value<string>();
                    if (o.Value["modelid"] != null)
                    sensor.modelid = o.Value["modelid"].Value<string>();
                    if (o.Value["name"] != null)
                        sensor.name = o.Value["name"].Value<string>();
                    if(o.Value["swversion"] != null)
                        sensor.swversion = o.Value["swversion"].Value<string>();
                    if (o.Value["type"] != null)
                        sensor.type = o.Value["type"].Value<string>();
                    if (o.Value["uniqueid"] != null)
                        sensor.uniqueid = o.Value["uniqueid"].Value<string>();
                    sensor = SetConfigState(sensor, o.Value["config"].Value<JObject>(), o.Value["state"].Value<JObject>());
                    sensorslist.Add(o.Key, sensor);
                }
                return sensorslist;
            }

            if (objectType == typeof(Sensor))
            {
                Sensor sensor = new Sensor();
                if(obj["manufacturername"] != null)
                    sensor.manufacturername = obj["manufacturername"].Value<string>();
                if (obj["modelid"] != null)
                sensor.modelid = obj["modelid"].Value<string>();
                if (obj["name"] != null)
                    sensor.name = obj["name"].Value<string>();
                if(obj["swversion"] != null)
                    sensor.swversion = obj["swversion"].Value<string>();
                if (obj["type"] != null)
                    sensor.type = obj["type"].Value<string>();
                if (obj["uniqueid"] != null)
                    sensor.uniqueid = obj["uniqueid"].Value<string>();
                if (obj["config"] != null)
                    sensor.config = JsonConvert.DeserializeObject<SensorConfig>(obj["config"].ToString());
                sensor = SetConfigState(sensor, obj["config"].Value<JObject>(), obj["state"].Value<JObject>());
               
                return sensor;
            }

            return null;
          
        }

        /// <summary>
        /// Config State setter.
        /// </summary>
        /// <param name="sensor">Current sensor.</param>
        /// <param name="config">Config.</param>
        /// <param name="state">State.</param>
        /// <returns></returns>
        private Sensor SetConfigState(Sensor sensor, JObject config, JObject state)
        {

            switch (sensor.type)
            {
                case "ZGPSwitch":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<HueTapSensorState>(state.ToString());
                    break;
                case "Daylight":

                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<DaylightSensorState>(state.ToString());
                    break;
                case "CLIPPresence":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<ClipPresenceSensorState>(state.ToString());
                    break;
                case "CLIPGenericFlag":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<ClipGenericFlagSensorState>(state.ToString());
                    break;
                case "CLIPGenericStatus":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<ClipGenericStatusState>(state.ToString());
                    break;
                case "CLIPHumidity":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<ClipHumiditySensorState>(state.ToString());
                    break;
                case "CLIPOpenClose":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<ClipOpenCloseSensorState>(state.ToString());
                    break;
                case "CLIPTemperature":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<ClipTemperatureSensorState>(state.ToString());
                    break;
                case "ZLLSwitch":
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<HueDimmerSensorState>(state.ToString());
                    break;
                default:
                    if (state != null)
                        sensor.state = JsonConvert.DeserializeObject<SensorState>(state.ToString());
                    break;

            }
            return sensor;
        }
        
        /// <summary>
        /// Check if can be converted to a sensor.
        /// </summary>
        /// <param name="objectType">Type of object to convert.</param>
        /// <returns>Sensor or a dictionary of sensors.</returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Dictionary<int, Sensor>)) || objectType == typeof(Sensor);
        }

        /// <summary>
        /// Convert sensor to string.
        /// </summary>
        /// <param name="writer">Json Writer</param>
        /// <param name="value">Object to write.</param>
        /// <param name="serializer">Json Serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Can Write.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
