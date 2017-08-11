using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HueLib2
{
    /// <summary>
    /// Generic Sensor State.
    /// </summary>
    //[DataContract]
    public interface ISensorState
    {
        /// <summary>
        /// LastUpdated
        /// </summary>
        [DataMember, ReadOnly(true)]
        string lastupdated { get; set; }

        /*public override string ToString()
        {
            JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
            return JsonConvert.SerializeObject(this, jss);
        }*/
    }
}
