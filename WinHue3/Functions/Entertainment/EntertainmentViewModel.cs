using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;
using WinHue3.Functions.BridgeManager;
using WinHue3.Philips_Hue.BridgeObject.Entertainment_API;
using WinHue3.Philips_Hue.HueObjects.GroupObject;

namespace WinHue3.Functions.Entertainment
{
    public class EntertainmentViewModel : ValidatableBindableBase
    {
        private ObservableCollection<Light> _listAvailableLights;
        private ObservableCollection<EntertainmentLight> _listLights;
        private EntertrainmentModel _entertainmentModel;
        private Light _selectedLight;
        private EntertainmentLight _selectedEntertainmentLight;

        private bool CanAddLight() => SelectedLight != null;

        public EntertainmentViewModel()
        {
            ListLights = new ObservableCollection<EntertainmentLight>();
            _entertainmentModel = new EntertrainmentModel();
        }

        public async Task Initialize()
        {
            ListAvailableLights = (await BridgesManager.Instance.SelectedBridge.GetListObjectsAsync<Light>()).ToObservableCollection();

        }

        public bool SaveEntertainment()
        {
            string newid = string.Empty;
            bool success = false;
            Group entertainment = new Group()
            {
                name = _entertainmentModel.Name,
                @class = "TV",
                lights = ListLights.Select(x => x.Light.Id).ToList(),
                type = "Entertainment"
            };

            success = BridgesManager.Instance.SelectedBridge.CreateObject(entertainment);

            if (success)
            {
                Location loc = new Location();
                newid = BridgesManager.Instance.SelectedBridge.LastCommandMessages.LastSuccess.value;
                foreach (EntertainmentLight el in ListLights)
                {
                   loc.Add(el.Light.Id, el.Location);
                }

                success = success && BridgesManager.Instance.SelectedBridge.SetEntertrainementLightLocation(newid, loc);
            }


            return success;

        }

        public ObservableCollection<Light> ListAvailableLights { get => _listAvailableLights; private set => SetProperty(ref _listAvailableLights,value); }
        public ObservableCollection<EntertainmentLight> ListLights { get => _listLights; set => SetProperty(ref _listLights,value); }
        public EntertrainmentModel EntertainmentModel { get => _entertainmentModel; set => SetProperty(ref _entertainmentModel,value); }

        public Light SelectedLight
        {
            get => _selectedLight;
            set => SetProperty(ref _selectedLight,value);
        }

        public ICommand AddEntertainmentLightCommand => new RelayCommand(param => AddEntertainmentLight(), (param) => CanAddLight());
        public ICommand RemoveEntertainmentLightCommand => new RelayCommand(param => RemoveEntertainmentLight(), (param) => CanRemoveLight());

        public EntertainmentLight SelectedEntertainmentLight
        {
            get => _selectedEntertainmentLight;
            set => SetProperty(ref _selectedEntertainmentLight,value);
        }

        private bool CanRemoveLight() => SelectedEntertainmentLight != null;

        private void RemoveEntertainmentLight()
        {
            ListAvailableLights.Add(SelectedEntertainmentLight.Light);
            ListLights.Remove(SelectedEntertainmentLight);
        }


        private void AddEntertainmentLight()
        {
            ListLights.Add(new EntertainmentLight(SelectedLight, new decimal[]{EntertainmentModel.X,EntertainmentModel.Y, EntertainmentModel.Z}));
            ListAvailableLights.Remove(SelectedLight);
        }

    }
}
