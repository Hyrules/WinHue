using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Rules.
    /// </summary>
    [DataContract]
    public class Rule : HueObject
    {
        private string _name;
        /// <summary>
        /// name.
        /// </summary>
        [DataMember, Category("Rule Properties"), Description("Name of the rule"), HueLib(true, true)]
        public string name
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
        [DataMember, Category("Conditions"), Description("Conditions of the rule"),Browsable(false), HueLib(true, true)]
        public List<RuleCondition> conditions { get; set; }
        /// <summary>
        /// actions.
        /// </summary>
        [DataMember, Category("Actions"), Description("Actions of the rule"), Browsable(false), HueLib(true, true)]
        public List<RuleAction> actions { get; set; }

        /// <summary>
        /// Owner of the rule.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Owner of the rule"), HueLib(false, false)]
        public string owner { get; set; }

        /// <summary>
        /// Number of time triggered.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Number of times the rule has been triggered"), HueLib(false, false)]
        public int? timestriggered { get; set; }

        /// <summary>
        /// Last time the rule was triggered
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Last time the rule was triggered"), HueLib(false, false)]
        public string lasttriggered { get; set; }

        /// <summary>
        /// Date of creation.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Date of creation"), HueLib(false, false)]
        public string created { get; set; }

        /// <summary>
        /// Enabled.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Rule Properties"), Description("Current status of the rule"), HueLib(true, true)]
        public string status { get; set; }

        /// <summary>
        /// To String.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }

    }
}
