using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace HueLib2
{
    /// <summary>
    /// Sensor State.
    /// </summary>
    [DataContract]
    public class ClipGenericStatusState : ISensorState, IRuleBody
    {
        /// <summary>
        /// Sensor Status.
        /// </summary>
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public string lastupdated { get; set; }
    }
}
