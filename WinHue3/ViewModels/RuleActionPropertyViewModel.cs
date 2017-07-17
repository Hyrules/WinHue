using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.ViewModels
{
    public class RuleActionPropertyViewModel : ValidatableBindableBase
    {
        private Type _propType;
        private string _propName;
        private string _value;
        private bool _isValid;

        public RuleActionPropertyViewModel()
        {
            _propName = string.Empty;
            _isValid = false;
        }

        public Type PropType
        {
            get { return _propType; }
            set { SetProperty(ref _propType, value); }
        }

        public string PropName
        {
            get { return _propName; }
            set { SetProperty(ref _propName,value); }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value,value); }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set { SetProperty(ref _isValid,value); }
        }
    }
}
