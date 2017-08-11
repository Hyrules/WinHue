using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    /// <summary>
    /// Rules condition.
    /// </summary>
    [DataContract]
    public class RuleCondition : ValidatableBindableBase
    {
        private HueAddress _address;
        private string _operator;
        private dynamic _value;

        /// <summary>
        /// Address.
        /// </summary>
        [HueProperty, DataMember]
        public HueAddress address
        {
            get => _address;
            set => SetProperty(ref _address,value);
        }
        /// <summary>
        /// Operator.
        /// </summary>
        [HueProperty, DataMember]
        public string @operator
        {
            get => _operator;
            set => SetProperty(ref _operator,value);
        }

        /// <summary>
        /// Value.
        /// </summary>
        [HueProperty, DataMember]
        public dynamic value
        {
            get => _value;
            set => SetProperty(ref _value,value);
        }

        public override string ToString()
        {
            return $"{address} {@operator} {value}";
        }
    }
}
