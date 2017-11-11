using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WinHue3.Functions.BridgeSettings;

namespace WinHue3.Validations
{
    public class RequireIPValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            BridgeSettingsViewModel _bsvm = validationContext.ObjectInstance as BridgeSettingsViewModel;
            if(_bsvm.NetworkModel.Dhcp)
            {
                return ValidationResult.Success;
            }
            else
            {
                try
                {
                    IPAddress.Parse(value.ToString());
                    return ValidationResult.Success;
                }
                catch(Exception)
                {
                    return new ValidationResult(GlobalStrings.Invalid_IP);
                }
            }
        }

    }
}
