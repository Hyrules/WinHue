using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Windows.Media;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Interface;
using WinHue3.Philips_Hue.BridgeObject.Entertainment_API;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.GroupObject
{
    /// <summary>
    /// Group Class.
    /// </summary>
    [JsonObject]
    public class Group : ValidatableBindableBase, IHueObject
    {
        private string _name;
        private ImageSource _image;
        private string _id;
        private Action _action;
        private GroupState _state;
        private StringCollection _lights;
        private string _type;
        private string _modelid;
        private string _uniqueid;
        private string _class;
        private bool _visible;
        private StringCollection _sensors;
        private Location _locations;
        private Stream _stream;

        /// <summary>
        /// Image of the group.
        /// </summary>
        [Category("Group Properties"), Description("Image of the group"), Browsable(false), JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        /// <summary>
        /// ID of the group.
        /// </summary>
        [Category("Group Properties"), Description("ID of the group"), JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id,value);
        }

        /// <summary>
        /// Action (State) of the group
        /// </summary>
        [Category("Action"), Description("Action"), ExpandableObject, DontSerialize]
        public Action action
        {
            get => _action;
            set => SetProperty(ref _action,value);
        }

        [Category("State"), Description("State"), ExpandableObject, DontSerialize]
        public GroupState state
        {
            get => _state;
            set => SetProperty(ref _state,value);
        }

        /// <summary>
        /// List of lights in the group.
        /// </summary>
        [Category("Group Properties"), Description("Lights in the group"), ExpandableObject]
        public StringCollection lights
        {
            get => _lights;
            set => SetProperty(ref _lights,value);
        }

        /// <summary>
        /// Group name.
        /// </summary>
        [Category("Group Properties"), Description("Name of the group")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        /// <summary>
        /// Type of the group
        /// </summary>
        [Category("Group Properties"), Description("The type of group"),CreateOnly]
        public string type
        {
            get => _type;
            set => SetProperty(ref _type,value);
        }

        /// <summary>
        /// Model ID
        /// </summary>
        [Category("Group Properties"), Description("Model id of the group"), DontSerialize]
        public string modelid
        {
            get => _modelid;
            set => SetProperty(ref _modelid,value);
        }

        /// <summary>
        /// Unique ID
        /// </summary>
        [Category("Group Properties"), Description("Unique id of group"), DontSerialize]
        public string uniqueid
        {
            get => _uniqueid;
            set => SetProperty(ref _uniqueid,value);
        }

        /// <summary>
        /// Class
        /// </summary>
        [Category("Group Properties"), Description("Class of the group")]
        public string @class
        {
            get => _class;
            set => SetProperty(ref _class,value);
        }

        [Category("Group Properties"), Description("List of sensors in group")]
        public StringCollection sensors
        {
            get => _sensors;
            set => SetProperty(ref _sensors, value);
        }

        [JsonIgnore, Browsable(false)]
        public bool visible
        {
            get => _visible;
            set => SetProperty(ref _visible, value);
        }

        [Category("Streaming"), Description("List of locations of lights in the group (For entertainment mode only)"),ExpandableObject­, DontSerialize]
        public Location Locations
        {
            get => _locations;
            set => SetProperty(ref _locations, value);

        }

        [Category("Streaming"), Description("Streaming status and settings"), ExpandableObject]
        public Stream stream
        {
            get => _stream;
            set => SetProperty(ref _stream, value);
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext ctx)
        {
            RefreshImage();
                

        }

        public void RefreshImage()
        {

            switch (type)
            {
                case "Entertainment":
                    if (state.all_on.GetValueOrDefault())
                        Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.entertainment_on);
                    else if (state.any_on.GetValueOrDefault())
                        Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.entertainment_on);
                    else
                        Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.entertainment_off);
                    break;
                default:
                    if (state.all_on.GetValueOrDefault())
                        Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOn_Large);
                    else if (state.any_on.GetValueOrDefault())
                        Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupSome_Large);
                    else
                        Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOff_Large);
                    break;
            }

        }

        public override bool Equals(object obj)
        {
            return obj is Group hueObject && hueObject.Id == Id;
        }

        /// <summary>
        /// To String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }

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
