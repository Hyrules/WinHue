using System.Runtime.Serialization;
using HueLib2.Objects.Rules;

namespace HueLib2
{
    /// <summary>
    /// Rules condition.
    /// </summary>
    [DataContract]
    public class RuleCondition
    {
        /// <summary>
        /// Address.
        /// </summary>
        [DataMember]
        public RuleAddress address { get; set; }
        /// <summary>
        /// Operator.
        /// </summary>
        [DataMember]
        public string @operator { get; set; }
        /// <summary>
        /// Value.
        /// </summary>
        [DataMember]
        public dynamic value { get; set; }

        public override string ToString()
        {
            return $"{address} {@operator} {value}";
        }
    }
}
