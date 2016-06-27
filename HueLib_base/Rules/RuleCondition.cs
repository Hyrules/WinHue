using System.Runtime.Serialization;

namespace HueLib_base
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
        public string address { get; set; }
        /// <summary>
        /// Operator.
        /// </summary>
        [DataMember(Name="operator")]
        public string op { get; set; }
        /// <summary>
        /// Value.
        /// </summary>
        [DataMember]
        public string value { get; set; }
    }
}
