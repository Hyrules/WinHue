using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.GroupObject
{
    /// <summary>
    /// Group Class.
    /// </summary>
    [DataContract, HueType("groups")]
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

        /// <summary>
        /// Image of the group.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Image of the group"), Browsable(false), JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        /// <summary>
        /// ID of the group.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("ID of the group"), JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id,value);
        }

        /// <summary>
        /// Action (State) of the group
        /// </summary>
        [DataMember, Category("Action"), Description("Action"), ExpandableObject, ReadOnly(true)]
        public Action action
        {
            get => _action;
            set => SetProperty(ref _action,value);
        }

        [DataMember, Category("State"), Description("State"), ExpandableObject, ReadOnly(true)]
        public GroupState state
        {
            get => _state;
            set => SetProperty(ref _state,value);
        }

        /// <summary>
        /// List of lights in the group.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Lights in the group"), ExpandableObject]
        public StringCollection lights
        {
            get => _lights;
            set => SetProperty(ref _lights,value);
        }

        /// <summary>
        /// Group name.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Name of the group")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        /// <summary>
        /// Type of the group
        /// </summary>
        [DataMember, Category("Group Properties"), Description("The type of group"),CreateOnly]
        public string type
        {
            get => _type;
            set => SetProperty(ref _type,value);
        }

        /// <summary>
        /// Model ID
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Model id of the group"), ReadOnly(true)]
        public string modelid
        {
            get => _modelid;
            set => SetProperty(ref _modelid,value);
        }

        /// <summary>
        /// Unique ID
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Unique id of group"), ReadOnly(true)]
        public string uniqueid
        {
            get => _uniqueid;
            set => SetProperty(ref _uniqueid,value);
        }

        /// <summary>
        /// Class
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Class of the group")]
        public string @class
        {
            get => _class;
            set => SetProperty(ref _class,value);
        }

        /// <summary>
        /// To String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Serializer.SerializeToJson(this);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false), ReadOnly(true), JsonIgnore, Browsable(false)]
        public bool visible
        {
            get => _visible;
            set => SetProperty(ref _visible, value);
        }
    }
}
