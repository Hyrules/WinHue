using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WinHue3.Utils;
using IHueObject = WinHue3.Philips_Hue.HueObjects.Common.IHueObject;

namespace WinHue3.Functions.Rules.Cloning
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
            get => _listReplacementsObject;
            set => SetProperty(ref _listReplacementsObject,value);
        }

        public ObservableCollection<IHueObject> ListRuleObject
        {
            get => _listRuleObject;
            set => _listRuleObject = value;
        }

        public IHueObject SelectedRuleObject
        {
            get => _selectedRuleObject;
            set => SetProperty(ref _selectedRuleObject, value);
        }

        public IHueObject SelectedReplacementObject
        {
            get => _selectedReplacementObject;
            set => SetProperty(ref _selectedReplacementObject, value);
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
