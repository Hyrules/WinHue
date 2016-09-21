using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Newtonsoft.Json;

namespace HueLib2
{
    /// <summary>
    /// Class for light State.
    /// </summary>
    [DataContract]
    public class State : CommonProperties
    {
        /// <summary>
        /// Create a state from a CommonProperties
        /// </summary>
        /// <param name="cp">A CommonProperties Object</param>
        public State(CommonProperties cp)
        {
            PropertyInfo[] prop = typeof(CommonProperties).GetProperties();

            foreach (PropertyInfo p in prop)
            {
                p.SetValue(this, p.GetValue(cp));
            }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public State()
        {
            
        }

        /// <summary>
        /// Reachable is the light is reachable ( true is the light is available to control, false if the bridge cannot control the light )
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false), ReadOnly(true), Description("Reachable is the light is reachable ( true is the light is available to control, false if the bridge cannot control the light )"), Category("State Properties")]
        public bool? reachable { get; set; }
   
        /// <summary>
        /// Convert state to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default });
        }
    }
}
