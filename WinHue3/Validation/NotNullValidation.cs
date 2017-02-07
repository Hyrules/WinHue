using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;

namespace WinHue3.Validation
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
