using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.RuleObject
{
    /// <summary>
    /// Rules condition.
    /// </summary>
    [JsonObject,ExpandableObject/*, JsonConverter(typeof(RuleConditionJsonConverter))*/]
    public class RuleCondition : ValidatableBindableBase
    {
        private HueAddress _address;
        private string _operator;
        private dynamic _value;

        /// <summary>
        /// Address.
        /// </summary>
        public HueAddress address
        {
            get => _address;
            set => SetProperty(ref _address,value);
        }
        /// <summary>
        /// Operator.
        /// </summary>
        public string @operator
        {
            get => _operator;
            set => SetProperty(ref _operator,value);
        }

        /// <summary>
        /// Value.
        /// </summary>
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
