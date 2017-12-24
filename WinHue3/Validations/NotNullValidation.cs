using System;
using System.ComponentModel.DataAnnotations;

namespace WinHue3.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NotNullValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return value != null ? ValidationResult.Success : new ValidationResult(GlobalStrings.HueTap_SelectAScene);
        }
    }
}
