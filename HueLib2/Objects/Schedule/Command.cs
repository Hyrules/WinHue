using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Command of the schedule.
    /// </summary>
    [DataContract, Serializable]
    public class Command
    {
        /// <summary>
        /// Put since you are modifiying a state or action.
        /// </summary>
        [DataMember]
        public string method { get; set; }
        /// <summary>
        /// // Address of the command.
        /// </summary>
        [DataMember]
        public string address { get; set; }
        /// <summary>
        /// See Body Class
        /// </summary>
        [DataMember, ExpandableObject]
        public Body body {get; set; }

        /// <summary>
        /// Override to string function.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
            return JsonConvert.SerializeObject(this, jss);
        }
    }
}
