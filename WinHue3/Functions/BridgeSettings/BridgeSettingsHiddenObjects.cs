using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.BridgeSettings
{
    public class BridgeSettingsHiddenObjects : ValidatableBindableBase
    {
        private ObservableCollection<Light> _selectedHiddenLights;
        private ObservableCollection<Light> _listLights;
       

        public BridgeSettingsHiddenObjects()
        {
            _selectedHiddenLights = new ObservableCollection<Light>();
            _listLights = new ObservableCollection<Light>();
            _selectedHiddenLights.CollectionChanged += _listLights_CollectionChanged;
        }

        private void _listLights_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }

        public ObservableCollection<Light> SelectedHiddenLights
        {
            get => _selectedHiddenLights; 
            set => SetProperty(ref _selectedHiddenLights,value);
        }

        public ObservableCollection<Light> ListLights
        {
            get => _listLights;
            set => SetProperty(ref _listLights,value);
        }
    }
}
