using System.Net;
using WinHue3.Validation;

namespace WinHue3.ViewModels
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
            get
            {
                return ip;
            }
            set
            {
                SetProperty(ref ip,value);
            }
        }


    }
}
