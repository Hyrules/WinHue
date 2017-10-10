using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    /// <summary>
    /// Actions.
    /// </summary>
    [DataContract,JsonConverter(typeof(RuleActionJsonConverter)),ExpandableObject]
    public class RuleAction
    {
        /// <summary>
        /// Address.
        /// </summary>
        [HueProperty, DataMember]
        public HueAddress address { get; set; }
        /// <summary>
        /// Method.
        /// </summary>
        [HueProperty, DataMember]
        public string method { get; set; }
        /// <summary>
        /// Body.
        /// </summary>
        [HueProperty, DataMember]
        public string body { get; set; }

        /// <summary>
        /// Convert the rule action to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{address} {method} {body}";
        }
    }
}
