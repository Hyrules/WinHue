using System;
using System.ComponentModel.DataAnnotations;

namespace WinHue3.Validation
{
    public class UIntValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if((string)value == string.Empty) return ValidationResult.Success;
            try
            {
                System.Convert.ToUInt16(value);
            }
            catch (Exception)
            {
                return new ValidationResult(ErrorMessageString);
            }
            return ValidationResult.Success;
        }
    }
}
