using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;
using WinHue3.Functions.BridgeManager;
using WinHue3.Philips_Hue.HueObjects.GroupObject;

namespace WinHue3.Functions.Entertainment
{
    public class EntertainmentViewModel : ValidatableBindableBase
    {
        private ObservableCollection<Light> _listAvailableLights;
        private ObservableCollection<Light> _listLights;
        private EntertrainmentModel _entertainmentModel;

        public EntertainmentViewModel()
        {
            ListLights = new ObservableCollection<Light>();
            _entertainmentModel = new EntertrainmentModel();
        }

        public async Task Initialize()
        {
            ListAvailableLights = (await BridgesManager.Instance.SelectedBridge.GetListObjectsAsync<Light>()).ToObservableCollection();

        }

        public bool SaveEntertainment()
        {
            Group entertainment = new Group()
            {
                name = _entertainmentModel.Name,
                @class = "TV",
                lights = ListLights.Select(x => x.Id).ToList(),
                type = "Entertainment"
            };

            return BridgesManager.Instance.SelectedBridge.CreateObject(entertainment);

        }

        public ObservableCollection<Light> ListAvailableLights { get => _listAvailableLights; private set => SetProperty(ref _listAvailableLights,value); }
        public ObservableCollection<Light> ListLights { get => _listLights; set => SetProperty(ref _listLights,value); }
        public EntertrainmentModel EntertainmentModel { get => _entertainmentModel; set => SetProperty(ref _entertainmentModel,value); }
    }
}
