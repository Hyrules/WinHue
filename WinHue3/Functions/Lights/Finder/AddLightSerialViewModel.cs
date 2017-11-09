using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.ViewModels
{
    public class AddLightSerialViewModel : ValidatableBindableBase
    {
        private string _listSerials;

        public AddLightSerialViewModel()
        {
            _listSerials = string.Empty;
        }

        public string ListSerials { get => _listSerials; set => SetProperty(ref _listSerials,value); }
    }
}
