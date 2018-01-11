using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.LIFX.Responses;
using WinHue3.LIFX.Responses.States.Device;
using WinHue3.Utils;

namespace WinHue3.LIFX.Finder
{
    public class LifxFinderViewModel : ValidatableBindableBase
    {
        private bool _isFinding;
        private List<LifxDevice> _devices;

        public LifxFinderViewModel()
        {
            _isFinding = false;
            _devices = new List<LifxDevice>();
        }

        public ICommand FindLifxDeviceCommand => new AsyncRelayCommand(param => FindLifxDevice(), (param) => CanFindLifxDevice());

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
                    _devices.Add(new LifxLight(kvp.Key,kvp.Value.Header.Target));
                }
            }
            _isFinding = false;
        }
    }
}