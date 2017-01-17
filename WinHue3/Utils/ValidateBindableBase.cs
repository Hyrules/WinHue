using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Prism.Mvvm;

namespace WinHue3
{
    public class ValidatableBindableBase : BindableBase, IDataErrorInfo
    {
        private ValidationContext validationContext { get; set; }

        protected ValidatableBindableBase()
        {
            this.validationContext = new ValidationContext(this);
        }

        public string this[string propertyName]
        {
            get
            {
                var prop = GetType().GetProperty(propertyName);
                var validationMap = prop.GetCustomAttributes(typeof(ValidationAttribute), true).Cast<ValidationAttribute>();
                List<string> errormessages = new List<string>();
                foreach (var v in validationMap)
                {
                    try
                    {
                        v.Validate(prop.GetValue(this, null), validationContext);
                    }
                    catch (Exception)
                    {
                        errormessages.Add((string)v.GetType().GetProperty("ErrorMessageString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(v));
                    }
   
                }

                return errormessages.Count == 0 ? null : string.Join(",", errormessages);
            }
        }
        
        public string Error { get; internal set; }

    }
}