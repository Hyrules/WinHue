using WinHue3.Utils;
using WinHue3.Validations;

namespace WinHue3.Functions.BridgePairing
{
    public class AddManualIPViewModel : ValidatableBindableBase
    {
        string ip;


        public AddManualIPViewModel()
        {
            ip = string.Empty;
        }


        [IpValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName ="Invalid_IP")]
        public string BridgeIPAddress
        {
            get => ip;
            set => SetProperty(ref ip,value);
        }


    }
}
