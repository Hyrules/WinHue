using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using WinHue3.Colors;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Scenes.Creator.ColorPicker;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Scenes.Creator
{
    public class SceneCreatorViewModel : ValidatableBindableBase
    {
        private ObservableCollection<Light> _listAvailableLights;
        private ObservableCollection<Light> _selectedLight;
        private readonly BackgroundWorker _bgWorker;
        private SceneCreatorModel _sceneCreatorModel;
        private Light _selectedSceneLight;
        private ObservableCollection<Light> _listSceneLights;
        private Bridge _bridge;

        private bool _hueChecked;
        private bool _briChecked;
        private bool _satChecked;
        private bool _ctChecked;
        private bool _xyChecked;
        private bool _ttChecked;

        public SceneCreatorViewModel()
        {
            _bgWorker = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
            _sceneCreatorModel = new SceneCreatorModel();

            _listAvailableLights = new ObservableCollection<Light>();
            _selectedLight = new ObservableCollection<Light>();
            _listSceneLights = new ObservableCollection<Light>();
        }

        public SceneCreatorModel SceneCreatorModel
        {
            get => _sceneCreatorModel;
            set => SetProperty(ref _sceneCreatorModel,value);
        }

        public void Initialize(Bridge bridge,List<Light> listlights, Scene edited = null)
        {
            _bridge = bridge;
            ListAvailableLights = new ObservableCollection<Light>(listlights);
            ListSceneLights = new ObservableCollection<Light>();
            EditScene(edited);
        }
        private void EditScene(Scene edited)
        {

            if (edited != null)
            {
                SceneCreatorModel.Name = edited.name;
                ListSceneLights = new ObservableCollection<Light>(ListAvailableLights.Where(x => edited.lights.Contains(x.Id)));
                foreach (Light h in ListSceneLights)
                {
                    h.state = edited.version == 1 ? ListAvailableLights.FirstOrDefault(x => x.Id == h.Id)?.state : edited.lightstates[h.Id];
                    ListAvailableLights.Remove(x => x.Id == h.Id);
                }

            }
        }

        public ObservableCollection<Light> ListSceneLights
        {
            get => _listSceneLights;
            set => SetProperty(ref _listSceneLights, value);
        }

        public ObservableCollection<Light> ListAvailableLights
        {
            get => _listAvailableLights;
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
            get => _selectedLight;
            set => SetProperty(ref _selectedLight, value);
        }

        public State ObjectState
        {
            get => SelectedSceneLight != null ? SelectedSceneLight.state : SceneCreatorModel.State;
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

        }

        public void GetColorFromImage()
        {
            Form_SelectColorFromImage fsci = new Form_SelectColorFromImage { Owner = Application.Current.MainWindow };
            if (fsci.ShowDialog() != true) return;
            System.Windows.Media.Color c = fsci.GetSelectedColor();
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
            get => _selectedSceneLight;
            set
            {
                SetProperty(ref _selectedSceneLight, value);

                HueChecked = value?.state.hue != null;
                SceneCreatorModel.Hue = value?.state.hue;

                BriChecked = value?.state.bri != null;
                SceneCreatorModel.Bri = value?.state.bri;

                SatChecked = value?.state.sat != null;
                SceneCreatorModel.Sat = value?.state.sat;

                CTChecked = value?.state.ct != null;
                SceneCreatorModel.Ct = value?.state.ct;
                if(value?.state.on != null)
                    SceneCreatorModel.On = (bool)value.state.on;

                XYChecked = value?.state.xy != null;
                if (value?.state.xy != null)
                {
                    SceneCreatorModel.X = value.state.xy[0];
                    SceneCreatorModel.Y = value.state.xy[1];
                }

                TtChecked = value?.state.transitiontime != null;
                SceneCreatorModel.TT = value?.state.transitiontime;
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
            _bgWorker.DoWork += BgWorker_DoWork;
            _bgWorker.RunWorkerCompleted += _bgWorker_RunWorkerCompleted;
            _bgWorker.RunWorkerAsync(new object[] { ListSceneLights, _bridge });
        }

        private void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] objarr = (object[]) e.Argument;
            ObservableCollection<Light> li = (ObservableCollection<Light>)objarr[0];
            Bridge bgwBridge = (Bridge)objarr[1];
            ObservableCollection<Light> liOriginalState = new ObservableCollection<Light>();
            foreach (Light obj in li)
            {
                Light hr = bgwBridge.GetObject<Light>(obj.Id);
                if (hr == null) continue;
                Light newlight = hr;
                newlight.state.alert = null;
                liOriginalState.Add(newlight);
            }

            foreach (Light obj in li)
            {
                State state = obj.state;
                bgwBridge.SetState(state, obj.Id);
            }

            Thread.Sleep(5000);

            foreach (Light obj in liOriginalState)
            {
                bgwBridge.SetState(obj.state, obj.Id);
            }

            Thread.Sleep(2000);


        }

        public void ClearSelectionSceneLight()
        {
            SelectedSceneLight = null;
           
        }

        public TimeSpan? TT
        {
            get
            {
                if (SceneCreatorModel.TT == null) return null;
                TimeSpan ts = TimeSpan.FromMilliseconds((double)SceneCreatorModel.TT * 100);
                return ts;
            }
            set
            {
                TimeSpan t = value.GetValueOrDefault();
                ushort tt = Convert.ToUInt16(t.TotalMilliseconds / 100);
                SceneCreatorModel.TT = tt;
                RaisePropertyChanged();
                
            }
        }

        private void AddSelectedLightsToScene()
        {
            foreach (Light obj in SelectedAvailableLights)
            {
                ListAvailableLights.Remove(obj);
                obj.state = new State { hue = SceneCreatorModel.Hue, bri = SceneCreatorModel.Bri, sat = SceneCreatorModel.Sat, ct = SceneCreatorModel.Ct, @on = SceneCreatorModel.On };
                if (SceneCreatorModel.X != null && SceneCreatorModel.Y != null)
                {
                    obj.state.xy = new decimal[]
                    {
                        Convert.ToDecimal(SceneCreatorModel.X),
                        Convert.ToDecimal(SceneCreatorModel.Y)
                    };
                }

                if (SceneCreatorModel.TT != null)
                {
                    obj.state.transitiontime = Convert.ToUInt16(SceneCreatorModel.TT);
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
                @on = SceneCreatorModel.On,
                ct = SceneCreatorModel.Ct,
                sat = SceneCreatorModel.Sat,
                transitiontime = SceneCreatorModel.TT               
            };

            if (SceneCreatorModel.X != null && SceneCreatorModel.Y != null)
            {
                SelectedSceneLight.state.xy = new decimal[]
                {
                    Convert.ToDecimal(SceneCreatorModel.X),
                    Convert.ToDecimal(SceneCreatorModel.Y)
                };
            }

            SelectedSceneLight = null;
            ClearSliders();
        }

        private void ClearSliders()
        {
            HueChecked = false;
            XYChecked = false;
            BriChecked = false;
            CTChecked = false;
            TtChecked = false;
            SatChecked = false;
            
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
            if (SceneCreatorModel.X == null &&
            SceneCreatorModel.Y == null &&
            SceneCreatorModel.Hue == null &&
            SceneCreatorModel.Bri == null &&
            SceneCreatorModel.Ct == null &&
            SceneCreatorModel.TT == null &&
            SceneCreatorModel.Sat == null && 
            SceneCreatorModel.On == true) return false;

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
        public ICommand CheckHueCommand => new RelayCommand(param => CheckHue(), param => CanCheckHue());
        public ICommand CheckCTCommand => new RelayCommand(param => CheckCT(), param => CanCheckCT());
        public ICommand CheckSatCommand => new RelayCommand(param => CheckSat(), param => CanCheckSat());
        public ICommand CheckXYCommand => new RelayCommand(param => CheckXY(), param => CanCheckXY());
        public ICommand CheckBriCommand => new RelayCommand(param => CheckBri(), param => CanCheckBri());
        public ICommand CheckTTCommand => new RelayCommand(param => CheckTT());



        private bool CanCheckBri()
        {
            if (SceneCreatorModel.On == false) return false;
            return true;
        }

        private bool CanCheckHue()
        {
            if (SceneCreatorModel.On == false) return false;
            if (XYChecked) return false;
            if (CTChecked) return false;
            return true;
        }

        private bool CanCheckSat()
        {
            if (SceneCreatorModel.On == false) return false;
            if (XYChecked) return false;
            if (CTChecked) return false;
            return true;
        }

        private bool CanCheckCT()
        {
            if (SceneCreatorModel.On == false) return false;
            if (HueChecked) return false;
            if (XYChecked) return false;
            if (SatChecked) return false;
            return true;
        }

        private bool CanCheckXY()
        {
            if (SceneCreatorModel.On == false) return false;
            if (HueChecked) return false;
            if (CTChecked) return false;
            if (SatChecked) return false;
            return true;
        }

        private void CheckTT()
        {
            if (TtChecked == false) SceneCreatorModel.TT = null;
        }

        private void CheckSat()
        {
            XYChecked = false;
            CTChecked = false;
            SceneCreatorModel.Ct = null;
            SceneCreatorModel.X = null;
            if (SatChecked == false) SceneCreatorModel.Sat = null;
        }

        private void CheckHue()
        {
            CTChecked = false;
            XYChecked = false;
            SceneCreatorModel.X = null;
            SceneCreatorModel.Ct = null;
            if (HueChecked == false) SceneCreatorModel.Hue = null;
        }
    
        private void CheckCT()
        {
            HueChecked = false;
            SatChecked = false;
            XYChecked = false;
            SceneCreatorModel.Hue = null;
            SceneCreatorModel.X = null;
            SceneCreatorModel.Sat = null;
            if (CTChecked == false) SceneCreatorModel.Ct = null;
        }

        private void CheckXY()
        {
            HueChecked = false;
            CTChecked = false;
            SatChecked = false;
            SceneCreatorModel.Ct = null;
            SceneCreatorModel.Hue = null;
            SceneCreatorModel.Sat = null;
            if (XYChecked == false) SceneCreatorModel.X = null;
        }

        private void CheckBri()
        {
            if (BriChecked == false) SceneCreatorModel.Bri = null;
        }

        public bool HueChecked
        {
            get => _hueChecked;
            set => SetProperty(ref _hueChecked,value);
        }

        public bool BriChecked
        {
            get => _briChecked;
            set => SetProperty(ref _briChecked,value);
        }

        public bool SatChecked
        {
            get => _satChecked;
            set => SetProperty(ref _satChecked,value);
        }

        public bool CTChecked
        {
            get => _ctChecked;
            set => SetProperty(ref _ctChecked,value);
        }

        public bool XYChecked
        {
            get => _xyChecked;
            set => SetProperty(ref _xyChecked,value);
        }

        public bool TtChecked
        {
            get => _ttChecked;
            set => SetProperty(ref _ttChecked,value);
        }
    }
}
