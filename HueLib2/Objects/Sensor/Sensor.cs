using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Documents;
using System.Windows.Media;
using HueLib2.Objects.HueObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Sensor Class.
    /// </summary>
    [DataContract,ExpandableObject,JsonConverter(typeof(SensorJsonConverter))]
    public class Sensor : IHueObject
    {
        private string _name;
        private ImageSource _image;

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Image of the Sensor"), ExpandableObject, ReadOnly(true)]
        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("ID of the Sensor"),  ReadOnly(true), Browsable(false)]
        public string Id { get; set; }

        /// <summary>
        /// Name of the sensor.
        /// </summary>
        [DataMember(Name = "name"), Category("Sensor Properties"), Description("Name of the sensor")]
        public string Name
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
        [DataMember, Category("Sensor Properties"), Description("ModelID of the sensor"), CreateOnly]
        public string modelid { get; set; }
        /// <summary>
        /// Software version of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Software version of the sensor"), CreateOnly]
        public string swversion { get; set; }
        /// <summary>
        /// Type of sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Type of the sensor"), CreateOnly]
        public string type { get; set; }
        /// <summary>
        /// Manufacturer name of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Manufacturer name of the sensor"), CreateOnly]
        public string manufacturername { get; set; }

        /// <summary>
        /// Unique ID of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Unique ID of the sensor"), CreateOnly]
        public string uniqueid { get; set; }

        /// <summary>
        /// Sensor config.
        /// </summary>
        [DataMember,ExpandableObject, Category("Configuration"), Description("Configuration of the sensor"), CreateOnly]
        public SensorConfig config { set; get; }

        /// <summary>
        /// Sensor state.
        /// </summary>
        [DataMember,ExpandableObject, Category("State"), Description("State of the sensor"),CreateOnly]
        public SensorState state { set; get; }

        [JsonIgnore]
        [DataMember]
        public string productid { get; internal set; }

        [JsonIgnore]
        [DataMember]
        public string swconfigid { get; internal set; }

        /// <summary>
        /// To String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }


        /// <summary>
        /// Event that happen when property has change.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// When a property has change this event is triggered - needed for the binding to refresh properly.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
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

                    if (o.Value["manufacturername"] != null)
                        sensor.manufacturername = o.Value["manufacturername"].Value<string>();
                    if (o.Value["modelid"] != null)
                    sensor.modelid = o.Value["modelid"].Value<string>();
                    if (o.Value["name"] != null)
                        sensor.Name = o.Value["name"].Value<string>();
                    if(o.Value["swversion"] != null)
                        sensor.swversion = o.Value["swversion"].Value<string>();
                    if (o.Value["type"] != null)
                        sensor.type = o.Value["type"].Value<string>();
                    if (o.Value["uniqueid"] != null)
                        sensor.uniqueid = o.Value["uniqueid"].Value<string>();

                    if (obj["config"] != null)
                        sensor = SetConfig(sensor, obj["config"].Value<JObject>());
                    if (obj["state"] != null)
                        sensor = SetState(sensor, obj["state"].Value<JObject>());

                    sensorslist.Add(o.Key, sensor);
                }
                return sensorslist;
            }



            if (objectType == typeof(Sensor))
            {
                
                Sensor sensor = new Sensor();

                if (obj["manufacturername"] != null)
                    sensor.manufacturername = obj["manufacturername"].Value<string>();
                if (obj["modelid"] != null)
                sensor.modelid = obj["modelid"].Value<string>();
                if (obj["name"] != null)
                    sensor.Name = obj["name"].Value<string>();
                if(obj["swversion"] != null)
                    sensor.swversion = obj["swversion"].Value<string>();
                if (obj["type"] != null)
                    sensor.type = obj["type"].Value<string>();
                if (obj["uniqueid"] != null)
                    sensor.uniqueid = obj["uniqueid"].Value<string>();

                if(obj["config"] != null)
                    sensor = SetConfig(sensor, obj["config"].Value<JObject>());
                if(obj["state"] != null)
                    sensor = SetState(sensor, obj["state"].Value<JObject>());
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
        private Sensor SetConfig(Sensor sensor, JObject config)
        {


            JsonSerializerSettings jss = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.Default
            };

            switch (sensor.type)
            {
                case "ZGPSwitch":
                    if (config != null)
                    {
          
                        sensor.config = JsonConvert.DeserializeObject<HueTapSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "Daylight":
                    if (config != null)
                    {
 
                        sensor.config = JsonConvert.DeserializeObject<DaylightSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "CLIPPresence":
                    if (config != null)
                    {
     
                        sensor.config = JsonConvert.DeserializeObject<ClipPresenceSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "CLIPGenericFlag":
                    if (config != null)
                    {
  
                        sensor.config = JsonConvert.DeserializeObject<ClipGenericFlagSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "CLIPGenericStatus":
                    if (config != null)
                    {
                      
                        sensor.config = JsonConvert.DeserializeObject<ClipGenericStatusSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "CLIPHumidity":
                    if (config != null)
                    {

                        sensor.config = JsonConvert.DeserializeObject<ClipHumiditySensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "CLIPOpenClose":
                    if (config != null)
                    {
             
                        sensor.config = JsonConvert.DeserializeObject<ClipOpenCloseSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "ZLLTemperature":
                case "CLIPTemperature":
                    if (config != null)
                    {
                     
                        sensor.config = JsonConvert.DeserializeObject<TemperatureSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "ZLLSwitch":
                    if (config != null)
                    {
                
                        sensor.config = JsonConvert.DeserializeObject<HueDimmerSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "ZLLPresence":
                    if (config != null)
                    {

                        sensor.config = JsonConvert.DeserializeObject<HueMotionSensorConfig>(config.ToString(),jss);
                    }
                    break;
                case "CLIPLightlevel":
                case "ZLLLightLevel":
                    if (config != null)
                    {

                        sensor.config = JsonConvert.DeserializeObject<LightLevelConfig>(config.ToString(),jss);
                    }
                    break;
                default:
                    if (config != null)
                    {
        
                        sensor.config = JsonConvert.DeserializeObject<SensorConfig>(config.ToString(),jss);
                    }
                    break;

            }
            return sensor;
        }

        private Sensor SetState(Sensor sensor, JObject state)
        {


            JsonSerializerSettings jss = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.Default
            };

            switch (sensor.type)
            {
                case "ZGPSwitch":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<ButtonSensorState>(state.ToString(), jss);
                    }
                    break;
                case "Daylight":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<DaylightSensorState>(state.ToString(), jss);
                    }
                    break;
                case "CLIPPresence":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<PresenceSensorState>(state.ToString(), jss);
                    }
                    break;
                case "CLIPGenericFlag":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<ClipGenericFlagSensorState>(state.ToString(), jss);
                    }
                    break;
                case "CLIPGenericStatus":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<ClipGenericStatusState>(state.ToString(), jss);
                    }
                    break;
                case "CLIPHumidity":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<ClipHumiditySensorState>(state.ToString(), jss);
                    }
                    break;
                case "CLIPOpenClose":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<ClipOpenCloseSensorState>(state.ToString(), jss);
                    }
                    break;
                case "ZLLTemperature":
                case "CLIPTemperature":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<TemperatureSensorState>(state.ToString(), jss);
                    }
                    break;
                case "ZLLSwitch":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<ButtonSensorState>(state.ToString(), jss);
                    }
                    break;
                case "ZLLPresence":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<PresenceSensorState>(state.ToString(), jss);
                    }
                    break;
                case "CLIPLightlevel":
                case "ZLLLightLevel":
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<LightLevelState>(state.ToString(), jss);
                    }
                    break;
                default:
                    if (state != null)
                    {
                        sensor.state = JsonConvert.DeserializeObject<SensorState>(state.ToString(), jss);
                    }
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

