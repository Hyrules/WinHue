using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;

namespace WinHue3.Models
{
    public class ResourceLinkCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private ObservableCollection<HueObject> _listlinkObject;
        private string _description;

        public ResourceLinkCreatorModel()
        {
            _name = string.Empty;
            _listlinkObject = new ObservableCollection<HueObject>();
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name,value); }
        }

        public ObservableCollection<HueObject> ListlinkObject
        {
            get { return _listlinkObject; }
            set { SetProperty(ref _listlinkObject,value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description,value); }
        }
    }
}
