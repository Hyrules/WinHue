using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WinHue3.ViewModels;

namespace WinHue3
{
    public abstract class View : ValidatableBindableBase , INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public Dictionary<string, List<string>> Errors => _errors;

        #region //************************ PROPERTIES *****************************************

        public bool HasErrors
        {
            get
            {
                return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0);
            }
        }

        #endregion

        #region //************************* METHODS ********************************************

        public void SetError(string errormsg, [CallerMemberName] string propertyName = null)
        {
            if(_errors.ContainsKey(propertyName))
            {
                if(!_errors[propertyName].Contains(errormsg))
                    _errors[propertyName].Add(errormsg);
            }
            else
            {
                _errors.Add(propertyName, new List<string>() { errormsg });
            }

            OnErrorChanged(propertyName);
        }

        public void RemoveError(string errormsg,[CallerMemberName]string propertyName = null)
        {
            if(_errors.ContainsKey(propertyName))
            {
                if(_errors[propertyName].Contains(errormsg))
                {
                    _errors[propertyName].Remove(errormsg);
                }

                if(_errors[propertyName].Count == 0)
                {
                    _errors.Remove(propertyName);
                }
                OnErrorChanged(propertyName);
            }
            
        }

        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (_errors.ContainsKey(propertyName) && _errors[propertyName] != null && _errors[propertyName].Count > 0)
                {
                    return _errors[propertyName].ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return _errors.SelectMany(err => err.Value.ToList());
            }

        }

        #endregion

        #region //*********************** EVENTS ****************************

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        #endregion

    }
}
