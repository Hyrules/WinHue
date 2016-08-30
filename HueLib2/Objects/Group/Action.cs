using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// Class for a group action.
    /// </summary>
    [DataContract]
    public class Action : CommonProperties
    {
        /// <summary>
        /// Ctor with CommonProperties
        /// </summary>
        /// <param name="cp"></param>
        public Action(CommonProperties cp)
        {
            PropertyInfo[] prop = typeof(CommonProperties).GetProperties();

            foreach (PropertyInfo p in prop)
            {
                p.SetValue(this, p.GetValue(cp));
            }

        }

        /// <summary>
        /// Ctor
        /// </summary>
        public Action()
        {
            
        }

        /// <summary>
        /// Scene to recall
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), Description("The scene identifier if the scene you wish to recall."), Category("Action Properties"), Browsable(false)]
        public string scene { get; set; }

        /// <summary>
        /// convert state to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }

    
}
