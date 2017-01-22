using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HueLib2;
using WinHue3.Resources;
using System.Threading;
using System.ComponentModel;

namespace WinHue3
{
    public class SceneCreatorView : View
    {
        private Scene _scene;
        private ObservableCollection<HueObject> _listAvailableLights;
        private ObservableCollection<HueObject> _listSceneLights;
        private HueObject _selectedSceneLight;
        private HueObject _selectedAvailableLight;
        private State _newstate;
        private double _ttvalue = -1;
        private bool _canpreviewscene = false;
        private bool _cansavescene = false;
        private bool _cancancel = true;
        private readonly Bridge _bridge;

        #region CTOR
        public SceneCreatorView(List<HueObject> listlights, Bridge bridge)
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
            _newstate = new State {@on = true};
            SetError(GlobalStrings.Scene_SelectOneLight, "ListSceneLights");
        }

        public SceneCreatorView(List<HueObject> listlights ,string sceneid,Bridge bridge)
        {
            _bridge = bridge;
             
            CommandResult cr= _bridge.GetObject<Scene>(sceneid);
            if (cr.Success)
            {
                _scene = (Scene) cr.resultobject;
                _listAvailableLights = new ObservableCollection<HueObject>(listlights);
                _listSceneLights = new ObservableCollection<HueObject>();
                _cansavescene = true;
                _canpreviewscene = true;
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

        }

        
        #endregion

        #region PROPERTIES

        public bool SceneOn
        {
            get
            {
                return (bool)CurrentState.on;
            }
            set
            {
                CurrentState.on = value;
                OnPropertyChanged();
            }
        }

        public HueObject SelectedAvailableLight
        {
            get
            {
                return _selectedAvailableLight;                
            }
            set
            {
                _selectedAvailableLight = value;
                OnPropertyChanged();
                OnPropertyChanged("CanAddLight");
            }
        }

        public bool CanSaveScene
        {
            get
            {
                return _cansavescene;
            }
            set
            {
                _cansavescene = value;
                OnPropertyChanged();
            }
        }

        public bool CanPreviewScene
        {
            get
            {
                return _canpreviewscene;
            }
            set
            {
                _canpreviewscene = value;
                OnPropertyChanged();
            }
        }

        public bool CanCancel
        {
            get
            {
                return _cancancel;
            }
            set
            {
                _cancancel = value;
                OnPropertyChanged();
            }
        }

        public bool CanAddLight
        {
            get
            {
                if (_selectedAvailableLight == null) return false;
                return true;
            }
        }

        public State CurrentState
        {
            get
            {
                if (_selectedSceneLight != null) return ((Light) _selectedSceneLight).state;
                return _newstate;
            }
            
        }

        public double Hue
        {
            get
            {
                if (CurrentState?.hue == null) return -1;
                return Convert.ToDouble(CurrentState.hue);
            }
            set
            {
                if (value == -1)
                {
                    CurrentState.hue = null;
                    OnPropertyChanged();
                    return;
                }
                CurrentState.hue = (ushort)value;
                OnPropertyChanged();
                CurrentState.ct = null;
                CurrentState.xy = null;
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
                OnPropertyChanged("CT");
                OnPropertyChanged("SliderSatGradientEndColor");

            }
        }

        public double Sat
        {
            get
            {
                if (CurrentState == null) return -1;
                return Convert.ToDouble(CurrentState.sat);
            }
            set
            {
                if (value == -1) CurrentState.sat = null;
                CurrentState.sat = (byte)value;
                OnPropertyChanged();
            }
        }

        public double CT
        {
            get
            {
                if (CurrentState == null) return 152;
                return Convert.ToDouble(CurrentState.ct);
            }
            set
            {
                if (value == -1) CurrentState.ct = null;
                CurrentState.hue = null;
                CurrentState.ct = (ushort)value;
                OnPropertyChanged();
                OnPropertyChanged("Hue");
                CurrentState.xy = null;
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
            }
        }

        public double Bri
        {
            get
            {
                if (CurrentState == null) return -1;
                return Convert.ToDouble(CurrentState.bri);
            }
            set
            {
                if (value == -1) CurrentState.bri = null;
                CurrentState.bri = (byte)value;
                OnPropertyChanged();
            }
        }

        public double TT
        {
            get
            {
                return _ttvalue;
            }
            set
            {
                _ttvalue = value;
                if (_ttvalue == -1)
                    CurrentState.transitiontime = null;
                else
                {
                    ushort result = Convert.ToUInt16(Math.Pow(2, (0.000244140625 * _ttvalue)) - 1);
                    CurrentState.transitiontime = result;
                }

                OnPropertyChanged();
                OnPropertyChanged("TransitionTimeMessage");
            }
        }

        public double X
        {
            get
            {
                if (CurrentState.xy == null) return -0.001;
                return Convert.ToDouble(CurrentState.xy.x);
            }
            set
            {
                if (CurrentState.xy == null)
                {
                    _newstate.xy = new XY();
                }
                CurrentState.xy.x = Convert.ToDecimal(value);
                OnPropertyChanged();
                CurrentState.hue = null;
                OnPropertyChanged("Hue");
                CurrentState.ct = null;
                OnPropertyChanged("CT");
            }
        }

        public double Y
        {
            get
            {
                if (CurrentState.xy == null) return -0.001;
                return Convert.ToDouble(CurrentState.xy.y);
            }
            set
            {
                if (CurrentState.xy == null)
                {
                    CurrentState.xy = new XY();
                }
                CurrentState.xy.y = Convert.ToDecimal(value);
                OnPropertyChanged();
                CurrentState.hue = null;
                OnPropertyChanged("Hue");
                CurrentState.ct = null;
                OnPropertyChanged("CT");
            }
        }


        public Color SliderSatGradientEndColor
        {
            get
            {
                double val = Hue / 273.06;
                System.Drawing.Color color = new HSLColor(val, 240, 120);
                return Color.FromRgb(color.R, color.G, color.B);
            }

        }

        public string TransitionTimeMessage
        {
            get
            {
                if (CurrentState.transitiontime >= 0)
                {
                    int time = (int)(CurrentState.transitiontime * 100);
                    if (time == 0)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {GUI.MainForm_Sliders_TransitionTime_Instant}";
                    }
                    else if (time > 0 && time < 1000)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {(double)time:0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Millisec}";
                    }
                    else if (time >= 1000 && time < 60000)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {((double)time / 1000):0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Seconds}";
                    }
                    else
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {((double)time / 60000):0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Minutes}";
                    }
                }
                else
                {
                    return $"{GUI.MainForm_Sliders_TransitionTime} : {GUI.MainForm_Sliders_TransitionTime_Unit_None}";
                }
            }
        }

        public bool CanRemoveSceneLight => _selectedSceneLight != null;

        public HueObject SelectedSceneLight 
        {
            get { return _selectedSceneLight; }
            set
            {
                _selectedSceneLight = value;
                OnPropertyChanged();
                OnPropertyChanged("CanRemoveSceneLight");
                OnPropertyChanged("Hue");
                OnPropertyChanged("Bri");
                OnPropertyChanged("Sat");
                OnPropertyChanged("CT");
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
                
                OnPropertyChanged("SceneOn");
                if (CurrentState.transitiontime != null)
                {
                    _ttvalue = Convert.ToDouble(Math.Sqrt((double) CurrentState.transitiontime + 1)/0.000244140625);
                    OnPropertyChanged("TT");
                    OnPropertyChanged("TransitionTimeMessage");
                }
                // Convert.ToUInt16(Math.Pow(2, (0.000244140625 * _ttvalue)) - 1)
            }
        }

        public string SceneName 
        {
            get
            {
                return _scene.name;
            }
            set
            {
                _scene.name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HueObject> ListAvailableLights => _listAvailableLights;

        public ObservableCollection<HueObject> ListSceneLights
        {
            get
            {
                return _listSceneLights;
            }
            set
            {
                _listSceneLights = value;
                OnPropertyChanged();
                OnPropertyChanged("ListAvailableLights");
                OnPropertyChanged("Hue");
                OnPropertyChanged("Sat");
                OnPropertyChanged("Bri");
                OnPropertyChanged("CT");
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
                OnPropertyChanged("TT");
            }
        } 

       
        #endregion

        #region METHODS

        private void RemoveSelectedSceneLight()
        {
            ((Light)_selectedSceneLight).state = new State();
            _scene.lights.Remove(_selectedSceneLight.Id);
            _listAvailableLights.Add(_selectedSceneLight);
            _scene.lightstates.Remove(_selectedSceneLight.Id);
            _listSceneLights.Remove(_selectedSceneLight);
            
            _selectedSceneLight = null;
            if (_listSceneLights.Count == 0)
                SetError(GlobalStrings.Scene_SelectOneLight, "ListSceneLights");
            OnPropertyChanged("SelectedSceneLight");
            CanSaveScene = _listSceneLights.Count > 0;
        }

        public HueObject GetScene()
        {
            return _scene;
        }

        public ObservableCollection<HueObject> GetSceneLights()
        {
            return _listSceneLights;
        }

        public void SetRandomColor()
        {
            Random rdm = new Random();
            Hue = rdm.Next(65535);
        }

        public void AddLightsToScene(List<HueObject> lightlist)
        {
            
            foreach (HueObject obj in lightlist)
            {
                _listAvailableLights.Remove(obj);
                ((Light)obj).state = _newstate;
                _listSceneLights.Add(obj);
                if(_scene.lightstates == null) _scene.lightstates = new Dictionary<string, State>();
                _scene.lightstates.Add(obj.Id,((Light)obj).state);
            }
            RemoveError(GlobalStrings.Scene_SelectOneLight, "ListSceneLights");
            _scene.lights.AddRange(lightlist.Select(x => x.Id).ToList());
            
            //_newstate = new State() { on = true};
            OnPropertyChanged("LightSceneLights");
            CanPreviewScene = true;
            CanSaveScene = true;
        }

        public void GetColorFromImage()
        {
            Form_SelectColorFromImage fsci = new Form_SelectColorFromImage() { Owner = Application.Current.MainWindow };
            if (fsci.ShowDialog() != true) return;
            Color c = fsci.GetSelectedColor();
            CGPoint color = HueColorConverter.CalculateXY(c,((Light)_selectedAvailableLight).modelid);
            if (CurrentState.xy == null)
            {
                CurrentState.xy = new XY();
            }
            CurrentState.sat = 255;
            CurrentState.xy.x = Convert.ToDecimal(color.x);
            CurrentState.xy.y = Convert.ToDecimal(color.y);
            OnPropertyChanged("X");
            OnPropertyChanged("Y");
            OnPropertyChanged("Sat");
        }

        public void ClearSelectionSceneLight()
        {
            _selectedSceneLight = null;
            SceneOn = true;
            OnPropertyChanged("SelectedSceneLight");
            OnPropertyChanged("Hue");
            OnPropertyChanged("Sat");
            OnPropertyChanged("Bri");
            OnPropertyChanged("CT");
            OnPropertyChanged("X");
            OnPropertyChanged("Y");
            OnPropertyChanged("TT");
        }

        private void DoPreviewScene()
        {         

            BackgroundWorker bgWorker = new BackgroundWorker() { WorkerReportsProgress = false, WorkerSupportsCancellation = false };
            bgWorker.DoWork += bgWorker_DoWork;
            bgWorker.RunWorkerAsync(_listSceneLights);
  

        }

        void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SetPreviewBtn(false);
            ObservableCollection<HueObject> li = (ObservableCollection<HueObject>)e.Argument;
            ObservableCollection<HueObject> liOriginalState = new ObservableCollection<HueObject>();
            foreach (HueObject obj in li)
            {
                HelperResult hr = HueObjectHelper.GetObject<Light>(_bridge, obj.Id);
                if (hr.Success)
                {
                    HueObject newlight = (HueObject) hr.Hrobject;
                    ((Light)newlight).state.alert = null;
                    liOriginalState.Add(newlight);
                }
            }

            foreach (HueObject obj in li)
            {
                _bridge.SetState<Light>(((Light) obj).state,obj.Id);
            }

            Thread.Sleep(5000);

            foreach (HueObject obj in liOriginalState)
            {
                _bridge.SetState<Light>(((Light)obj).state,obj.Id);
            }

            Thread.Sleep(2000);
            SetPreviewBtn(true);

        }

        delegate void SetBoolValue(bool value);

        private void SetPreviewBtn(bool state)
        {
            CanPreviewScene = state;
            CanCancel = state;
            CanSaveScene = state;
        }

        #endregion

        #region COMMANDS
        public ICommand RemoveSelectedSceneLightCommand => new RelayCommand(param => RemoveSelectedSceneLight());
        public ICommand SetRandomColorCommand => new RelayCommand(param => SetRandomColor());
        public ICommand GetColorFromImageCommand => new RelayCommand(param => GetColorFromImage());
        public ICommand ClearSelectionSceneLightCommand => new RelayCommand(param => ClearSelectionSceneLight());
        public ICommand DoPreviewSceneCommand => new RelayCommand(param => DoPreviewScene());
        #endregion
    }
}