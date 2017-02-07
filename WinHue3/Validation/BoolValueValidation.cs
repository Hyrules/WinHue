using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class BoolValueValidation : ValidationAttribute
    {
        private bool _value;

        public BoolValueValidation(bool value)
        {
            _value = value;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return _value == (bool) value ? ValidationResult.Success : new ValidationResult(ErrorMessageString) ;
        }
    }
}
