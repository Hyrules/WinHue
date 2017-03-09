using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Addons.Model
{
    public class AlertCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private string _description;
        private string _url;
        private bool _enable;

        public AlertCreatorModel()
        {
            Name = string.Empty;
            Description = string.Empty;
            Url = string.Empty;
            Enable = true;
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetProperty(ref _name,value);
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetProperty(ref _description,value);
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }

            set
            {
                SetProperty(ref _url,value);
            }
        }

        public bool Enable
        {
            get
            {
                return _enable;
            }

            set
            {
                SetProperty(ref _enable,value);
            }
        }
    }
}
