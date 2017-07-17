using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using WinHue3.ViewModels;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WinHue3.Validation
{
    public class RuleValueValidation : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RuleConditionViewModel rcvm = validationContext.ObjectInstance as RuleConditionViewModel;
            try
            {
                //Activator.CreateInstance(rcvm.PropType);
                Type currentType;
                currentType = rcvm.PropType.IsNullable() ? Nullable.GetUnderlyingType(rcvm.PropType) : rcvm.PropType;
                Convert.ChangeType(value, currentType);
                rcvm.IsValid = true ;
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                rcvm.IsValid = false;
                string msg = string.Format(ErrorMessageString, rcvm.PropType.Name);
                return new ValidationResult(msg);
            }
            
        }
    }
}
