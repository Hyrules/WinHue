using System.Runtime.Serialization;
using HueLib2.Objects.Rules;
using Newtonsoft.Json;

namespace HueLib2
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
        public RuleAddress address { get;set; }
        /// <summary>
        /// Method.
        /// </summary>
        [DataMember]
        public string method { get; set; }
        /// <summary>
        /// Body.
        /// </summary>
        [DataMember/*,JsonConverter(typeof(RuleBodyJsonConverter))*/]
        public RuleBody body { get; set; }

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
