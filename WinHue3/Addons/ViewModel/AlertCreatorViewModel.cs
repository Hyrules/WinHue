using HueLib2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Addons.Model;

namespace WinHue3.Addons.ViewModel
{
    public class AlertCreatorViewModel : ValidatableBindableBase
    {
        private AlertCreatorModel _alertCreatorModel;
        private ObservableCollection<Criteria> _criterias;
        private Body _action;
        private string _value;
        private string _rsselement;
        private string _operator;

        public AlertCreatorModel AlertCreatorModel
        {
            get
            {
                return _alertCreatorModel;
            }

            set
            {
                SetProperty(ref _alertCreatorModel,value);
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                SetProperty(ref _value,value);
            }
        }

        public string Rsselement
        {
            get { return _rsselement; }
            set { SetProperty(ref _rsselement,value); }
        }

        public string Operator
        {
            get { return _operator; }
            set { SetProperty(ref _operator,value); }
        }

        public AlertCreatorViewModel()
        {
            _alertCreatorModel = new AlertCreatorModel();
        }


    }
}
