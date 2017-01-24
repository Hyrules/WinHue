using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models
{
    public class ManageUsersModel : ValidatableBindableBase
    {
        private string _appname;
        private string _devtype;
        private string _lastused;
        private string _created;
        private string _key;


        public ManageUsersModel()
        {
            _appname = string.Empty;
            _devtype = string.Empty;
            _lastused = string.Empty;
            _created = string.Empty;
            _key = string.Empty;
        }

        public string ApplicationName
        {
            get { return _appname; }
            set { SetProperty(ref _appname,value); }
        }

        public string Devtype
        {
            get { return _devtype; }
            set { SetProperty(ref _devtype,value); }
        }

        public string Lastused
        {
            get { return _lastused; }
            set { SetProperty(ref _lastused,value); }
        }

        public string Created
        {
            get { return _created; }
            set { SetProperty(ref _created,value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key,value); }
        }
    }
}
