using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace WinHue3.ViewModels
{
    public class ValidatableBindableBase : BindableBase, IDataErrorInfo, IChangeTracking
    {
        private ValidationContext _validationContext { get; set; }
        private bool _isChanged;

        protected ValidatableBindableBase()
        {
            this._validationContext = new ValidationContext(this);
        }

        [Browsable(false),JsonIgnore]
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
                        v.Validate(prop.GetValue(this, null), _validationContext);
                    }
                    catch (Exception)
                    {
                        errormessages.Add((string)v.GetType().GetProperty("ErrorMessageString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(v));
                    }
   
                }

                return errormessages.Count == 0 ? null : string.Join(",", errormessages);
            }
        }
        


        [Browsable(false),JsonIgnore]
        public string Error { get; internal set; }

        [Browsable(false),JsonIgnore]
        public bool IsChanged
        {
            get => _isChanged;
            private set => _isChanged =value;
        }

        public void AcceptChanges()
        {
            this.IsChanged = false;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.IsChanged = true;
            base.OnPropertyChanged(args);
        }
    }
}