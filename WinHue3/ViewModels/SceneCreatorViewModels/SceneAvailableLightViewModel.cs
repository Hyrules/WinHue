using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using System.Collections.ObjectModel;

namespace WinHue3.ViewModels
{
    public class SceneAvailableLightViewModel : ValidatableBindableBase
    {
        private ObservableCollection<HueObject> _listAvailableLights;

        public SceneAvailableLightViewModel()
        {
            ListAvailableLights = new ObservableCollection<HueObject>();
        }

        public ObservableCollection<HueObject> ListAvailableLights
        {
            get { return _listAvailableLights; }
            set
            {
                SetProperty(ref _listAvailableLights,value);
                if(value != null)
                {
                    foreach(var i in value)
                    {
                        ((Light)i).state = new State();
                    }
                }
            }
        }
    }
}
