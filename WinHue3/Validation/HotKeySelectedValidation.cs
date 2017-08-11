using System.ComponentModel.DataAnnotations;
using WinHue3.ViewModels;

namespace WinHue3.Validation
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
