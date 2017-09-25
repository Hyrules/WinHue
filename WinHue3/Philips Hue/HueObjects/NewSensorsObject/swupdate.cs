using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WinHue3.ViewModels;

namespace WinHue3.Philips_Hue.HueObjects.NewSensorsObject
{
    [DataContract]
    public class SwUpdate : ValidatableBindableBase
    {
        private string _state;
        private string _lastinstall;

        [DataMember]
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        [DataMember]
        public string Lastinstall
        {
            get { return _lastinstall; }
            set { SetProperty(ref _lastinstall,value); }
        }
    }
}
