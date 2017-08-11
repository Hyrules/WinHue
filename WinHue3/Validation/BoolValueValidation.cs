using System;
using System.ComponentModel.DataAnnotations;

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
