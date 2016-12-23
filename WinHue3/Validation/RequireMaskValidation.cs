using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Resources;
using WinHue3.ViewModels;


namespace WinHue3.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequireMaskAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ScheduleCreatorViewModel scmv = validationContext.ObjectInstance as ScheduleCreatorViewModel;
            if (scmv.SelectedType == "W")
            {
                return (byte)value == 0 ? new ValidationResult(GlobalStrings.Schedule_SelectAtLeastOneDay) : ValidationResult.Success;
            }
            else
            {
                return ValidationResult.Success;
            }
        }

    }
}
