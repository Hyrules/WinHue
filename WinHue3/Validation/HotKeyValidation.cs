using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Validation
{
    public class HotKeyValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return value.ToString() != string.Empty
                ? ValidationResult.Success
                : new ValidationResult(GlobalStrings.Hotkey_SelectObject);
        }
    }
}
