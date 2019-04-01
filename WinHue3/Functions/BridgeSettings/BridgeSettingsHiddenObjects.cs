using System.Collections.ObjectModel;
using System.Windows.Input;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Functions.BridgeSettings
{
    public class BridgeSettingsHiddenObjects : ValidatableBindableBase
    {
        private ObservableCollection<IHueObject> _hiddenObjects;
        private ObservableCollection<IHueObject> _listObjects;
        private IHueObject _selectedHiddenObject;
        private IHueObject _selectedObject;
       

        public BridgeSettingsHiddenObjects()
        {
            _hiddenObjects = new ObservableCollection<IHueObject>();
            _listObjects = new ObservableCollection<IHueObject>();
            _hiddenObjects.CollectionChanged += _listLights_CollectionChanged;
        }

        public ICommand ClearHiddenObjectsCommand => new RelayCommand(param=> ClearHiddenObjects());
        public ICommand AddObjectToHiddenCommand => new RelayCommand(param => AddObjectToHidden(), (param) => CanAddObject());
        public ICommand RemoveHiddenObjectCommand => new RelayCommand(param => RemoveHiddenObject(), (param) => CanRemoveObject());

        private bool CanAddObject()
        {
            return SelectedObject != null;
        }

        private void AddObjectToHidden()
        {
            HiddenObjects.Add(SelectedObject);
            ListObjects.Remove(SelectedObject);
            SelectedObject = null;
        }

       
        private bool CanRemoveObject()
        {
            return SelectedHiddenObject != null;
        }

        private void RemoveHiddenObject()
        {
            ListObjects.Add(SelectedHiddenObject);
            HiddenObjects.Remove(SelectedHiddenObject);
            SelectedHiddenObject = null;
        }

        private void ClearHiddenObjects()
        {
            HiddenObjects.Clear();
        }

        private void _listLights_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }

        public ObservableCollection<IHueObject> HiddenObjects
        {
            get => _hiddenObjects; 
            set => SetProperty(ref _hiddenObjects,value);
        }

        public ObservableCollection<IHueObject> ListObjects
        {
            get => _listObjects;
            set => SetProperty(ref _listObjects,value);
        }

        public IHueObject SelectedHiddenObject
        {
            get => _selectedHiddenObject;
            set => SetProperty(ref _selectedHiddenObject,value);
        }

        public IHueObject SelectedObject
        {
            get => _selectedObject;
            set => SetProperty(ref _selectedObject,value);
        }
    }
}
