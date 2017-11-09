using System;
using System.ComponentModel.DataAnnotations;

namespace WinHue3.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RequireHueTapButtonValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            return (string)value == string.Empty ? new ValidationResult(GlobalStrings.HueTap_NoButtonSelected) : ValidationResult.Success;
        }
    }
}
