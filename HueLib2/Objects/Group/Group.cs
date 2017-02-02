using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Markup;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Group Class.
    /// </summary>
    [DataContract, DefaultProperty("Group")]
    public class Group : HueObject
    {
        private string _name;

        /// <summary>
        /// Action (State) of the group
        /// </summary>
        [DataMember, Category("Action"), Description("Action"),ExpandableObject, ReadOnly(true)]
        public Action action { get; set; }
        /// <summary>
        /// List of lights in the group.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Lights in the group"),Browsable(false)]
        public List<string> lights { get; set;}
        /// <summary>
        /// Group name.
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Name of the group")]
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
        /// Type of the group
        /// </summary>
        [DataMember, Category("Group Properties"), Description("The type of group"), CreateOnly]
        public string type { get; set; }

        /// <summary>
        /// Model ID
        /// </summary>
        [JsonIgnore]
        [DataMember, Category("Group Properties"), Description("Model id of the group")]
        public string modelid { get; set; }

        /// <summary>
        /// Unique ID
        /// </summary>
        [JsonIgnore]
        [DataMember, Category("Group Properties"), Description("Unique id of group")]
        public string uniqueid { get; set; }

        /// <summary>
        /// Class
        /// </summary>
        [DataMember, Category("Group Properties"), Description("Class of the group")]
        public string @class { get; set; }

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
