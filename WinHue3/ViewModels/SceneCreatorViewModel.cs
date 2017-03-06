using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HueLib2;
using WinHue3.Models;

namespace WinHue3.ViewModels
{
    public class SceneCreatorViewModel : ValidatableBindableBase
    {
        private ObservableCollection<Light> _listAvailableLights;
        private ObservableCollection<Light> _selectedLight;
        private BackgroundWorker _bgWorker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
        private Bridge _bridge;
        private SceneCreatorModel _sceneCreatorModel;
        private Light _selectedSceneLight;
        private ObservableCollection<Light> _listSceneLights;

        public SceneCreatorViewModel()
        {
            SceneCreatorModel = new SceneCreatorModel();

            ListAvailableLights = new ObservableCollection<Light>();
            SelectedAvailableLights = new ObservableCollection<Light>();
            ListSceneLights = new ObservableCollection<Light>();
        }

        public SceneCreatorModel SceneCreatorModel
        {
            get { return _sceneCreatorModel; }
            set { SetProperty(ref _sceneCreatorModel,value); }
        }

        public void Initialize(List<Light> listlights, Bridge bridge)
        {
            _bridge = bridge;
            ListAvailableLights = new ObservableCollection<Light>(listlights);
            ListSceneLights = new ObservableCollection<Light>();
        }

        public void Initialize(Bridge bridge)
        {
            _bridge = bridge;
            ListSceneLights = new ObservableCollection<Light>();
        }

        public ObservableCollection<Light> ListSceneLights
        {
            get { return _listSceneLights; }
            set { SetProperty(ref _listSceneLights, value); }
        }

        public ObservableCollection<Light> ListAvailableLights
        {
            get { return _listAvailableLights; }
            set
            {
                SetProperty(ref _listAvailableLights, value);
                if (value == null) return;
                foreach (Light i in value)
                {
                    i.state = new State();
                }
            }
        }

        public ObservableCollection<Light> SelectedAvailableLights
        {
            get { return _selectedLight; }
            set { SetProperty(ref _selectedLight, value); }
        }

        public State ObjectState
        {
            get
            {
                return SelectedSceneLight != null ? SelectedSceneLight.state : SceneCreatorModel.State;
            }
            set
            {

                if (SelectedSceneLight != null)
                {
                    SelectedSceneLight.state = value;             
                }
                else
                {
                    SceneCreatorModel.State = value;
                }

            }
        
        }

        public Scene Scene
        {
            get
            {
                Scene scene = new Scene
                {
                    name = SceneCreatorModel.Name,
                    lights = ListSceneLights.Select(x => x.Id).ToList(),
                    lightstates = new Dictionary<string, State>(),
                    recycle = SceneCreatorModel.Recycle
                };
                
                foreach (Light l in ListSceneLights)
                {
                    scene.lightstates.Add(l.Id,l.state);
                }

                return scene;
            }
            set
            {
                SceneCreatorModel.Name = value.name;
                ListSceneLights = new ObservableCollection<Light>(ListAvailableLights.Where(x => value.lights.Contains(x.Id)));
                foreach (Light h in ListSceneLights)
                {
                    h.state = value.lightstates[h.Id];
                }
            }
        }

        public void GetColorFromImage()
        {
            Form_SelectColorFromImage fsci = new Form_SelectColorFromImage { Owner = Application.Current.MainWindow };
            if (fsci.ShowDialog() != true) return;
            Color c = fsci.GetSelectedColor();
            CGPoint color = HueColorConverter.CalculateXY(c, "");
            SceneCreatorModel.Sat = 255;
            SceneCreatorModel.X = Convert.ToDecimal(color.x);
            SceneCreatorModel.Y = Convert.ToDecimal(color.y);
        }

        public void SetRandomColor()
        {
            Random rdm = new Random();
            SceneCreatorModel.Hue = (ushort)rdm.Next(65535);
        }

        public Light SelectedSceneLight
        {
            get { return _selectedSceneLight; }
            set
            {
                SetProperty(ref _selectedSceneLight, value);
                if (value == null) return;
                SceneCreatorModel.Hue = value.state.hue;
                SceneCreatorModel.Bri = value.state.bri;
                SceneCreatorModel.Sat = value.state.sat;
                SceneCreatorModel.Ct = value.state.ct;
                if (value.state.xy != null)
                {
                    SceneCreatorModel.X = value.state.xy.x;
                    SceneCreatorModel.Y = value.state.xy.y;
                }
                SceneCreatorModel.TT = value.state.transitiontime;
            }
        }

