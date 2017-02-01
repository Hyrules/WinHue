using HueLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.ViewModels;
using WinHue3.Models;
using System.Collections.ObjectModel;

namespace WinHue3.ViewModels
{
    public class SceneCreatorViewModel : ValidatableBindableBase
    {

        private Bridge _bridge;
        private SceneCreatorModel _sceneCreatorModel;
        private SceneListLightsViewModel _sceneListLightsModel;
        private SceneSlidersViewModels _sceneSlidersViewModel;
        private SceneAvailableLightViewModel _sceneAvailableLightViewModel;

        public SceneCreatorViewModel()
        {
            SceneCreatorModel = new SceneCreatorModel();
            SceneListLightsViewModel = new SceneListLightsViewModel();
            SceneSlidersViewModel = new SceneSlidersViewModels();
            SceneAvailableLightViewModel = new SceneAvailableLightViewModel();
        }

        public SceneCreatorModel SceneCreatorModel
        {
            get { return _sceneCreatorModel; }
            set { SetProperty(ref _sceneCreatorModel,value); }
        }

        public SceneListLightsViewModel SceneListLightsViewModel
        {
            get { return _sceneListLightsModel; }
            set { SetProperty(ref _sceneListLightsModel,value); }
        }

        public SceneSlidersViewModels SceneSlidersViewModel
        {
            get { return _sceneSlidersViewModel; }
            set { _sceneSlidersViewModel = value; }
        }

        public SceneAvailableLightViewModel SceneAvailableLightViewModel
        {
            get { return _sceneAvailableLightViewModel; }
            set { _sceneAvailableLightViewModel = value; }
        }

        public void Initialize(List<HueObject> listlights, Bridge bridge)
        {
            _bridge = bridge;

            SceneAvailableLightViewModel.ListAvailableLights = new ObservableCollection<HueObject>(listlights);
            SceneListLightsViewModel.ListSceneLights = new ObservableCollection<HueObject>();


        }

        public void Initialize(List<HueObject> listlights, string sceneid, Bridge bridge)
        {
            _bridge = bridge;
            Scene scene;
            CommandResult cr = _bridge.GetObject<Scene>(sceneid);
            if (cr.Success)
            {
                scene = (Scene)cr.resultobject;
                SceneAvailableLightViewModel.ListAvailableLights = new ObservableCollection<HueObject>(listlights);
                SceneListLightsViewModel.ListSceneLights = new ObservableCollection<HueObject>();

                foreach (string s in scene.lights)
                {
                    int index = SceneListLightsViewModel.ListSceneLights.FindIndex(x => x.Id == s);
                    if (index == -1) continue;
                    if (!scene.lightstates.ContainsKey(s)) continue;
                    ((Light)SceneListLightsViewModel.ListSceneLights[index]).state = scene.lightstates[s];
                    SceneListLightsViewModel.ListSceneLights.Add(SceneListLightsViewModel.ListSceneLights[index]);
                    SceneListLightsViewModel.ListSceneLights.RemoveAt(index);
                }
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
        }
    }
}
