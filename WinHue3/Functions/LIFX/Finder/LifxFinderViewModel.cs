using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.LIFX.Framework;
using WinHue3.LIFX.Framework.Responses;
using WinHue3.LIFX.Framework.Responses.States.Device;
using WinHue3.Utils;

namespace WinHue3.LIFX.Finder
{
    public class LifxFinderViewModel : ValidatableBindableBase
    {
        private bool _isFinding;
        private ObservableCollection<LifxDevice> _devices;

        public LifxFinderViewModel()
        {
            _isFinding = false;
            _devices = new ObservableCollection<LifxDevice>();
        }

        public ICommand FindLifxDeviceCommand => new AsyncRelayCommand(param => FindLifxDevice(), (param) => CanFindLifxDevice());

        public ObservableCollection<LifxDevice> Devices
        {
            get { return _devices; }
            set { SetProperty(ref _devices,value); }
        }

        private bool CanFindLifxDevice()
        {
            return !_isFinding;
        }

        private async Task FindLifxDevice()
        {
            _isFinding = true;
            
            LifxCommMessage<Dictionary<IPAddress,StateService>> response = await Lifx.GetDevicesAsync();

            if (response.Error == false)
            {
                foreach (KeyValuePair<IPAddress, StateService> kvp in response.Data)
                {
                    Devices.Add(await LifxLight.CreateLight(kvp.Key,kvp.Value.Header.Target));
                }
            }
            _isFinding = false;
        }
    }
}