using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.ViewModels;

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
            catch (Exception e)
            {
                return new ValidationResult(ErrorMessageString);
            }
            return ValidationResult.Success;
        }
    }
}
