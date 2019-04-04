using System.Collections.ObjectModel;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;
using WinHue3.Validations;

namespace WinHue3.Functions.Groups.Creator
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
            get => _name;
            set => SetProperty(ref _name, value);
        }

        [MinimumCount(1, ErrorMessageResourceName = "Group_Select_One_Light", ErrorMessageResourceType = typeof(GlobalStrings))]
        public ObservableCollection<Light> Listlights
        {
            get => _listlights;
            set => SetProperty(ref _listlights,value);
        }

        public ObservableCollection<Light> ListAvailableLights
        {
            get => _listAvailableLights;
            set => SetProperty(ref _listAvailableLights,value);
        }

        public bool CanClass => Type == "Room";

        public string Type
        {
            get => _type;
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
            get => _class;
            set => SetProperty(ref _class,value);
        }
    }
}
