using HueLib2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.ViewModels
{
    public class SceneListLightsViewModel : ValidatableBindableBase
    {
        private ObservableCollection<HueObject> _listSceneLights;

        public SceneListLightsViewModel()
        {

        }

        public ObservableCollection<HueObject> ListSceneLights
        {
            get { return _listSceneLights; }
            set { SetProperty(ref _listSceneLights,value); }
        }
    }
}
