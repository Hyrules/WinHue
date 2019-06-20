using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Media;
using log4net;
using Newtonsoft.Json;
using WinHue3.Functions.Lights.SupportedDevices;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;


namespace WinHue3.Philips_Hue.HueObjects.LightObject
{
    /// <summary>
    /// Light Class.
    /// </summary>
    [DefaultProperty("Light"), JsonObject]
    public class Light : ValidatableBindableBase, IHueObject
    {
        /// <summary>
        /// Logging 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
        [Category("State"),Description("State of the light"), ExpandableObject, DontSerialize,ReadOnly(true)]
        public State state { get=> _state; set => SetProperty(ref _state, value); }

        /// <summary>
        /// Type of light.
        /// </summary>

        [Category("Light Properties"),Description("Type of Light"),DontSerialize]
        public string type { get => _type; set => SetProperty(ref _type, value); }

        /// <summary>
        /// Manufacturer name.
        /// </summary>
        
        [Category("Light Properties"),Description("Manufacturer name"), DontSerialize]
        public string manufacturername { get => _manufacturername ; set => SetProperty(ref _manufacturername,value); }

        /// <summary>
        /// Name of the light.
        /// </summary>
        [Category("Light Properties"), Description("Light Name")]
        public string name { get => _name; set => SetProperty(ref _name,value); }

        /// <summary>
        /// Model ID of the Light.
        /// </summary>
        [Category("Light Properties"), Description("Model ID"), DontSerialize]
        public string modelid { get => _modelid; set => SetProperty(ref _modelid, value); }

        /// <summary>
        /// Software Version of the Light.
        /// </summary>
        [Category("Light Properties"), Description("Software Version"), DontSerialize]
        public string swversion { internal get => _swversion; set => SetProperty(ref _swversion, value); }

        /// <summary>
        /// Unique ID of the light.
        /// </summary>
        [Category("Light Properties"), Description("Unique ID"), DontSerialize]
        public string uniqueid { get => _uniqueid; set => SetProperty(ref _uniqueid,value); }

        /// <summary>
        /// Luminaire unique ID of the light.
        /// </summary>
        [Category("Light Properties"), Description("Luminaire Unique ID"), DontSerialize]
        public string luminaireuniqueid { get => _luminaireuniqueid; set => SetProperty(ref _luminaireuniqueid, value); }

        /// <summary>
        /// Light Visibility.
        /// </summary>
        [Browsable(false), JsonIgnore]
        public bool visible { get => _visible; set => SetProperty(ref _visible,value);}

        /// <summary>
        /// Capabilities of the light.
        /// </summary>
        [Category("Light Properties"), Description("Capabilities"),  ExpandableObject, DontSerialize]
        public LightCapabilities capabilities { get => _capabilities; set => SetProperty(ref _capabilities, value); }

        /// <summary>
        /// Config of the light.
        /// </summary>
        [Category("Light Properties"), Description("Config"),  ExpandableObject, DontSerialize]
        public LightConfig config { get => _config; set => SetProperty(ref _config, value); }

        /// <summary>
        /// Light software update.
        /// </summary>
        [Category("Light Properties"), Description("Software Update"),  ExpandableObject, DontSerialize]
        public LightSwUpdate swupdate { get => _swupdate; set => SetProperty(ref _swupdate,value); }

        /// <summary>
        /// Light streaming capabilities
        /// </summary>
        [Category("Light Properties"), Description("Current light supports streaming features"), DontSerialize]
        public bool Streaming { get => _streaming; set => SetProperty(ref _streaming,value); }

        [OnDeserialized]
        void OnDeserialized(StreamingContext ctx)
        {
            RefreshImage();
        }

        public override bool Equals(object obj)
        {
            return obj is Light hueobject && hueobject.Id == Id;
        }

        public void RefreshImage()
        {
            if (state?.on != null)
                Image = GetImageForLight(modelid, config?.archetype);
        }

        /// <summary>
        /// Return the new image from the light
        /// </summary>
        /// <param name="modelid">model id of the light.</param>
        /// <param name="archetype">Archetype of the light.</param>
        /// <returns>New image of the light</returns>
        private ImageSource GetImageForLight(string modelid = null, string archetype = null)
        {
            string modelID = modelid ?? "DefaultHUE";

            if (modelID == string.Empty)
            {
                log.Debug("STATE : " + state + " empty MODELID using default images");
                return LightImageLibrary.Images["DefaultHUE"][state.on.GetValueOrDefault()];
            }

            ImageSource newImage;

            if (LightImageLibrary.Images.ContainsKey(modelID)) // Check model ID first
            {
                log.Debug("STATE : " + state + " MODELID : " + modelID);
                newImage = LightImageLibrary.Images[modelID][state.on.GetValueOrDefault()];

            }
            else if (archetype != null && LightImageLibrary.Images.ContainsKey(archetype)) // Check archetype after model ID, giving model ID priority
            {
                log.Debug("STATE : " + state + " ARCHETYPE : " + archetype);
                newImage = LightImageLibrary.Images[archetype][state.on.GetValueOrDefault()];
            }
            else // Neither model ID or archetype are known
            {
                log.Debug("STATE : " + state + " unknown MODELID : " + modelID + " and ARCHETYPE : " + archetype + " using default images.");
                newImage = LightImageLibrary.Images["DefaultHUE"][state.on.GetValueOrDefault()];
            }
            return newImage;
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Return an exact copy of the object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
