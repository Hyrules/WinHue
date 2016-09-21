using System.Net;

namespace WinHue3
{
    public class AddManualIPView : View
    {
        string ip;

        #region CTOR

        public AddManualIPView()
        {
            ip = string.Empty;
        }

        #endregion

        #region PROPERTIES

        public string BridgeIPAddress
        {
            get
            {
                return ip;
            }
            set
            {
                ip = value;
                OnPropertyChanged();
                IPAddress ipadr;
                if(IPAddress.TryParse(ip, out ipadr))
                {
                    RemoveError(GlobalStrings.Invalid_IP);
                }
                else
                {
                    SetError(GlobalStrings.Invalid_IP);
                }
            }
        }

        #endregion

        #region METHODS

        #endregion

        #region COMMANDS

        #endregion

    }
}
