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
        private RuleAddress _address;
        private string _operator;
        private dynamic _value;

        /// <summary>
        /// Address.
        /// </summary>
        [DataMember]
        public RuleAddress address
        {
            get => _address;
            set => _address = value;
        }
        /// <summary>
        /// Operator.
        /// </summary>
        [DataMember]
        public string @operator
        {
            get => _operator;
            set => _operator = value;
        }

        /// <summary>
        /// Value.
        /// </summary>
        [DataMember]
        public dynamic value
        {
            get => _value;
            set => _value = value;
        }

        public override string ToString()
        {
            return $"{address} {@operator} {value}";
        }
    }
}
