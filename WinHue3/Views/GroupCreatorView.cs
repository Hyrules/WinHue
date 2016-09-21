using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HueLib2;
using System.Windows.Input;

namespace WinHue3
{
    public class GroupCreatorView : View
    {
        private ObservableCollection<HueObject> _lightlist;
        private ObservableCollection<HueObject> _grouplights;

        private Group _group;
        private HueObject _selectedavailableLight;
        private HueObject _selectedgrouplight;

        #region //************************** CTOR ******************************************

        public GroupCreatorView(List<HueObject> lightlist )
        {
            _lightlist = new ObservableCollection<HueObject>(lightlist);
            _grouplights = new ObservableCollection<HueObject>();
            _group = new Group {lights = new List<string>()};
            SetError(GlobalStrings.Group_Select_One_Light, "GroupLightList");
        }

        public GroupCreatorView(List<HueObject> lightlist, Group group)
        {
            _lightlist = new ObservableCollection<HueObject>(lightlist);
            _grouplights = new ObservableCollection<HueObject>();
            foreach(string s in group.lights)
            {
                if(_lightlist.Any(x => x.Id == s))
                {
                    _grouplights.Add(_lightlist.First(x => x.Id == s));
                    _lightlist.Remove(_lightlist.First(x => x.Id == s));
                }
            }
            _group = group;

        }

        #endregion

        #region //************************** PROPERTIES *************************************

        public ObservableCollection<HueObject> AvailableLightList => _lightlist;
        public ObservableCollection<HueObject> GroupLightList => _grouplights;

        public bool CanRemoveLight => _selectedgrouplight != null;

        public bool CanAddLight => _selectedavailableLight != null;

        public HueObject SelectedAvailableLight
        {
            get { return _selectedavailableLight; }
            set
            {
                _selectedavailableLight = value;
                OnPropertyChanged();
                OnPropertyChanged("CanAddLight");
            }

        }

        public HueObject SelectedGroupLight
        {
            get
            {
                return _selectedgrouplight;
            }
            set
            {
                _selectedgrouplight = value;
                OnPropertyChanged();
                OnPropertyChanged("CanRemoveLight");
            }
        }

        public string GroupName
        {
            get { return _group.name; }
            set { _group.name = value; }
        }

        #endregion

        #region //*************************** METHODS ****************************************

        public Group GetGroup()
        {
            return _group;
        }

        private void AddLightToGroup()
        {
            _group.lights.Add(_selectedavailableLight.Id);
            _grouplights.Add(_selectedavailableLight);
            _lightlist.Remove(_selectedavailableLight);
            RemoveError(GlobalStrings.Group_Select_One_Light, "GroupLightList");
        }

        private void RemoveLightFromGroup()
        {
            _group.lights.Remove(_selectedgrouplight.Id);
            _lightlist.Add(_selectedgrouplight);
            _grouplights.Remove(_selectedgrouplight);

            if (_grouplights.Count == 0)
                SetError(GlobalStrings.Group_Select_One_Light, "GroupLightList");
        }

        private void ClearFields()
        {
            _selectedavailableLight = null;
            _selectedgrouplight = null;

            foreach (HueObject obj in _grouplights)
            {
                _lightlist.Add(obj);
                
            }
            _grouplights.Clear();
            GroupName = string.Empty;
            SetError(GlobalStrings.Group_Select_One_Light, "GroupLightList");
        }

        #endregion

        #region //**************************** COMMANDS *************************************

        public ICommand AddLightToGroupCommand => new RelayCommand(param => AddLightToGroup());
        public ICommand RemoveLightFromGroupCommand => new RelayCommand(param => RemoveLightFromGroup());
        public ICommand ClearFieldsCommand => new RelayCommand(param => ClearFields());

        #endregion

        #region //**************************** EVENTS *****************************************

        #endregion
    }
}
