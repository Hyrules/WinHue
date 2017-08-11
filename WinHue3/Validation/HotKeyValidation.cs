using System.ComponentModel.DataAnnotations;

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
