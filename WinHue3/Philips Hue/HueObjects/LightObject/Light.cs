using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;


namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    /// <summary>
    /// Light Class.
    /// </summary>
    [DefaultProperty("Light"), JsonObject, HueType("lights")]
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
        private bool _visible;
        private bool _streaming;
        private LightCapabilities _capabilities;
        private LightConfig _config;
        private LightSwUpdate _swupdate;

        /// <summary>
        /// ID of the Light
        /// </summary>
        [Category("Light Properties"), Description("ID"),  JsonIgnore]
        public string Id { get => _id; set => SetProperty(ref _id,value); }

        /// <summary>
        /// Image
        /// </summary>
        [Category("Light Properties"),Description("Image of the light"),  Browsable(false), JsonIgnore]
        public ImageSource Image { get => _image; set => SetProperty(ref _image, value); }

        /// <summary>
        /// State of the Light.
        /// </summary>
        [Category("State"),Description("State of the light"), ExpandableObject]
        public State state { get=> _state; set => SetProperty(ref _state, value); }

        /// <summary>
        /// Type of light.
        /// </summary>

        [Category("Light Properties"),Description("Type of Light")]
        public string type { get => _type; set => SetProperty(ref _type, value); }

        /// <summary>
        /// Manufacturer name.
        /// </summary>
        
        [Category("Light Properties"),Description("Manufacturer name")]
        public string manufacturername { get => _manufacturername ; set => SetProperty(ref _manufacturername,value); }

        /// <summary>
        /// Name of the light.
        /// </summary>
        [Category("Light Properties"), Description("Light Name")]
        public string name { get => _name; set => SetProperty(ref _name,value); }

        /// <summary>
        /// Model ID of the Light.
        /// </summary>
        [Category("Light Properties"), Description("Model ID")]
        public string modelid { get => _modelid; set => SetProperty(ref _modelid, value); }

        /// <summary>
        /// Software Version of the Light.
        /// </summary>
        [Category("Light Properties"), Description("Software Version")]
        public string swversion { internal get => _swversion; set => SetProperty(ref _swversion, value); }

        /// <summary>
        /// Unique ID of the light.
        /// </summary>
        [Category("Light Properties"), Description("Unique ID")]
        public string uniqueid { get => _uniqueid; set => SetProperty(ref _uniqueid,value); }

        /// <summary>
        /// Luminaire unique ID of the light.
        /// </summary>
        [Category("Light Properties"), Description("Luminaire Unique ID")]
        public string luminaireuniqueid { get => _luminaireuniqueid; set => SetProperty(ref _luminaireuniqueid, value); }

        /// <summary>
        /// Light Visibility.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public bool visible { get => _visible; set => SetProperty(ref _visible,value);}

        /// <summary>
        /// Capabilities of the light.
        /// </summary>
        [Category("Light Properties"), Description("Capabilities"),  ExpandableObject]
        public LightCapabilities capabilities { get => _capabilities; set => SetProperty(ref _capabilities, value); }

        /// <summary>
        /// Config of the light.
        /// </summary>
        [Category("Light Properties"), Description("Config"),  ExpandableObject]
        public LightConfig config { get => _config; set => SetProperty(ref _config, value); }

        /// <summary>
        /// Light software update.
        /// </summary>
        [Category("Light Properties"), Description("Software Update"),  ExpandableObject]
        public LightSwUpdate swupdate { get => _swupdate; set => SetProperty(ref _swupdate,value); }

        /// <summary>
        /// Light streaming capabilities
        /// </summary>
        [Category("Light Properties"), Description("Current light supports streaming features")]
        public bool Streaming { get => _streaming; set => SetProperty(ref _streaming,value); }

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
