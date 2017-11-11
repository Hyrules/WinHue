using System.ComponentModel.DataAnnotations;
using WinHue3.Functions.HotKeys.Creator;

namespace WinHue3.Functions.HotKeys.Validation
{
    public class HotKeySelectedValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HotKeyCreatorViewModel hkvm = (HotKeyCreatorViewModel)validationContext.ObjectInstance;
            if (hkvm.IsGeneric)
            {
                return ValidationResult.Success;
            }
            else
                return value != null ? ValidationResult.Success : new ValidationResult(GlobalStrings.HueTap_SelectAScene); 
        }
    }
}
