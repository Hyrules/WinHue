using HueLib2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinHue3.ViewModels
{
    public class CloneRuleViewModel : ValidatableBindableBase
    {
        private ObservableCollection<HueObject> _listRuleObject;
        private ObservableCollection<HueObject> _listReplacementsObject;
        private HueObject _selectedRuleObject;
        private HueObject _selectedReplacementObject;
        public CloneRuleViewModel()
        {

        }

        public void Initialize(List<HueObject> listRuleobject)
        {
            ListRuleObject = new ObservableCollection<HueObject>(listRuleobject);
        }

        public ObservableCollection<HueObject> ListReplacementsObject
        {
            get { return _listReplacementsObject; }
            set { SetProperty(ref _listReplacementsObject,value); }
        }

        public ObservableCollection<HueObject> ListRuleObject
        {
            get { return _listRuleObject; }
            set { _listRuleObject = value; }
        }

        public HueObject SelectedRuleObject
        {
            get { return _selectedRuleObject; }
            set { SetProperty(ref _selectedRuleObject, value); }
        }

        public HueObject SelectedReplacementObject
        {
            get { return _selectedReplacementObject; }
            set { SetProperty(ref _selectedReplacementObject, value); }
        }

        private void AddToList()
        {
            ListReplacementsObject.Remove(SelectedRuleObject);
            SelectedRuleObject = null;
        }

        private void RemoveFromList()
        {
            
        }

        public ICommand AddToListCommand => new RelayCommand(param => AddToList());

        public ICommand RemoveFromListCommand => new RelayCommand(param => RemoveFromList());

    }
}
