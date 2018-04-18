using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Entertainment
{
    public class EntertainmentViewModel : ValidatableBindableBase
    {
        private ObservableCollection<Light> _listAvailableLights;
        private ObservableCollection<Light> _listEtLights;

        public EntertainmentViewModel()
        {
            ListEtLights = new ObservableCollection<Light>();
        }

        public ICommand AddLightCommand => new RelayCommand(param => AddLight());

        private void AddLight()
        {
            
        }

        public ObservableCollection<Light> ListAvailableLights { get => _listAvailableLights; set => SetProperty(ref _listAvailableLights,value); }
        public ObservableCollection<Light> ListEtLights { get => _listEtLights; set => SetProperty(ref _listEtLights,value); }


    }
}
