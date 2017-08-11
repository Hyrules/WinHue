using HueLib2.Objects.HueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2.Objects.Interfaces
{
    [JsonConverter(typeof(ISensorConverter)), HueType("sensors")]
    public interface ISensor : IHueObject
    {
        /// <summary>
        /// Model id of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("ModelID of the sensor"), CreateOnly]
        string modelid { get; set; }
        /// <summary>
        /// Software version of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Software version of the sensor"), CreateOnly]
        string swversion { get; set; }
        /// <summary>
        /// Type of sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Type of the sensor"), CreateOnly]
        string type { get; set; }
        /// <summary>
        /// Manufacturer name of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Manufacturer name of the sensor"), CreateOnly]
        string manufacturername { get; set; }

        /// <summary>
        /// Unique ID of the sensor.
        /// </summary>
        [DataMember, Category("Sensor Properties"), Description("Unique ID of the sensor"), CreateOnly]
        string uniqueid { get; set; }

        /// <summary>
        /// Sensor config.
        /// </summary>
    /*    [DataMember, ExpandableObject, Category("Configuration"), Description("Configuration of the sensor"), CreateOnly]
        TC config { set; get; }

        /// <summary>
        /// Sensor state.
        /// </summary>
        [DataMember, ExpandableObject, Category("State"), Description("State of the sensor"), CreateOnly]
        TS state { set; get; }*/

        [JsonIgnore]
        [DataMember]
        string productid { get; set; }

        [JsonIgnore]
        [DataMember]
        string swconfigid { get;set; }


    }

    public class ISensorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer,value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = serializer.Deserialize<JObject>(reader);
            SensorCreator sc = new SensorCreator();
            ISensor sensor = sc.CreateCreator(obj["type"].Value<string>());
            JsonConvert.PopulateObject(obj.ToString(), sensor, new JsonSerializerSettings(){});
            return sensor;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ISensor);
        }
    }
}
