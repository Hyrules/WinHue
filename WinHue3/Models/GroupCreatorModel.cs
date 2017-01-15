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
        private ObservableCollection<HueObject> _listlights;
        private ObservableCollection<HueObject> _listAvailableLights;
        private string _type;
        private string _class;

        public GroupCreatorModel()
        {
            _listlights = new ObservableCollection<HueObject>();
            _listAvailableLights = new ObservableCollection<HueObject>();
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
        public ObservableCollection<HueObject> Listlights
        {
            get { return _listlights; }
            set { SetProperty(ref _listlights,value); }
        }

        public ObservableCollection<HueObject> ListAvailableLights
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
                OnPropertyChanged("CanClass");

            }
        }

        public string Class
        {
            get { return _class; }
            set { SetProperty(ref _class,value); }
        }
    }
}
