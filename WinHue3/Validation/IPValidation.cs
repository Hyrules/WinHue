using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Validation
{
    public class IpValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                IPAddress.Parse(value.ToString());
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                return new ValidationResult(GlobalStrings.Invalid_IP);
            }

        }
    }
}
