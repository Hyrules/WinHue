using HueLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.ViewModels
{
    public class SceneCreatorViewModel : ValidatableBindableBase
    {
/*
        private Bridge _bridge;
        public SceneCreatorViewModel()
        {

        }

        public void Initialize(List<HueObject> listlights, Bridge bridge)
        {
            _bridge = bridge;
            _scene = new Scene();
            _scene.lights = new List<string>();
            _scene.recycle = false;
            _listAvailableLights = new ObservableCollection<HueObject>(listlights);
            _listSceneLights = new ObservableCollection<HueObject>();
            foreach (HueObject light in _listAvailableLights)
            {
                ((Light)light).state = new State();
            }
            _newstate = new State { @on = true };

        }

        public void Initialize(List<HueObject> listlights, string sceneid, Bridge bridge)
        {
            _bridge = bridge;

            CommandResult cr = _bridge.GetObject<Scene>(sceneid);
            if (cr.Success)
            {
                _scene = (Scene)cr.resultobject;
                _listAvailableLights = new ObservableCollection<HueObject>(listlights);
                _listSceneLights = new ObservableCollection<HueObject>();
                OnPropertyChanged("CanSaveSecene");
                OnPropertyChanged("CanPreviewScene");
                foreach (string s in _scene.lights)
                {

                    int index = _listAvailableLights.FindIndex(x => x.Id == s);
                    if (index == -1) continue;
                    if (!_scene.lightstates.ContainsKey(s)) continue;
                    ((Light)_listAvailableLights[index]).state = _scene.lightstates[s];
                    _listSceneLights.Add(_listAvailableLights[index]);
                    _listAvailableLights.RemoveAt(index);
                }

                foreach (HueObject light in _listAvailableLights)
                {
                    ((Light)light).state = new State();
                }
                _newstate = new State { @on = true };
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
        }*/
    }
}
