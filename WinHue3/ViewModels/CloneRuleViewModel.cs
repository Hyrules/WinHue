using HueLib2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2.Objects.HueObject;

namespace WinHue3.ViewModels
{
    public class CloneRuleViewModel : ValidatableBindableBase
    {
        private ObservableCollection<IHueObject> _listRuleObject;
        private ObservableCollection<IHueObject> _listReplacementsObject;
        private IHueObject _selectedRuleObject;
        private IHueObject _selectedReplacementObject;
        public CloneRuleViewModel()
        {

        }

        public void Initialize(List<IHueObject> listRuleobject)
        {
            ListRuleObject = new ObservableCollection<IHueObject>(listRuleobject);
        }

        public ObservableCollection<IHueObject> ListReplacementsObject
        {
            get { return _listReplacementsObject; }
            set { SetProperty(ref _listReplacementsObject,value); }
        }

        public ObservableCollection<IHueObject> ListRuleObject
        {
            get { return _listRuleObject; }
            set { _listRuleObject = value; }
        }

        public IHueObject SelectedRuleObject
        {
            get { return _selectedRuleObject; }
            set { SetProperty(ref _selectedRuleObject, value); }
        }

        public IHueObject SelectedReplacementObject
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
