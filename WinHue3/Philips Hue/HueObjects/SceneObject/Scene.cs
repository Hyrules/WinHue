using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Interface;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.SceneObject
{
    /// <summary>
    /// Scene Class.
    /// </summary>
    [JsonObject]
    public class Scene : ValidatableBindableBase, IHueObject
    {
        private string _name;
        private ImageSource _image;
        private string _id;
        private StringCollection _lights;
        private string _owner;
        private AppData _appdata;
        private bool? _recycle;
        private bool? _locked;
        private string _picture;
        private int? _version;
        private string _lastupdated;
        private Dictionary<string, State> _lightstates;
        private bool? _storelightstate;
        private ushort? _transitiontime;
        private bool _visible;
        private bool _on;
        private string _type;
        private string _group;

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [Category("Scene Properties"), Description("Image of the Scene"),  Browsable(false), JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [Category("Scene Properties"), Description("ID of the Scene"),  JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id,value);
        }


        /// <summary>
        /// Name of the scene.
        /// </summary>
        [Category("Scene Properties"), Description("Name of the scene")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        /// <summary>
        /// List of the light in the scene. 
        /// </summary>
        [Category("Scene Properties"), Description("Lights of the scene")]
        public StringCollection lights
        {
            get => _lights;
            set => SetProperty(ref _lights, value);
        }

        /// <summary>
        /// Owner of the scene.
        /// </summary>
        [Category("Scene Properties"),Description("Whitelist user that created or modified the content of the scene"),DontSerialize]
        public string owner
        {
            get => _owner;
            set => SetProperty(ref _owner,value);
        }

        /// <summary>
        /// App specific data.
        /// </summary>
        [Category("Scene Properties"),Description("App specific data linked to the scene."), ExpandableObject]
        public AppData appdata
        {
            get => _appdata;
            set => SetProperty(ref _appdata,value);
        }

        /// <summary>
        /// Scene can be deleted by the bridge automatically.
        /// </summary>
        [Category("Scene Properties"),Description("Indicates whether the scene can be automatically deleted by the bridge."),CreateOnly]
        public bool? recycle
        {
            get => _recycle;
            set => SetProperty(ref _recycle,value);
        }

        /// <summary>
        /// Scene is Locked.
        /// </summary>
        [Category("Scene Properties"),Description("Indicates that the scene is locked by a rule or a schedule and cannot be deleted until all resources requiring or that reference the scene are deleted."), DontSerialize]
        public bool? locked
        {
            get => _locked;
            set => SetProperty(ref _locked,value);
        }

        /// <summary>
        /// Picture path
        /// </summary>
        [Category("Scene Properties"), Description("Path to the picture.")]
        public string picture
        {
            get => _picture;
            set => SetProperty(ref _picture,value);
        }

        /// <summary>
        /// Version of the scene.
        /// </summary>
        [Category("Scene Properties"), Description("Version of scene document."),DontSerialize]
        public int? version
        {
            get => _version;
            set => SetProperty(ref _version,value);
        }

        /// <summary>
        /// Last time the scene was updated in UTC.
        /// </summary>
        [Category("Scene Properties"),Description("UTC time the scene has been created or has been updated."), DontSerialize]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated,value);
        }

        /// <summary>
        /// State of the lights in the scene.
        /// </summary>
        [Category("Scene Properties"),Description("States of every lights in the scene."), Browsable(false)]
        public Dictionary<string, State> lightstates
        {
            get => _lightstates;
            set => SetProperty(ref _lightstates,value);
        }

        /// <summary>
        /// Store current light state in scene.
        /// </summary>
        [Category("Scene Properties"),Description("Store the current light state in the scene."), Browsable(false)]
        public bool? storelightstate
        {
            get => _storelightstate;
            set => SetProperty(ref _storelightstate,value);
        }

        /// <summary>
        /// Transition time of the scene.
        /// </summary>
        [Category("Scene Properties"), Description("Transition time of the scene.")]
        public ushort? transitiontime
        {
            get => _transitiontime;
            set => SetProperty(ref _transitiontime,value);
        }

        [JsonIgnore, Browsable(false)]
        public bool visible
        {
            get => _visible;
            set { SetProperty(ref _visible,value); }
        }

        [JsonIgnore, Browsable(false)]
        public bool On
        {
            get => _on;
            set => SetProperty(ref _on, value);
        }

        [Category("Scene Properties"),Description("Type of the scene"),ItemsSource(typeof(GroupeTypeItemSource))]
        public string type
        {
            get => _type; 
            set => SetProperty(ref _type,value);
        }

        [Category("Scene Properties"),Description("Group ID that a scene is linked to.")]
        public string group
        {
            get => _group; 
            set => SetProperty(ref _group,value);
        }

        /// <summary>
        /// To String.
        /// </summary>
        public override string ToString()
        {
            return name;

        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext ctx)
        {
            Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.scenes);
            if (appdata.data is null && appdata.version is null)
                appdata = null;
        }


        public object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            return obj is Scene hueobject && hueobject.Id == Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
