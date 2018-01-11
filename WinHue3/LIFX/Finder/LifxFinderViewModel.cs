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


        public LifxFinderViewModel()
        {
            
        }

        public ICommand FindLifxDeviceCommand => new AsyncRelayCommand(param => FindLifxDevice(), (param) => CanFindLifxDevice());

        private bool CanFindLifxDevice()
        {
            
        }

        private async Task FindLifxDevice()
        {
            LifxCommMessage<Dictionary<IPAddress,StateService>> response = await Lifx.GetDevicesAsync();

            if (response.Error == false)
            {
                foreach (KeyValuePair<IPAddress, StateService> kvp in response.Data)
                {

                }
            }

        }
    }
}