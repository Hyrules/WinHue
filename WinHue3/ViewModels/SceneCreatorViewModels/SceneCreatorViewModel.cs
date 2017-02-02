using HueLib2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.ViewModels;
using WinHue3.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using System.ComponentModel;

namespace WinHue3.ViewModels
{
    public class SceneCreatorViewModel : ValidatableBindableBase
    {
        private ObservableCollection<HueObject> _listAvailableLights;
        private HueObject _selectedLight;
        
        private Bridge _bridge;
        private SceneCreatorModel _sceneCreatorModel;
        private SceneListLightsViewModel _sceneListLightsModel;
        private SceneSlidersViewModels _sceneSlidersViewModel;
        private ObservableCollection<HueObject> _listSceneLights;
        private HueObject _selectedSceneLight;

        public SceneCreatorViewModel()
        {
            SceneCreatorModel = new SceneCreatorModel();
            SceneListLightsViewModel = new SceneListLightsViewModel();
            SceneSlidersViewModel = new SceneSlidersViewModels();
            ListAvailableLights = new ObservableCollection<HueObject>();
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

        public void Initialize(List<HueObject> listlights, Bridge bridge)
        {
            _bridge = bridge;
            ListAvailableLights = new ObservableCollection<HueObject>(listlights);
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
                ListAvailableLights = new ObservableCollection<HueObject>(listlights);
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

        public ObservableCollection<HueObject> ListSceneLights
        {
            get { return _listSceneLights; }
            set { SetProperty(ref _listSceneLights, value); }
        }

        public ObservableCollection<HueObject> ListAvailableLights
        {
            get { return _listAvailableLights; }
            set
            {
                SetProperty(ref _listAvailableLights, value);
                if (value != null)
                {
                    foreach (var i in value)
                    {
                        ((Light)i).state = new State();
                    }
                }
            }
        }

        public HueObject SelectedAvailableLight
        {
            get { return _selectedLight; }
            set { SetProperty(ref _selectedLight, value); }
        }

        public void GetColorFromImage()
        {
            Form_SelectColorFromImage fsci = new Form_SelectColorFromImage() { Owner = Application.Current.MainWindow };
            if (fsci.ShowDialog() != true) return;
            Color c = fsci.GetSelectedColor();
            CGPoint color = HueColorConverter.CalculateXY(c, ((Light)SelectedAvailableLight).modelid);
            SceneCreatorModel.Sat = 255;
            SceneCreatorModel.X = Convert.ToDecimal(color.x);
            SceneCreatorModel.Y = Convert.ToDecimal(color.y);
        }

        public void SetRandomColor()
        {
            Random rdm = new Random();
            SceneCreatorModel.Hue = (ushort)rdm.Next(65535);
        }

        public HueObject SelectedSceneLight
        {
            get { return _selectedSceneLight; }
            set { SetProperty(ref _selectedSceneLight, value); }
        }

        private void RemoveSelectedSceneLight()
        {
            ((Light)SelectedSceneLight).state = new State();
            //_scene.lights.Remove(SelectedSceneLight.Id);
            ListAvailableLights.Add(SelectedSceneLight);
           // _scene.lightstates.Remove(SelectedSceneLight.Id);
            ListSceneLights.Remove(SelectedSceneLight);

            SelectedSceneLight = null;

            
        }

        public void AddLightsToScene(List<HueObject> lightlist)
        {

            foreach (HueObject obj in lightlist)
            {
                _listAvailableLights.Remove(obj);
              //  ((Light)obj).state = _newstate;
                ListSceneLights.Add(obj);
             //   if (_scene.lightstates == null) _scene.lightstates = new Dictionary<string, State>();
             //   _scene.lightstates.Add(obj.Id, ((Light)obj).state);
            }

          //  _scene.lights.AddRange(lightlist.Select(x => x.Id).ToList());

            //_newstate = new State() { on = true};
            OnPropertyChanged("LightSceneLights");

        }

        private void DoPreviewScene()
        {

            BackgroundWorker bgWorker = new BackgroundWorker() { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerAsync(ListSceneLights);
        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            ObservableCollection<HueObject> li = (ObservableCollection<HueObject>)e.Argument;
            ObservableCollection<HueObject> liOriginalState = new ObservableCollection<HueObject>();
            foreach (HueObject obj in li)
            {
                HelperResult hr = HueObjectHelper.GetObject<Light>(_bridge, obj.Id);
                if (hr.Success)
                {
                    HueObject newlight = (HueObject)hr.Hrobject;
                    ((Light)newlight).state.alert = null;
                    liOriginalState.Add(newlight);
                }
            }

            foreach (HueObject obj in li)
            {
                _bridge.SetState<Light>(((Light)obj).state, obj.Id);
            }

            Thread.Sleep(5000);

            foreach (HueObject obj in liOriginalState)
            {
                _bridge.SetState<Light>(((Light)obj).state, obj.Id);
            }

            Thread.Sleep(2000);


        }

        public void ClearSelectionSceneLight()
        {
            _selectedSceneLight = null;
            SceneCreatorModel.On = true;
            OnPropertyChanged("SelectedSceneLight");
            OnPropertyChanged("Hue");
            OnPropertyChanged("Sat");
            OnPropertyChanged("Bri");
            OnPropertyChanged("CT");
            OnPropertyChanged("X");
            OnPropertyChanged("Y");
            OnPropertyChanged("TT");
        }

        public ICommand RemoveSelectedSceneLightCommand => new RelayCommand(param => RemoveSelectedSceneLight());
        public ICommand SetRandomColorCommand => new RelayCommand(param => SetRandomColor());
        public ICommand GetColorFromImageCommand => new RelayCommand(param => GetColorFromImage());
        public ICommand ClearSelectionSceneLightCommand => new RelayCommand(param => ClearSelectionSceneLight());
        public ICommand DoPreviewSceneCommand => new RelayCommand(param => DoPreviewScene());


    }
}
