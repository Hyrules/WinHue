using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    [DefaultProperty("Resource Links"), DataContract]
    public class Resourcelink : HueObject
    {
        /// <summary>
        /// Name of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Name of the resource link")]
        public string name { get; set; }

        /// <summary>
        /// Description of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Description of the resource link")]
        public string description { get; set; }

        /// <summary>
        /// Class of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Class of the resource link")]
        public int @class { get; set; }

        /// <summary>
        /// Owner of the resource link
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("Owner of the resource link")]
        public string owner { get; set; }

        /// <summary>
        /// List of resource links
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Category("Resource Link"), Description("List of resource links"),ExpandableObject]
        public List<string> links { get; set; }
    }
}
