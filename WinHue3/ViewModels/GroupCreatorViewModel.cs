using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;
using WinHue3.Models;

namespace WinHue3.ViewModels
{
    public class GroupCreatorViewModel : ValidatableBindableBase
    {
        private GroupCreatorModel _groupCreator;

        private HueObject _selectedavailableLight;
        private HueObject _selectedgrouplight;

        public GroupCreatorViewModel()
        {
            _groupCreator = new GroupCreatorModel();

            HelperResult hr = HueObjectHelper.GetBridgeLights(BridgeStore.SelectedBridge);
            if (hr.Success)
            {
                GroupCreator.AvailableLightList = new BindingList<HueObject>((List<HueObject>)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
        }

        public Group Group
        {
            set { _groupCreator.Group = value; }
            get { return _groupCreator.Group; }
        } 

        public GroupCreatorModel GroupCreator
        {
            get { return _groupCreator; }
            set { SetProperty(ref _groupCreator, value); }
        }

        public HueObject SelectedAvailableLight
        {
            get { return _selectedavailableLight; }
            set
            {
                if(SetProperty(ref _selectedavailableLight,value))
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
                if(SetProperty(ref _selectedgrouplight,value))
                    OnPropertyChanged("CanRemoveLight");
            }
        }

        public bool CanAddLight => _selectedavailableLight != null;

        public bool CanRemoveLight => _selectedgrouplight != null;

        private void AddLightToGroup()
        {
            GroupCreator.GroupLightList.Add(_selectedavailableLight);
            GroupCreator.AvailableLightList.Remove(_selectedavailableLight);         
        }

        private void RemoveLightFromGroup()
        {
            GroupCreator.AvailableLightList.Add(_selectedgrouplight);
            GroupCreator.GroupLightList.Remove(_selectedgrouplight);
        }

        private void ClearFields()
        {
            _selectedavailableLight = null;
            _selectedgrouplight = null;

            foreach (HueObject obj in GroupCreator.GroupLightList)
            {
                GroupCreator.AvailableLightList.Add(obj);

            }
            GroupCreator.GroupLightList.Clear();
            GroupCreator.Name = string.Empty;
        }

        public ICommand AddLightToGroupCommand => new RelayCommand(param => AddLightToGroup());
        public ICommand RemoveLightFromGroupCommand => new RelayCommand(param => RemoveLightFromGroup());
        public ICommand ClearFieldsCommand => new RelayCommand(param => ClearFields());

    }
}
