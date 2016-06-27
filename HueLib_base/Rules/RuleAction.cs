using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HueLib_base
{
    /// <summary>
    /// Actions.
    /// </summary>
    [DataContract]
    public class RuleAction
    {
        /// <summary>
        /// Address.
        /// </summary>
        [DataMember]
        public string address { get;set; }
        /// <summary>
        /// Method.
        /// </summary>
        [DataMember]
        public string method { get; set; }
        /// <summary>
        /// Body.
        /// </summary>
        [DataMember,JsonConverter(typeof(RuleBodyJsonConverter))]
        public RuleBody body { get; set; }

        /// <summary>
        /// Convert the rule action to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}",address, method);
        }
    }
}
