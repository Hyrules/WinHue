using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.ViewModels;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.SceneObject
{
    /// <summary>
    /// Scene Class.
    /// </summary>
    [DataContract, HueType("scenes")]
    public class Scene : ValidatableBindableBase, IHueObject
    {
        private string _name;
        private ImageSource _image;
        private string _id;
        private List<string> _lights;
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

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [DataMember, Category("Scene Properties"), Description("Image of the Scene"), ReadOnly(true), Browsable(false), JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [DataMember, Category("Scene Properties"), Description("ID of the Scene"), ReadOnly(true), JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id,value);
        }

        /// <summary>
        /// Name of the scene.
        /// </summary>
        [HueProperty, DataMember, Category("Scene Properties"), Description("Name of the scene")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        /// <summary>
        /// List of the light in the scene. 
        /// </summary>
        [HueProperty, DataMember, Category("Scene Properties"), Description("Lights of the scene"), Browsable(false)]
        public List<string> lights
        {
            get => _lights;
            set => SetProperty(ref _lights, value);
        }

        /// <summary>
        /// Owner of the scene.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"),Description("Whitelist user that created or modified the content of the scene"), ReadOnly(true)]
        public string owner
        {
            get => _owner;
            set => SetProperty(ref _owner,value);
        }

        /// <summary>
        /// App specific data.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"),Description("App specific data linked to the scene."), ExpandableObject, ReadOnly(true)]
        public AppData appdata
        {
            get => _appdata;
            set => SetProperty(ref _appdata,value);
        }

        /// <summary>
        /// Scene can be deleted by the bridge automatically.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"),Description("Indicates whether the scene can be automatically deleted by the bridge."),CreateOnly]
        public bool? recycle
        {
            get => _recycle;
            set => SetProperty(ref _recycle,value);
        }

        /// <summary>
        /// Scene is Locked.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"),Description("Indicates that the scene is locked by a rule or a schedule and cannot be deleted until all resources requiring or that reference the scene are deleted."),ReadOnly(true)]
        public bool? locked
        {
            get => _locked;
            set => SetProperty(ref _locked,value);
        }

        /// <summary>
        /// Picture path
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"), Description("Path to the picture.")]
        public string picture
        {
            get => _picture;
            set => SetProperty(ref _picture,value);
        }

        /// <summary>
        /// Version of the scene.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"), Description("Version of scene document."),ReadOnly(true)]
        public int? version
        {
            get => _version;
            set => SetProperty(ref _version,value);
        }

        /// <summary>
        /// Last time the scene was updated in UTC.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"),Description("UTC time the scene has been created or has been updated."), ReadOnly(true)]
        public string lastupdated
        {
            get => _lastupdated;
            set => SetProperty(ref _lastupdated,value);
        }

        /// <summary>
        /// State of the lights in the scene.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"),Description("States of every lights in the scene."), Browsable(false), ReadOnly(true)]
        public Dictionary<string, State> lightstates
        {
            get => _lightstates;
            set => SetProperty(ref _lightstates,value);
        }

        /// <summary>
        /// Store current light state in scene.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"),Description("Store the current light state in the scene."), Browsable(false)]
        public bool? storelightstate
        {
            get => _storelightstate;
            set => SetProperty(ref _storelightstate,value);
        }

        /// <summary>
        /// Transition time of the scene.
        /// </summary>
        [HueProperty, DataMember(IsRequired = false), Category("Scene Properties"), Description("Transition time of the scene.")]
        public ushort? transitiontime
        {
            get => _transitiontime;
            set => SetProperty(ref _transitiontime,value);
        }

        /// <summary>
        /// To String.
        /// </summary>
        public override string ToString()
        {
            return Serializer.SerializeToJson(this);

        }


        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