        private void RemoveSelectedSceneLight()
        {
            SelectedSceneLight.state = null;
            ListAvailableLights.Add(SelectedSceneLight);
            ListSceneLights.Remove(SelectedSceneLight);
            SelectedSceneLight = null;
        }

        private void DoPreviewScene()
        {
            _bgWorker.DoWork += bgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
            _bgWorker.RunWorkerAsync(new object[] { ListSceneLights,_bridge});
        }

        private void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] objarr = (object[]) e.Argument;
            Bridge br = (Bridge)objarr[1];
            ObservableCollection<Light> li = (ObservableCollection<Light>)objarr[0];
            ObservableCollection<Light> liOriginalState = new ObservableCollection<Light>();
            foreach (Light obj in li)
            {
                HelperResult hr = HueObjectHelper.GetObject<Light>(br, obj.Id);
                if (!hr.Success) continue;
                Light newlight = (Light)hr.Hrobject;
                newlight.state.alert = null;
                liOriginalState.Add(newlight);
            }

            foreach (Light obj in li)
            {
                _bridge.SetState<Light>(obj.state, obj.Id);
            }

            Thread.Sleep(5000);

            foreach (Light obj in liOriginalState)
            {
                _bridge.SetState<Light>(obj.state, obj.Id);
            }

            Thread.Sleep(2000);


        }

        public void ClearSelectionSceneLight()
        {
            SelectedSceneLight = null;
           
        }

        private void AddSelectedLightsToScene()
        {
            foreach (Light obj in SelectedAvailableLights)
            {
                ListAvailableLights.Remove(obj);
                obj.state = new State { hue = SceneCreatorModel.Hue, bri = SceneCreatorModel.Bri, sat = SceneCreatorModel.Sat, ct = SceneCreatorModel.Ct };
                if (SceneCreatorModel.X != null && SceneCreatorModel.Y != null)
                {
                    obj.state.xy = new XY(Convert.ToDecimal(SceneCreatorModel.X), Convert.ToDecimal(SceneCreatorModel.Y));
                }

                if (SceneCreatorModel.TT != null)
                {
                    obj.state.transitiontime = Convert.ToUInt32(SceneCreatorModel.TT);
                }
                ListSceneLights.Add(obj);
            }

            ClearSliders();

        }

        private void ModifyState()
        {
            SelectedSceneLight.state = new State()
            {
                hue = SceneCreatorModel.Hue,
                bri = SceneCreatorModel.Bri,
                on = SceneCreatorModel.On,
                ct = SceneCreatorModel.Ct,
                sat = SceneCreatorModel.Sat
            };

            if (SceneCreatorModel.X != null && SceneCreatorModel.Y != null)
            {
                SelectedSceneLight.state.xy = new XY(Convert.ToDecimal(SceneCreatorModel.X), Convert.ToDecimal(SceneCreatorModel.Y));
            }

            SelectedSceneLight = null;
            ClearSliders();
        }

        private void ClearSliders()
        {
            SceneCreatorModel.X = null;
            SceneCreatorModel.Y = null;
            SceneCreatorModel.Hue = null;
            SceneCreatorModel.Bri = null;
            SceneCreatorModel.Ct = null;
            SceneCreatorModel.TT = null;
            SceneCreatorModel.Sat = null;
            SceneCreatorModel.On = true;
        }

        private bool CanRemoveSelectedSceneLight()
        {
            return SelectedSceneLight != null;
        }

        private bool CanAddLightsToScene()
        {
            return SelectedAvailableLights.Count > 0;
        }

        private bool CanModify()
        {
            return SelectedSceneLight != null;
        }

        private bool CanPreview()
        {
            return ListSceneLights.Count > 0 && !_bgWorker.IsBusy;
        }

        public ICommand RemoveSelectedSceneLightCommand => new RelayCommand(param => RemoveSelectedSceneLight(), param=> CanRemoveSelectedSceneLight());
        public ICommand SetRandomColorCommand => new RelayCommand(param => SetRandomColor());
        public ICommand GetColorFromImageCommand => new RelayCommand(param => GetColorFromImage());
        public ICommand ClearSelectionSceneLightCommand => new RelayCommand(param => ClearSelectionSceneLight());
        public ICommand DoPreviewSceneCommand => new RelayCommand(param => DoPreviewScene(), (param) => CanPreview());
        public ICommand AddSelectedLightsToSceneCommand => new RelayCommand(param => AddSelectedLightsToScene(), param => CanAddLightsToScene());
        public ICommand ModifyStateCommand => new RelayCommand(param => ModifyState(), (param) => CanModify());




    }
}
