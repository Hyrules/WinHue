using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HueLib_base
{
    /// <summary>
    /// Scene App Data object
    /// </summary>
    [DataContract,Serializable]
    public class AppData
    {
        /// <summary>
        /// Version info.
        /// </summary>
        [DataMember(IsRequired = false), Category("Apddata Properties"), Description("App specific version of the data field. App should take versioning into account when parsing the data string.")]
        public int? version { get; set; }

        /// <summary>
        /// Free format string.
        /// </summary>
        [DataMember(IsRequired = false), Category("Apddata Properties"), Description("App specific data. Free format string.")]
        public string data { get;set;}

        /// <summary>
        /// Convert the object to Json serialized string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
            return JsonConvert.SerializeObject(this, jss);
        }
    }
}
