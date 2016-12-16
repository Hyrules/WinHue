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

        private BindingList<HueObject> _availableLightList;
        private BindingList<HueObject> _groupLightList;
        private string _name;
        private Group _group;

        public GroupCreatorModel()
        {
            _availableLightList = new BindingList<HueObject> {RaiseListChangedEvents = true};
            _group = new Group();
            _groupLightList = new BindingList<HueObject> {RaiseListChangedEvents = true};
            _groupLightList.ListChanged += _groupLightList_ListChanged;
        }

        private void _groupLightList_ListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("GroupLightList");
        }

        public Group Group
        {
            get
            {

                _group.lights = _groupLightList.Select(o => o.Id).ToList<string>();
                _group.name = Name;             
                return _group;
            }
            set
            {
                _group = value;
                foreach (string s in _group.lights)
                {
                    HueObject obj = _availableLightList.First(x => x.Id == s);
                    if (obj == null) continue;
                    GroupLightList.Add(obj);
                    AvailableLightList.Remove(obj);
                }
                Name = _group.name;                
                OnPropertyChanged("GroupLightList");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);

            }
        }

        public BindingList<HueObject> AvailableLightList
        {
            get
            {
                return _availableLightList;
            }
            set
            {
                SetProperty(ref _availableLightList, value);
            }
        }


        [MinimumCount(1,ErrorMessageResourceName = "Group_Select_One_Light", ErrorMessageResourceType = typeof(GlobalStrings))]
        public BindingList<HueObject> GroupLightList
        {
            get
            {
                return _groupLightList;
            }
            set
            {
                SetProperty(ref _groupLightList, value); 
               
            }
        }



    }
}
