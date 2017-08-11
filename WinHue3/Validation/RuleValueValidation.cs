using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WinHue3.ViewModels.RuleCreatorViewModels;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace WinHue3.Validation
{
    public class RuleValueValidation : ValidationAttribute
    {
       
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RuleCreatorConditionViewModel rcvm = validationContext.ObjectInstance as RuleCreatorConditionViewModel;
            if(rcvm.ConditionOperator == "dx") return ValidationResult.Success;
            string msg = string.Empty;
            try
            {
                if (
                    rcvm.ConditionOperator == "eq"
                )
                {
                    if(TestEqual(value.ToString())) return ValidationResult.Success;
                    msg = string.Format(ErrorMessageString, "bool or int");
                }
                if (
                    rcvm.ConditionOperator == "ddx" ||
                    rcvm.ConditionOperator == "not in" ||
                    rcvm.ConditionOperator == "in" ||
                    rcvm.ConditionOperator == "stable" ||
                    rcvm.ConditionOperator == "not stable"
                )
                {
                    if (TestChanged(value.ToString())) {return ValidationResult.Success;}
                    msg = string.Format(ErrorMessageString, "bool, int or Timestamp");
                }

                if (
                    rcvm.ConditionOperator == "lt" ||
                    rcvm.ConditionOperator == "gt"
                )
                {
                    if(TextLtGt(value.ToString())){ return ValidationResult.Success;}
                    msg = string.Format(ErrorMessageString, "int");
                }
                else
                {
                    msg = string.Format(ErrorMessageString, rcvm.ConditionOperator);
                }
               
                return new ValidationResult(msg);
            }
            catch (Exception)
            {
                msg = string.Format(ErrorMessageString, "unknown error");
                return new ValidationResult(msg);
            }
            
        }

        private bool TestTimestamp(string value)
        {
            try
            {
                //[xxx] items are optional
                // MATCH FOR TIME INTEVALS [W127]T23:59:59/T23:59:59
                Regex r = new Regex(@"(W([0-1]2[0-7]|[0-1][0-1][0-9])/)?T([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)/T([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)");
                Match m = r.Match(value);
                if (m.Success) return true;

                // MATCH FOR RECURRING TIMES W126/T23:59:59[A11:59:59]
                r = new Regex(@"(W([0-1]2[0-7]|[0-1][0-1]\d)/)T([0-1]?\d|2[0-3]):([0-5]\d):([0-5]\d)(A(0\d|1[0-1]):([0-5]\d):([0-5]?\d))?");
                m = r.Match(value);
                if (m.Success) return true;

                // MATCH FOR TIMERS [R[nn]/]PT23:59:59[A:11:59:59]
                r = new Regex(@"(R(0\d|[1-9]\d)/)?PT([0-1]\d|[0-2][0-3]):([0-5]\d):([0-5]\d)(A(0\d|1[0-1]):([0-5]\d):([0-5]\d))?");
                m = r.Match(value);
                if (m.Success) return true;

                // MATCH FOR ABSOLUTE TIME YYYY-MM-DDT23:59:59[A11:59:59]
                r = new Regex(@"(\d\d\d\d)-(0\d|1[0-2])-([0-2]\d|3[0-1])T([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)(A(0\d|1[0-1]):([0-5]\d):([0-5]\d))?");
                m = r.Match(value);
                if (m.Success) return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Equal is a int or bool
        /// </summary>
        /// <param name="value">Value to test</param>
        /// <returns>True or false the value is valid.</returns>
        private bool TestEqual(string value)
        {
            try
            {
                if (value.ToLower() == "true" || value.ToLower() == "false") return true;
                Convert.ChangeType(value, TypeCode.Int32);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if dx and ddx for timestamp or int or bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TestChanged(string value)
        {
            try
            {
                if (TestEqual(value)) return true;
                if (TestTimestamp(value)) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if lt and get for int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TextLtGt(string value)
        {
            try
            {
                Convert.ChangeType(value, TypeCode.Int32);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
