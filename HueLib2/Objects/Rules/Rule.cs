using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Media;
using HueLib2.Objects.HueObject;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using HueLib2.Objects.Rules;

namespace HueLib2
{
    /// <summary>
    /// Rules.
    /// </summary>
    [DataContract, JsonConverter(typeof(RuleJsonConverter))]
    public class Rule : IHueObject
    {
        private string _name;
        private ImageSource _image;

        /// <summary>
        /// Image of the rule.
        /// </summary>
        [DataMember, Category("Rule Properties"), Description("Image of the rule"), ReadOnly(true), Browsable(false)]
        public ImageSource Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// ID of the rule.
        /// </summary>
        [DataMember, Category("Rule Properties"), Description("ID of the rule"), ReadOnly(true)]
        public string Id { get; set; }

        /// <summary>
        /// name.
        /// </summary>
        [DataMember(Name = "name"), Category("Rule Properties"), Description("Name of the rule")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Conditions.
        /// </summary>
        [DataMember, Category("Conditions"), Description("Conditions of the rule"),ExpandableObject]
        public List<RuleCondition> conditions { get; set; }
        /// <summary>
        /// actions.
        /// </summary>
        [DataMember, Category("Actions"), Description("Actions of the rule"),ExpandableObject]
        public List<RuleAction> actions { get; set; }

        /// <summary>
        /// Owner of the rule.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Owner of the rule"), ReadOnly(true)]
        public string owner { get; set; }

        /// <summary>
        /// Number of time triggered.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Number of times the rule has been triggered"), ReadOnly(true)]
        public int? timestriggered { get; set; }

        /// <summary>
        /// Last time the rule was triggered
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Last time the rule was triggered"), ReadOnly(true)]
        public string lasttriggered { get; set; }

        /// <summary>
        /// Date of creation.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Date of creation"), ReadOnly(true)]
        public string created { get; set; }

        /// <summary>
        /// Enabled.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Current status of the rule")]
        public string status { get; set; }

        /// <summary>
        /// To String.
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
