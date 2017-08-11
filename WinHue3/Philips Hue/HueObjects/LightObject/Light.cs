using System.ComponentModel;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;


namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    /// <summary>
    /// Light Class.
    /// </summary>
    [DefaultProperty("Light"), DataContract, HueType("lights")]
    public class Light : ValidatableBindableBase, IHueObject
    {
        private string _name;
        private ImageSource _image;
        private string _type;
        private State _state;
        private string _manufacturername;
        private string _id;
        private string _modelid;
        private string _swversion;
        private string _uniqueid;
        private string _luminaireuniqueid;

        /// <summary>
        /// ID of the Light
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("ID"), ReadOnly(true), JsonIgnore]
        public string Id { get => _id; set => SetProperty(ref _id,value); }

        /// <summary>
        /// Image
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Image of the light"), ReadOnly(true), Browsable(false), JsonIgnore]
        public ImageSource Image { get => _image; set => SetProperty(ref _image, value); }

        /// <summary>
        /// State of the Light.
        /// </summary>
        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("State"),Description("State of the light"), ExpandableObject, ReadOnly(true)]
        public State state { get=> _state; set => SetProperty(ref _state, value); }

        /// <summary>
        /// Type of light.
        /// </summary>

        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"),Description("Type of Light"), ReadOnly(true)]
        public string type { get => _type; set => SetProperty(ref _type, value); }

        /// <summary>
        /// Manufacturer name.
        /// </summary>
        
        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"),Description("Manufacturer name"), ReadOnly(true), ]
        public string manufacturername { get => _manufacturername ; set => SetProperty(ref _manufacturername,value); }

        /// <summary>
        /// Name of the light.
        /// </summary>
        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Light Name")]
        public string name { get => _name; set => SetProperty(ref _name,value); }

        /// <summary>
        /// Model ID of the Light.
        /// </summary>
        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Model ID"), ReadOnly(true)]
        public string modelid { get => _modelid; set => SetProperty(ref _modelid, value); }

        /// <summary>
        /// Software Version of the Light.
        /// </summary>
        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Software Version"), ReadOnly(true)]
        public string swversion { get => _swversion; set => SetProperty(ref _swversion, value); }

        /// <summary>
        /// Unique ID of the light.
        /// </summary>
        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Unique ID"), ReadOnly(true)]
        public string uniqueid { get => _uniqueid; set => SetProperty(ref _uniqueid,value); }

        /// <summary>
        /// Luminaire unique ID of the light.
        /// </summary>
        [HueProperty, DataMember(EmitDefaultValue = false, IsRequired = false), Category("Light Properties"), Description("Luminaire Unique ID"), ReadOnly(true)]
        public string luminaireuniqueid { get => _luminaireuniqueid; set => SetProperty(ref _luminaireuniqueid, value); }


        /// <summary>
        /// To string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }

        /// <summary>
        /// Return an exact copy of the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

    }
}
