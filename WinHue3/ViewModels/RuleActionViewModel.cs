using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2.Objects.Rules;

namespace WinHue3.ViewModels
{
    public class RuleActionViewModel : ValidatableBindableBase
    {
        private string _method;
        private RuleAddress _address;
        private ObservableCollection<RuleActionPropertyViewModel> _listProperties;
        private string _objtype;
        private string _objid;


        public RuleActionViewModel()
        {
            _method = "PUT";
            _address = new RuleAddress();
            _listProperties = new ObservableCollection<RuleActionPropertyViewModel>();
        }


        public string Method
        {
            get { return _method; }
            set { SetProperty(ref _method,value); }
        }

        public ObservableCollection<RuleActionPropertyViewModel> ListProperties
        {
            get { return _listProperties; }
            set { _listProperties = value; }
        }

        public string Address => $"{ObjectType}/{ObjectId}";

        public string ObjectType
        {
            get { return _objtype; }
            set { _objtype = value; }
        }

        public string ObjectId
        {
            get { return _objid; }
            set { _objid = value; }
        }
    }
}
