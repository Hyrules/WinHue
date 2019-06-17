using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Interface;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    /// <summary>
    /// Rules.
    /// </summary>
    [DataContract, /*JsonConverter(typeof(RuleJsonConverter))*/]
    public class Rule : ValidatableBindableBase, IHueObject
    {
        private string _name;
        private ImageSource _image;
        private string _id;
        private RuleConditionCollection _conditions;
        private RuleActionCollection _actions;
        private string _owner;
        private int? _timestriggered;
        private string _lasttriggered;
        private string _created;
        private string _status;
        private bool _visible;

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [DataMember, Category("Rule Properties"), Description("Image of the rule"), ReadOnly(true), Browsable(false),JsonIgnore]
        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image,value);
        }

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [DataMember, Category("Rule Properties"), Description("ID of the rule"), ReadOnly(true), JsonIgnore]
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id ,value);
        }

        /// <summary>
        /// name.
        /// </summary>
        [DataMember, Category("Rule Properties"), Description("Name of the rule")]
        public string name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        /// <summary>
        /// Conditions.
        /// </summary>
        [DataMember, Category("Conditions"), Description("Conditions of the rule"), ExpandableObject]
        public RuleConditionCollection conditions
        {
            get => _conditions;
            set => SetProperty(ref _conditions,value);
        }

        /// <summary>
        /// actions.
        /// </summary>
        [DataMember, Category("Actions"), Description("Actions of the rule"), ExpandableObject]
        public RuleActionCollection actions
        {
            get => _actions;
            set => SetProperty(ref _actions,value);
        }

        /// <summary>
        /// Owner of the rule.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"),Description("Owner of the rule"), ReadOnly(true), DontSerialize]
        public string owner
        {
            get => _owner;
            set => SetProperty(ref _owner,value);
        }

        /// <summary>
        /// Number of time triggered.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"),Description("Number of times the rule has been triggered"), ReadOnly(true)]
        public int? timestriggered
        {
            get => _timestriggered;
            set => SetProperty(ref _timestriggered,value);
        }

        /// <summary>
        /// Last time the rule was triggered
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"),Description("Last time the rule was triggered"), ReadOnly(true)]
        public string lasttriggered
        {
            get => _lasttriggered;
            set => SetProperty(ref _lasttriggered,value);
        }

        /// <summary>
        /// Date of creation.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"),Description("Date of creation"), ReadOnly(true)]
        public string created
        {
            get => _created;
            set => SetProperty(ref _created,value);
        }

        /// <summary>
        /// Enabled.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"),Description("Current status of the rule")]
        public string status
        {
            get => _status;
            set => SetProperty(ref _status,value);
        }

        [DataMember(EmitDefaultValue = false, IsRequired = false), ReadOnly(true), JsonIgnore, Browsable(false)]
        public bool visible
        {
            get => _visible;
            set => SetProperty(ref _visible,value);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);
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

        public override bool Equals(object obj)
        {
            return obj is Rule hueobject && hueobject.Id == Id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
