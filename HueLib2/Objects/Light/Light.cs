using System.CodeDom;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Media;
using HueLib2.Objects.HueObject;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Light Class.
    /// </summary>
    [DefaultProperty("Light"), DataContract]
    public class Light : IHueObject
    {
        private string _name;
        private ImageSource _image;


        /// <summary>
        /// Image
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Image of the light"), ReadOnly(true), Browsable(false)]
        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// State of the Light.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("State"), Description("State of the light"),ExpandableObject, ReadOnly(true)]
        public State state { get; set; }
        /// <summary>
        /// Type of light.
        /// </summary>

        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Type of Light"), ReadOnly(true)]
        public string type { get; set; }

        /// <summary>
        /// Manufacturer name.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Manufacturer name"), ReadOnly(true)]
        public string manufacturername { get; set; }

        /// <summary>
        /// ID of the Light
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("ID"), ReadOnly(true)]
        public string Id { get; set; }

        /// <summary>
        /// Name of the light.
        /// </summary>
        [DataMember(Name = "name",EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Light Name")]
        public string Name {
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
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Model ID"), ReadOnly(true)]
        public string modelid { get; set; }
        /// <summary>
        /// Software Version of the Light.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Software Version"), ReadOnly(true)]
        public string swversion { get; set; }

        /// <summary>
        /// Unique ID of the light.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Unique ID"), ReadOnly(true)]
        public string uniqueid { get; set; }

        /// <summary>
        /// To string.
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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
