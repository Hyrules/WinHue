using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using WinHue3.Validation;
using WinHue3.ViewModels;

namespace WinHue3.Models
{
    public class GroupCreatorModel : ValidatableBindableBase
    {

        
        private string _name;
        private ObservableCollection<Light> _listlights;
        private ObservableCollection<Light> _listAvailableLights;
        private string _type;
        private string _class;

        public GroupCreatorModel()
        {
            _listlights = new ObservableCollection<Light>();
            _listAvailableLights = new ObservableCollection<Light>();
            _type = "LightGroup";
            _class = "Other";
        }

        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);

            }
        }

        [MinimumCount(1, ErrorMessageResourceName = "Group_Select_One_Light", ErrorMessageResourceType = typeof(GlobalStrings))]
        public ObservableCollection<Light> Listlights
        {
            get { return _listlights; }
            set { SetProperty(ref _listlights,value); }
        }

        public ObservableCollection<Light> ListAvailableLights
        {
            get { return _listAvailableLights; }
            set { SetProperty(ref _listAvailableLights,value); }
        }

        public bool CanClass => Type == "Room";

        public string Type
        {
            get { return _type; }
            set
            {
                SetProperty(ref _type,value);
                if (value == "LightGroup")
                    Class = "Other";
                RaisePropertyChanged("CanClass");

            }
        }

        public string Class
        {
            get { return _class; }
            set { SetProperty(ref _class,value); }
        }
    }
}
