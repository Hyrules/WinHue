using System.CodeDom;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Light Class.
    /// </summary>
    [DefaultProperty("Light"), DataContract]
    public class Light : HueObject
    {
        private string _name;
        /// <summary>
        /// State of the Light.
        /// </summary>
        [JsonIgnore]
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("State"), Description("State of the light"),ExpandableObject]
        public State state { get; set; }
        /// <summary>
        /// Type of light.
        /// </summary>
        [JsonIgnore]
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Type of Light")]
        public string type { get; set; }

        /// <summary>
        /// Manufacturer name.
        /// </summary>
        [JsonIgnore]
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Manufacturer name")]
        public string manufacturername { get; set; }

        /// <summary>
        /// Name of the light.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Light Name")]
        public string name {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Model ID of the Light.
        /// </summary>
        [JsonIgnore]
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Model ID")]
        public string modelid { get; set; }
        /// <summary>
        /// Software Version of the Light.
        /// </summary>
        [JsonIgnore]
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Software Version")]
        public string swversion { get; set; }

        /// <summary>
        /// Unique ID of the light.
        /// </summary>
        [JsonIgnore]
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Unique ID")]
        public string uniqueid { get; set; }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }



    }
}
