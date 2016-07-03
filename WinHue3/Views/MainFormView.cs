using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using HueLib;
using HueLib_base;
using System.Windows;
using System.Reflection;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WinHue3.Classes;
using WinHuePluginModule;
using Action = HueLib_base.Action;
using Color = System.Windows.Media.Color;
using System.Collections.ObjectModel;
using WinHue3.Resources;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using MessageBox = System.Windows.MessageBox;


namespace WinHue3
{
    public class MainFormView : View
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Bridge _selectedBridge;
        private HueObject _selectedObject;
        private readonly DispatcherTimer _findlighttimer = new DispatcherTimer();
        private readonly DispatcherTimer _findsensortimer = new DispatcherTimer();
        private readonly DispatcherTimer _refreshStates = new DispatcherTimer();
        private ObservableCollection<HueObject> _listBridgeObjects;
        private readonly ObservableCollection<RibbonSplitButtonPlugin> _plugins = new ObservableCollection<RibbonSplitButtonPlugin>();
        private ObservableCollection<Bridge> _listBridges = new ObservableCollection<Bridge>();
        public Form_EventLog _fel;
        private Form_SceneMapping _fsm;
        private ushort? _transitiontime = null;
        private double _ttvalue = -1;
        private string _lastmessage = string.Empty;
        private List<HotKey> _listHotKeys;

        #pragma warning disable 649
        [ImportMany(typeof(IWinHuePluginModule), AllowRecomposition = true)]
        private IEnumerable<IWinHuePluginModule> _availablePlugins;
        #pragma warning restore 649

        #region CTOR

        public MainFormView(Form_EventLog fel)
        {
            _fel = fel;
            _findlighttimer.Interval = new TimeSpan(0, 1, 0);
            _findlighttimer.Tick += _findlighttimer_Tick;
            _findsensortimer.Interval = new TimeSpan(0,1,0);
            _findsensortimer.Tick += _findsensortimer_Tick;
            _refreshStates.Interval = new TimeSpan(0, 0, 3);
            _refreshStates.Tick += _refreshStates_Tick;
            _listHotKeys = WinHueSettings.settings.listHotKeys;
            _refreshStates.Start();
            Cursor_Tools.ShowWaitCursor();

            // Load from the settings.
            log.Info("Loading bridge from saved settings...");
            foreach (KeyValuePair<string, BridgeSaveSettings> kvp in WinHueSettings.settings.BridgeInfo)
            {
                log.Info($"Bridge {kvp.Value.ip} added.");
                _listBridges.Add(new Bridge(IPAddress.Parse(kvp.Value.ip), kvp.Key, kvp.Value.apiversion, kvp.Value.swversion, kvp.Value.name, kvp.Value.apikey));
                OnPropertyChanged("MultiBridgeCB");
            }

            LoadBridge();

        }

        private void _refreshStates_Tick(object sender, EventArgs e)
        {
            if (_listBridgeObjects == null) return;
            //log.Debug("Automatic refresh in progress");

            List<HueObject> ll = _listBridgeObjects.Where(x => x.GetType() == typeof(Light)).ToList();

            foreach (HueObject obj in ll)
            {
                RefreshObject(obj);
            }

        }

        #endregion

        #region PROPERTIES

        public bool IsValidScene
        {
            get
            {
                if (_selectedObject == null) return false;
                if (!(_selectedObject is Scene)) return false;
                return ((Scene)_selectedObject).version != 1;
            }
        }

        public Visibility MultiBridgeCB
        {
            get { return _listBridges.Count > 1 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ObservableCollection<Bridge> ListBridges
        {
            get { return _listBridges; }
        } 

        public bool EnableSearchLights
        {
            get
            {
                if (_selectedBridge == null) return false;
                bool result = false;
                result = !_findlighttimer.IsEnabled;
                return EnableControls && result;
            }
        }

        public bool EnableSearchSensors
        {

            get
            {
                if (_selectedBridge == null) return false;
                bool result = false;
                result = !_findsensortimer.IsEnabled;
                return EnableControls && result;
            }
        }

        public bool IsLightOrGroup
        {
            get
            {
                if (_selectedObject == null) return false;
                return (_selectedObject is Light) || (_selectedObject is Group);
            }
        }

        public SolidColorBrush XYRectangleColor => new SolidColorBrush(ColorConversion.ConvertXYToColor(new PointF((float)SliderX, (float)SliderY), 1));

        public ObservableCollection<RibbonSplitButtonPlugin> ListPlugins => _plugins;

        public bool CanDeleteObject
        {
            get
            {
                if (_selectedObject == null) return false;
                return true;
            }
        }

        public string TransitionTimeTooltip
        {
            get
            {
                if (_transitiontime >= 0)
                {
                    int time = (int)(_transitiontime * 100);
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

        public bool EnableHueSlider
        {
            get
            {
                if (_selectedObject == null) return false;
                bool result = false;
                if (_selectedObject is Light)
                    result = ((Light) _selectedObject).state.hue != null;
                else if (_selectedObject is Group)
                    result = ((Group) _selectedObject).action.hue != null;                
                return EnableSliders & result;
            }
        }

        public bool EnableCTSlider
        {
            get
            {
                if (_selectedObject == null) return false;
                bool result = false;
                if (_selectedObject is Light)
                    result = ((Light)_selectedObject).state.ct != null;
                else if (_selectedObject is Group)
                    result = ((Group)_selectedObject).action.ct != null;
                return EnableSliders & result;
            }
        }

        public bool EnableBriSlider
        {
            get
            {
                if (_selectedObject == null) return false;
                bool result = false;
                if (_selectedObject is Light)
                    result = ((Light)_selectedObject).state.bri != null;
                else if (_selectedObject is Group)
                    result = ((Group)_selectedObject).action.bri != null;
                return EnableSliders & result;
            }
        }

        public bool EnableSatSlider
        {
            get
            {
                if (_selectedObject == null) return false;
                bool result = false;
                if (_selectedObject is Light)
                    result = ((Light)_selectedObject).state.sat != null;
                else if (_selectedObject is Group)
                    result = ((Group)_selectedObject).action.sat != null;
                return EnableSliders & result;
            }
        }

        public bool EnableXYSliders
        {
            get
            {
                if (_selectedObject == null) return false;
                bool result = false;
                if (_selectedObject is Light)
                    result = ((Light)_selectedObject).state.xy != null;
                else if (_selectedObject is Group)
                    result = ((Group)_selectedObject).action.xy != null;
                return EnableSliders & result;
            }
        }

        public bool EnableTTSlider
        {
            get
            {
                if (_selectedObject == null) return false;
                return EnableSliders;
            }
        }

        public Color SliderSatGradientEndColor
        {
            get
            {
                double val = SliderHue / 273.06;
                System.Drawing.Color color = new HSLColor(val, 240, 120); 
                return Color.FromRgb(color.R, color.G, color.B);
            }
        } 

        public double SliderHue
        {
            get
            {
                if (_selectedObject == null || _selectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light) _selectedObject).state.hue == null ? 0 : Convert.ToDouble(((Light) _selectedObject).state.hue);
                if(_selectedObject is Group)
                    return ((Group) _selectedObject).action.hue == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.hue);
                return 0;
            }
           set
           {
                if (_selectedObject == null || _selectedBridge == null) return;
                if (_selectedObject is Light)
                   ((Light)_selectedObject).state.hue = (ushort)value;
                if (_selectedObject is Group)
                   ((Group) _selectedObject).action.hue = (ushort)value;
                OnPropertyChanged();
                OnPropertyChanged("SliderSatGradientEndColor");
           }
        }

        public double SliderBri
        {
            get
            {
                if (_selectedObject == null || _selectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light)_selectedObject).state.bri == null ? 0 : Convert.ToDouble(((Light)_selectedObject).state.bri);
                if (_selectedObject is Group)
                    return ((Group)_selectedObject).action.bri == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.bri);
                return 0;
            }
            set
            {
                if (_selectedObject == null || _selectedBridge == null) return;
                if (_selectedObject is Light)
                    ((Light)_selectedObject).state.bri = (byte)value;
                if (_selectedObject is Group)
                    ((Group)_selectedObject).action.bri = (byte)value;
                OnPropertyChanged();
            }
        }

        public double SliderCT
        {
            get
            {
                if (_selectedObject == null || _selectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light)_selectedObject).state.ct == null ? 0 : Convert.ToDouble(((Light)_selectedObject).state.ct);
                if (_selectedObject is Group)
                    return ((Group)_selectedObject).action.ct == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.ct);
                return 0;
            }
            set
            {
                if (_selectedObject == null || _selectedBridge == null) return;
                if (_selectedObject is Light)
                    ((Light)_selectedObject).state.ct = (ushort)value;
                if (_selectedObject is Group)
                    ((Group)_selectedObject).action.ct = (ushort)value;
                OnPropertyChanged();
            }
        }

        public double SliderSat
        {
            get
            {
                if (_selectedObject == null || _selectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light)_selectedObject).state.sat == null ? 0 : Convert.ToDouble(((Light)_selectedObject).state.sat);
                if (_selectedObject is Group)
                    return ((Group)_selectedObject).action.sat == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.sat);
                return 0;
            }
            set
            {
                if (_selectedObject == null || _selectedBridge == null) return;
                if (_selectedObject is Light)
                    ((Light)_selectedObject).state.sat = (byte)value;
                if (_selectedObject is Group)
                    ((Group)_selectedObject).action.sat = (byte)value;
                OnPropertyChanged();
            }
        }

        public double SliderX
        {
            get
            {
                if (_selectedObject == null || _selectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return Convert.ToDouble(((Light)_selectedObject).state.xy?.x);
                if (_selectedObject is Group)
                    return Convert.ToDouble(((Group)_selectedObject).action.xy?.x);
                return 0;
            }
            set
            {
                if (_selectedObject == null || _selectedBridge == null) return;
                if (_selectedObject is Light)
                    ((Light)_selectedObject).state.xy.x = (decimal)value;
                if (_selectedObject is Group)
                    ((Group)_selectedObject).action.xy.x = (decimal)value;
                OnPropertyChanged();
                OnPropertyChanged("XYRectangleColor");
            }
        }

        public double SliderY
        {
            get
            {
                if (_selectedObject == null || _selectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return Convert.ToDouble(((Light)_selectedObject).state.xy?.y);
                if (_selectedObject is Group)
                    return Convert.ToDouble(((Group)_selectedObject).action.xy?.y);
                return 0;
            }
            set
            {
                if (_selectedObject == null || _selectedBridge == null) return;
                if (_selectedObject is Light)
                    ((Light)_selectedObject).state.xy.y = (decimal)value;
                if (_selectedObject is Group)
                    ((Group)_selectedObject).action.xy.y = (decimal)value;
                OnPropertyChanged();
                OnPropertyChanged("XYRectangleColor");
            }
        }

        public double SliderTT
        {
            get
            {
                if (_selectedObject == null || _selectedBridge == null) return -1;
                return _ttvalue;
            }
            set
            {
                _ttvalue = value;
                if (_ttvalue == -1)
                    _transitiontime = null;            
                
                else
                {
                    ushort result = (ushort) (Math.Pow(2, 0.000244140625* _ttvalue) - 1);
                    _transitiontime = result;
                }

                OnPropertyChanged();
                OnPropertyChanged("TransitionTimeTooltip");
            }
        }

        public Bridge SelectedBridge
        {
            get { return _selectedBridge; }
            set
            {
                _selectedBridge = value;
                OnPropertyChanged();
                if (_selectedBridge == null) return;
                RefreshView();
                OnPropertyChanged("ListBridgeObjects");
                OnPropertyChanged("EnableControls");
                OnPropertyChanged("UpdateAvailable");
                
            }
        }

        public HueObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = HueObjectHelper.GetBridgeObject(_selectedBridge, value);
                log.Debug("Selected object : " + _selectedObject);
                OnPropertyChanged();
                OnPropertyChanged("EnableSliders");
                OnPropertyChanged("CanSchedule");                
                OnPropertyChanged("EnableHueSlider");
                OnPropertyChanged("EnableBriSlider");
                OnPropertyChanged("EnableCTSlider");
                OnPropertyChanged("EnableSatSlider");
                OnPropertyChanged("EnableXYSliders");
                OnPropertyChanged("EnableTTSlider");
                OnPropertyChanged("SliderHue");
                OnPropertyChanged("SliderSat");
                OnPropertyChanged("SliderCT");
                OnPropertyChanged("SliderBri");
                OnPropertyChanged("SliderX");
                OnPropertyChanged("SliderY");
                OnPropertyChanged("CanDeleteObject");
                OnPropertyChanged("IsValidScene");
                OnPropertyChanged("CanEditObject");
            }
        }

        public ObservableCollection<HueObject> ListBridgeObjects => _listBridgeObjects;

        public bool EnableControls => _selectedBridge != null;

        public bool EnableSliders
        {
            get
            {
                if (_selectedObject == null) return false;
                return _selectedObject is Light || _selectedObject is Group; 
            }
        }

        public bool CanSchedule
        {
            get
            {
                if (_selectedObject == null) return false;
                return _selectedObject is Light || _selectedObject is Group;
            }
        }

        public Visibility UpdateAvailable
        {
            get
            {
                if (_selectedBridge == null) return Visibility.Collapsed;
                return _selectedBridge.GetSwUpdate() ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        public bool CanEditObject
        {
            get
            {
                if (_selectedObject is Light) return false;
                if(_selectedObject is Scene)
                    if (((Scene) _selectedObject).version < 2) return false; 
                return true;
            }
        }

        public string StatusBarMessage => _selectedBridge == null ? string.Empty : _lastmessage;

        #endregion

        #region METHODS

        /// <summary>
        /// Associate the bridge with it's apikey if already paired.
        /// </summary>
        private ObservableCollection<Bridge> AssociateBridgeWithApiKey(ObservableCollection<Bridge> bridge)
        {
            foreach (Bridge br in bridge)
            {
                
                if (!WinHueSettings.settings.BridgeInfo.ContainsKey(br.Mac)) continue;
                log.Debug($"Associating bridge {br.IpAddress} with APIkey {WinHueSettings.settings.BridgeInfo[br.Mac].apikey}");
                br.ApiKey = WinHueSettings.settings.BridgeInfo[br.Mac].apikey;
                if (!HueObjectHelper.IsAuthorized(br)) br.ApiKey = string.Empty;
                br.IsDefault = br.Mac == WinHueSettings.settings.DefaultBridge;
            }
            return bridge;
        }

        private void LoadBridge()
        {
            log.Info("Loading bridge...");
            _listBridges = AssociateBridgeWithApiKey(_listBridges);

            // if None of the bridge are paired or if there is no default bridge
            if (_listBridges.All(x => x.ApiKey == string.Empty) || (WinHueSettings.settings.DefaultBridge == string.Empty))
            {
                log.Warn("No bridge paired/Authorized or no default bridge.... Please do the pairing procedure.");
                Cursor_Tools.ShowNormalCursor();
                if (MessageBox.Show(GlobalStrings.BridgeSetupNotCompleted, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    DoBridgePairing();

            }
            else
            {
                Cursor_Tools.ShowWaitCursor();
                log.Info("Trying to find the default bridge...");
                Bridge temp = _listBridges.FirstOrDefault(x => x.Mac == WinHueSettings.settings.DefaultBridge);
                log.Info($"Selecting default bridge {temp.IpAddress}");
                if (temp.Equals(default(Bridge)))
                {
                    log.Warn("The default bridge not found. Using the first bridge in the list if any.");
                    if (_listBridges.Count == 0)
                    {
                        log.Error("No other bridge to chose. Please check that your default bridge is on.");
                    }
                    else
                    {
                        log.Error("Selecting the first bridge in the list as default bridge.");
                        temp = _listBridges[0];
                    }
                }
                            
                temp.OnMessageAdded += MessageAdded;
                SelectedBridge = temp;

                LoadPlugins();
                Cursor_Tools.ShowNormalCursor();
            }

        }

        void MessageAdded(object sender, EventArgs e)
        {
            if (_selectedBridge.lastMessages != null)
            {
                log.Info(_selectedBridge.lastMessages);
                if(_selectedBridge.lastMessages.Count > 0)
                    _lastmessage = _selectedBridge.lastMessages[_selectedBridge.lastMessages.Count - 1].ToString();
            }

            OnPropertyChanged("StatusBarMessage");

        }

        private void DoBridgeUpdate()
        {
            if (_selectedBridge == null) return;
            if (MessageBox.Show(GlobalStrings.Update_Confirmation, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            log.Info("Updating bridge to the latest firmware.");
            if (!_selectedBridge.DoSwUpdate())
            {
                log.Error("An error occured while trying to start a bridge update. Please try again later.");
                return;
            }
            else
            {
                Form_Wait fw = new Form_Wait();
                fw.ShowWait(GlobalStrings.ApplyingUpdate,new TimeSpan(0,0,0,180), Application.Current.MainWindow );
                BridgeSettings brs = _selectedBridge.GetBridgeSettings();
                if (brs != null)
                {
                    switch (brs.swupdate.updatestate)
                    {
                        case 0:
                            log.Info("Bridge updated succesfully");
                            _selectedBridge.SetNotify();
                            break;
                        case 1:
                            log.Info("Bridge update incoming. Please wait for it to be ready.");
                            break;
                        case 2:
                            log.Info("Bridge update still available or a new update is available.");
                            break;
                        default:
                            log.Error("An error occured while updating the bridge. Please try again later.");
                            break;

                    }
                }

            }
            
            OnPropertyChanged("UpdateAvailable");
        }

        private void CheckForNewBulb()
        {
            if (_selectedBridge == null) return;
            
            if (_selectedBridge.SearchNewLights())
            {
                log.Info("Seaching for new lights...");
                _findlighttimer.Start();
                OnPropertyChanged("EnableSearchLights");
            }
            else
            {
                MessageBox.Show(GlobalStrings.Error_Getting_NewLights, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error("There was an error while looking for new lights.");
            }
        }


        private void _findlighttimer_Tick(object sender, EventArgs e)
        {
            _findlighttimer.Stop();
            log.Info("Done searching for new lights.");
            List<HueObject> newlights = HueObjectHelper.GetBridgeNewLights(_selectedBridge);

            log.Info($"Found {newlights.Count} new sensors.");
            _listBridgeObjects.AddRange(newlights);
            OnPropertyChanged("ListBridgeObjects");
            OnPropertyChanged("EnableSearchLights");
        }

        private void _findsensortimer_Tick(object sender, EventArgs e)
        {
            _findsensortimer.Stop();
            log.Info("Done searching for new sensors.");
            List<HueObject> newsensors = HueObjectHelper.GetBridgeNewSensors(_selectedBridge);
            log.Info($"Found {newsensors.Count} new sensors.");
            _listBridgeObjects.AddRange(newsensors);
            OnPropertyChanged("ListBridgeObjects");
            OnPropertyChanged("EnableSearchSensors");
        }

        private void SliderChangeHue()
        {
            if (_selectedObject == null || _selectedBridge == null) return;
            if (_selectedObject is Light)
                _selectedBridge.SetLightState(_selectedObject.Id, new State() { hue = (ushort)SliderHue, transitiontime = _transitiontime});
            if (_selectedObject is Group)
                _selectedBridge.SetGroupAction(_selectedObject.Id, new Action() { hue = (ushort)SliderHue, transitiontime = _transitiontime });
        }

        private void SliderChangeBri()
        {
            if (_selectedObject == null || _selectedBridge == null) return;
            if (_selectedObject is Light)
                _selectedBridge.SetLightState(_selectedObject.Id, new State() { bri = (byte)SliderBri, transitiontime = _transitiontime });
            if (_selectedObject is Group)
                _selectedBridge.SetGroupAction(_selectedObject.Id, new Action() { bri = (byte)SliderBri, transitiontime = _transitiontime });
        }

        private void SliderChangeCT()
        {
            if (_selectedObject == null || _selectedBridge == null) return;
            if (_selectedObject is Light)
                _selectedBridge.SetLightState(_selectedObject.Id, new State() { ct = (ushort)SliderCT, transitiontime = _transitiontime });
            if (_selectedObject is Group)
                _selectedBridge.SetGroupAction(_selectedObject.Id, new Action() { ct = (ushort)SliderCT, transitiontime = _transitiontime });
        }

        private void SliderChangeSat()
        {
            if (_selectedObject == null || _selectedBridge == null) return;
            if (_selectedObject is Light)
                _selectedBridge.SetLightState(_selectedObject.Id, new State() { sat = (byte)SliderSat, transitiontime = _transitiontime });
            if (_selectedObject is Group)
                _selectedBridge.SetGroupAction(_selectedObject.Id, new Action() { sat = (byte)SliderSat, transitiontime = _transitiontime });
        }

        private void SliderChangeXY()
        {
            if (_selectedObject == null || _selectedBridge == null) return;
            if (_selectedObject is Light)
                _selectedBridge.SetLightState(_selectedObject.Id, new State() { xy = new XY() {x = (decimal)SliderX,y = (decimal)SliderY}, transitiontime = _transitiontime });
            if (_selectedObject is Group)
                _selectedBridge.SetGroupAction(_selectedObject.Id, new Action() { xy = new XY() { x = (decimal)SliderX, y = (decimal)SliderY }, transitiontime = _transitiontime });
        }

        private void DoBridgePairing()
        {
            Form_BridgeDetectionPairing dp = new Form_BridgeDetectionPairing() {Owner = Application.Current.MainWindow };
            if(dp.ShowDialog() == true)
            {
                _listBridges = dp.GetModifications();
                LoadBridge();
            }
        }

        #region CONTEXT MENU COMMANDS
        private void DeleteObject()
        {
            if (_selectedObject == null) return;
            if (MessageBox.Show(string.Format(GlobalStrings.Confirm_Delete_Object, _selectedObject.GetName()),GlobalStrings.Warning,MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                bool result = false;
                if (_selectedObject is Light)
                {
                    result = _selectedBridge.DeleteLight(_selectedObject.Id);
                }
                else if (_selectedObject is Group)
                {
                    result = _selectedBridge.DeleteGroup(_selectedObject.Id);
                }
                else if(_selectedObject is Scene)
                {
                    result = _selectedBridge.DeleteScene(_selectedObject.Id);
                }
                else if (_selectedObject is Sensor)
                {
                    result = _selectedBridge.DeleteSensor(_selectedObject.Id);
                }
                else if(_selectedObject is Rule)
                {
                    result = _selectedBridge.DeleteRule(_selectedObject.Id);
                }
                else if (_selectedObject is Schedule)
                {
                    result = _selectedBridge.DeleteSchedule(_selectedObject.Id);
                }
                log.Debug("Result : " + result);
                if (result)
                {
                    int index = _listBridgeObjects.FindIndex(x => x.Id == _selectedObject.Id && x.GetType() == _selectedObject.GetType());
                    if (index == -1) return;
                    _listBridgeObjects.RemoveAt(index);
                    OnPropertyChanged("ListBridgeObjects");
                    SelectedObject = null;
                    log.Info($"Object ID : {index} removed.");
                }
            }
        }

        private void RenameObject()
        {
            Form_RenameObject fro = new Form_RenameObject(_selectedBridge,_selectedObject) {Owner = Application.Current.MainWindow};
            if (fro.ShowDialog() != true) return;
            _selectedObject.SetName(fro.GetNewName());
            int index = _listBridgeObjects.FindIndex(x => x.Id == _selectedObject.Id && x.GetType() == _selectedObject.GetType());
            if (index == -1) return;
            _listBridgeObjects[index].SetName(fro.GetNewName());
            log.Info($"Renamed object ID : {index} renamed.");
        }

        private void RefreshObject(HueObject obj, bool logging = false)
        {
            HueObject newobj;

            if (obj is Light)
            {
                newobj = HueObjectHelper.GetBridgeLight(_selectedBridge, obj.Id);
            }
            else if (obj is Group)
            {
                newobj = HueObjectHelper.GetBridgeGroup(_selectedBridge, obj.Id);
            }
            else if (obj is Scene)
            {
                newobj = HueObjectHelper.GetBridgeScene(_selectedBridge, obj.Id);
            }
            else if (obj is Sensor)
            {
                newobj = HueObjectHelper.GetBridgeSensor(_selectedBridge, obj.Id);
            }
            else if (obj is Rule)
            {
                newobj = HueObjectHelper.GetBridgeRule(_selectedBridge, obj.Id);
            }
            else if (obj is Schedule)
            {
                newobj = HueObjectHelper.GetBridgeSchedule(_selectedBridge,obj.Id);
            }
            else
            {
                newobj = null;
            }

            if(logging) log.Debug("Refreshing Object : " + newobj.ToString());
            if (newobj == null) return;

            int index = _listBridgeObjects.FindIndex(x => x.Id == obj.Id && x.GetType() == obj.GetType());
            if (index == -1) return;
            _listBridgeObjects[index] = newobj;
            //SelectedObject = newobj;
            OnPropertyChanged("ListBridgeObjects");
            if(logging) log.Info($"Refreshed Object ID: {index}");
        }

        private void EditObject()
        {
            if (_selectedObject == null) return;
            if (_selectedObject is Light) return;
            log.Debug("Editing object : " + _selectedObject);

            if(_selectedObject is Group)
            {
                Form_GroupCreator fgc = new Form_GroupCreator(_selectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
                if(fgc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Schedule)
            {
                Form_ScheduleCreator fsc = new Form_ScheduleCreator(_selectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
                if (fsc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Sensor)
            {
                Sensor obj = (Sensor)_selectedObject;
                if (obj.modelid == "PHDL00")
                {
                    Form_Daylight dl = new Form_Daylight(_selectedBridge, obj);
                    dl.Owner = Application.Current.MainWindow;
                    if(dl.ShowDialog() == true)
                    {
                        RefreshObject(_selectedObject);
                    }
                }
                else if (obj.modelid == "ZGPSWITCH")
                {
                    Form_HueTapConfig htc = new Form_HueTapConfig(_selectedBridge, obj.Id);
                    htc.Owner = Application.Current.MainWindow;
                    if(htc.ShowDialog() == true)
                    {
                        RefreshObject(_selectedObject);
                    }

                }
                else
                {
                    Form_SensorCreator fsc = new Form_SensorCreator(_selectedBridge,obj);
                    fsc.Owner = Application.Current.MainWindow;
                    if (fsc.ShowDialog() == true)
                    {
                        RefreshObject(_selectedObject);
                    }

                }
            }
            else if (_selectedObject is Rule)
            {
                Form_RulesCreator2 frc = new Form_RulesCreator2(_selectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
                if (frc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Scene)
            {
                Form_SceneCreator fscc = new Form_SceneCreator(_selectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
                if (fscc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
        }


        #endregion

        #region HOME TOOLBAR 

        private void ChangeBridgeSettings()
        {
            Form_BridgeSettings frs = new Form_BridgeSettings(_selectedBridge) { Owner = Application.Current.MainWindow };
            if (frs.ShowDialog() == true)
            {
                string newname = _selectedBridge.GetBridgeSettings().name;
                if (_selectedBridge.Name != newname)
                {
                    _selectedBridge.Name = newname;
                    _listBridges[_listBridges.IndexOf(_selectedBridge)].Name = newname;
                    Bridge selbr = _selectedBridge;
                    SelectedBridge = null;
                    SelectedBridge = selbr;
                    OnPropertyChanged("ListBridges");
                }
                RefreshView();
            }

        }

        private void RefreshView()
        {
            _listBridgeObjects = null;
            OnPropertyChanged("ListBridgeObjects");
            log.Info($"Getting list of objects from bridge at {_selectedBridge.IpAddress}.");
            _listBridgeObjects = new ObservableCollection<HueObject>(HueObjectHelper.GetBridgeDataStore(_selectedBridge));
            log.Info($"Found {_listBridgeObjects.Count} objects in the bridge.");
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_listBridgeObjects);
            view.GroupDescriptions?.Clear();
            PropertyGroupDescription groupDesc = new TypeGroupDescription();
            view.GroupDescriptions?.Add(groupDesc);
            OnPropertyChanged("ListBridgeObjects");
            _selectedObject = null;
            OnPropertyChanged("SelectedObject");
            log.Info($"Finished refreshing view.");
        }

        private void CreateGroup()
        {
            Form_GroupCreator fgc = new Form_GroupCreator(_selectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the Group creator window for bridge {_selectedBridge.IpAddress} ");
            if (fgc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created group ID {fgc.GetCreatedOrModifiedID()} from bridge {_selectedBridge.IpAddress}");
            _listBridgeObjects.Add(HueObjectHelper.GetBridgeGroup(_selectedBridge,fgc.GetCreatedOrModifiedID()));
        }

        private void CreateScene()
        {
            Form_SceneCreator fsc = new Form_SceneCreator(_selectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the scene creator window for bridge {_selectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created scene ID {fsc.GetCreatedOrModifiedID()} from bridge {_selectedBridge.IpAddress}");
            _listBridgeObjects.Add(HueObjectHelper.GetBridgeScene(_selectedBridge,fsc.GetCreatedOrModifiedID()));
            
        }

        private void CreateSchedule()
        {
            Form_ScheduleCreator fscc = new Form_ScheduleCreator(_selectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the schedule creator window passing bridge {_selectedBridge.IpAddress} ");
            if (fscc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created schedule ID {fscc.GetCreatedOrModifiedID()} from bridge {_selectedBridge.IpAddress}");
            _listBridgeObjects.Add(HueObjectHelper.GetBridgeSchedule(_selectedBridge,fscc.GetCreatedOrModifiedID()));
        }

        private void CreateRule()
        {
            Form_RulesCreator2 frc = new Form_RulesCreator2(_selectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the rule creator window passing bridge {_selectedBridge.IpAddress} ");
            if (frc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created rule ID {frc.GetCreatedOrModifiedId()} from bridge {_selectedBridge.IpAddress}");
            _listBridgeObjects.Add(HueObjectHelper.GetBridgeRule(_selectedBridge,frc.GetCreatedOrModifiedId()));
        }

        private void CreateSensor()
        {
            Form_SensorCreator fsc = new Form_SensorCreator(_selectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the sensor creator window passing bridge {_selectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created sensor ID {fsc.GetCreatedOrModifiedID()} from bridge {_selectedBridge.IpAddress}");
            _listBridgeObjects.Add(HueObjectHelper.GetBridgeSensor(_selectedBridge, fsc.GetCreatedOrModifiedID()));
        }

        private void CreateAnimation()
        {
            Form_AnimationCreator fac = new Form_AnimationCreator(SelectedBridge) {Owner = Application.Current.MainWindow};
            log.Debug($@"Opening the animation creator windows passing bridge {_selectedBridge.IpAddress} ");
            if (fac.ShowDialog() != true) return;
        }

        private void AllOn()
        {
            if (_selectedBridge == null) return;
            log.Info("Sending all on command to bridge" + _selectedBridge.IpAddress);
            if (_selectedBridge.SetGroupAction("0", new Action() {@on = true}).SuccessCount == 1)
            {
                log.Debug("Refreshing the main view.");
                RefreshView();
            }
        }

        private void AllOff()
        {
            if (_selectedBridge == null) return;
            log.Info("Sending all off command to bridge" + _selectedBridge.IpAddress);
            if (_selectedBridge.SetGroupAction("0", new Action() {@on = false}).SuccessCount == 1)
            {
                log.Debug("Refreshing the main view.");
                RefreshView();
            }

        }

        private void ShowEventLog()
        {
            if (_fel.IsVisible) return;
            log.Debug("Opening event log.");
            _fel.Show();

        }

        private void SearchNewSensors()
        {
            if (_selectedBridge == null) return;
            if(_selectedBridge.FindNewSensors())
            {
                log.Info("Looking for new sensors for 1 minute.");
                _findsensortimer.Start();
                OnPropertyChanged("EnableSearchSensors");
            }
            else
            {
                log.Error("Unable to look for new sensors. Please check the log for more details.");
            }
        }

        private void DoubleClickObject()
        {
            if (_selectedObject == null) return;
            if (!(_selectedObject is Light) && !(_selectedObject is Group) && !(_selectedObject is Scene)) return;
            log.Debug("Double click on : " + _selectedObject);
            if ((_selectedObject is Light) || (_selectedObject is Group))
            {
                ImageSource newimg = HueObjectHelper.ToggleObjectOnOffState(_selectedBridge, _selectedObject);
                if (newimg == null) return;
                _selectedObject.Image = newimg;
                int index = _listBridgeObjects.FindIndex(x => x.Id == _selectedObject.Id && x.GetType() == _selectedObject.GetType());
                if (index == -1) return;
                if (_selectedObject.HasProperty("state"))
                {
                    ((Light) _selectedObject).state.on = !((Light) _selectedObject).state.on;
                    ((Light) _listBridgeObjects[index]).state.on = !((Light) _listBridgeObjects[index]).state.on;
                }
                else
                {
                    ((Group) _selectedObject).action.on = !((Group) _selectedObject).action.on;
                    ((Group) _listBridgeObjects[index]).action.on = !((Group) _listBridgeObjects[index]).action.on;
                }

                _listBridgeObjects[index].Image = newimg;
                OnPropertyChanged("SelectedObject");
            }
            else
            {
                log.Info($"Activating scene : {_selectedObject.Id}");
                _selectedBridge.ActivateScene(_selectedObject.Id);
              /*  Scene scene = (Scene) _selectedObject;
                foreach (string s in scene.lights)
                {
                    HueObject hueobj = _listBridgeObjects.First(x => x.Id == s && x.GetType() == typeof (Light));
                    RefreshObject(hueobj);
                }*/
            }
            
        }
          
        private void IdentifyLong()
        {
            if (_selectedObject == null) return;
            if (!(_selectedObject is Light) && !(_selectedObject is Group)) return;
            if(_selectedObject is Light)
            {
                log.Info($@"Sending the long Identify command to light ID : {_selectedObject.Id}");
                _selectedBridge.SetLightState(_selectedObject.Id, new State() { alert = "lselect" });
            }
            else
            {
                log.Info($@"Sending the long Identify command to group ID : {_selectedObject.Id}");
                _selectedBridge.SetGroupAction(_selectedObject.Id, new Action() { alert = "lselect" });
            }
        }

        private void IdentifyShort()
        {
            if (_selectedObject == null) return;
            if (!(_selectedObject is Light) && !(_selectedObject is Group)) return;
            if (_selectedObject is Light)
            {
                log.Info($@"Sending the short Identify command to light ID : {_selectedObject.Id}");
                _selectedBridge.SetLightState(_selectedObject.Id, new State() { alert = "select" });
            }
            else
            {
                log.Info($@"Sending the short Identify command to group ID : {_selectedObject.Id}");
                _selectedBridge.SetGroupAction(_selectedObject.Id, new Action() { alert = "select" });
            }
        }


        public void ReplaceCurrentState()
        {
            if (_selectedObject == null) return;
            if (!(_selectedObject is Scene)) return;
            if (MessageBox.Show(GlobalStrings.Scene_Replace_Current_States, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            log.Info($@"Replacing scene {((Scene)_selectedObject).name} lights state with current one.");
            _selectedBridge.SetScene(_selectedObject.Id);
        }

        public void KeyPress(Key k)
        {
            if (k == Key.Delete)
            {
                DeleteObject();
            }

        }

        public void CreateHotKey()
        {
            Form_HotKeyCreator fhkc = new Form_HotKeyCreator(SelectedBridge) {Owner = Application.Current.MainWindow};
            fhkc.ShowDialog();
            _listHotKeys = fhkc.GetHotKeys();
        }

        #endregion

        public void HandleHotkey(KeyEventArgs e)
        {
            ModifierKeys m = e.KeyboardDevice.Modifiers;
            Key k = e.Key;
            try
            {
                HotKey h = _listHotKeys.First(x => x.Modifier == m && x.Key == k);
                if (h.objecType != null)
                {
                    if (h.objecType == typeof(Light))
                    {
                        _selectedBridge.SetLightState(h.id, new State(h.properties));
                    }
                    else if (h.objecType == typeof(Group))
                    {
                        _selectedBridge.SetGroupAction(h.id, new Action(h.properties));
                    }
                    else if (h.objecType == typeof(Scene))
                    {
                        _selectedBridge.ActivateScene(h.id);
                    }
                }
                else
                {
                    if (_selectedObject != null)
                    {
                        if (_selectedObject is Light)
                        {
                            _selectedBridge.SetLightState(_selectedObject.Id, new State(h.properties));
                        }
                        else if (_selectedObject is Group)
                        {
                            _selectedBridge.SetGroupAction(_selectedObject.Id, new Action(h.properties));
                        }
                        else
                        {
                            log.Warn("Cannot apply this Hotkey to this type of object.");
                        }
                    }
                    else
                    {

                    }
                }
            }
            catch (InvalidOperationException)
            {
                log.Warn("No Hotkey was found.");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ViewSceneMapping()
        {
            _fsm = new Form_SceneMapping(_selectedBridge) {Owner = Application.Current.MainWindow};
            _fsm.Show();
        }

        #region PLUGINS
        /// <summary>
        /// Load all the plugins in the plugin folder.
        /// </summary>
        /// <returns></returns>
        public bool LoadPlugins()
        {
            log.Info("Loading plugins...");
            var catalog = new AggregateCatalog();
            _plugins.Clear();

            try
            {
                if (!Directory.Exists("plugins"))
                {
                    log.Warn("Plugin directory not found. Creating it...");
                    DirectoryInfo df = Directory.CreateDirectory("plugins"); // Directory Information for the created mood folder
                }

                if (!Directory.Exists("lights"))
                {
                    log.Warn("Lights directory not found. Creating it...");
                    DirectoryInfo df = Directory.CreateDirectory("lights"); // Directory Information for the created light folder
                }

                catalog.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\plugins"));
                catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                CompositionContainer container = new CompositionContainer(catalog);
                container.ComposeExportedValue("Bridge", _selectedBridge);
                container.ComposeParts(this);
            }
            catch (DirectoryNotFoundException ex)
            {
                log.Error("A needed directory was not found.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Error("Authorisation denied while creating a directory.",ex);
            }
            catch (Exception ex)
            {
                log.Fatal("Errow while loading the plugins",ex);
            }

            if (_availablePlugins == null) return false;

            foreach (IWinHuePluginModule plugin in _availablePlugins)
            {
                log.Debug("Creating button for plugin" + plugin.pluginName);
                RibbonSplitButtonPlugin pluginbutton = new RibbonSplitButtonPlugin()
                {
                    Label = plugin.pluginName,
                    LargeImageSource = GDIManager.CreateImageSourceFromImage(plugin.pluginIcon),
                    Plugin = plugin,
                };

                pluginbutton.SetBinding(UIElement.IsEnabledProperty, new Binding("EnableControls"));

                pluginbutton.Click += Pluginbutton_Click;

                RibbonButton settingsbutton = new RibbonButton() { Label = GlobalStrings.Plugin_Settings };

                settingsbutton.Click += delegate
                {
                    pluginbutton.Plugin.Stop();
                    pluginbutton.IsChecked = false;
                    try
                    {
                        log.Debug("Showing settings for plugin " + plugin.pluginName);
                        pluginbutton.Plugin.ShowSettingsForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(GlobalStrings.Error, pluginbutton.Plugin.pluginName), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        GeneratePluginExceptionFile(ex, pluginbutton.Plugin);
                        log.Fatal($@"Error trying to show plugin {pluginbutton.Plugin.pluginName} settings form",ex);
                    }

                };

                pluginbutton.Items.Add(settingsbutton);
                _plugins.Add(pluginbutton);
                log.Info($"Loaded plugin {pluginbutton.Plugin.pluginName}.");
            }
            log.Info("Finished loading plugins.");
            return true;
        }

        private static void GeneratePluginExceptionFile(Exception excpt, IWinHuePluginModule plugin)
        {
            log.Debug($@"Exception in plugin {plugin.pluginName}", excpt);
        }

        private static void Pluginbutton_Click(object sender, RoutedEventArgs e)
        {
            RibbonSplitButtonPlugin button = (RibbonSplitButtonPlugin)sender;

            if (!button.IsChecked)
            {
                button.IsChecked = true;
                try
                {
                    log.Debug("Stating plugin " + button.Plugin.pluginName);
                    button.Plugin.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(GlobalStrings.Error, button.Plugin.pluginName), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    GeneratePluginExceptionFile(ex, button.Plugin);
                }
            }
            else
            {
                button.IsChecked = false;
                try
                {
                    log.Debug("Stopping plugin " + button.Plugin.pluginName);
                    button.Plugin.Stop();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(GlobalStrings.Error, button.Plugin.pluginName), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    GeneratePluginExceptionFile(ex, button.Plugin);
                }
            }
        }
        #endregion

        #endregion

        #region EVENTS

        #endregion

        #region COMMANDS

        //*************** Toolbar Commands ********************        
        public ICommand CheckForNewBulbCommand => new RelayCommand(param => CheckForNewBulb());
        public ICommand UpdateBridgeCommand => new RelayCommand(param => DoBridgeUpdate());
        public ICommand ChangeBridgeSettingsCommand => new RelayCommand(param => ChangeBridgeSettings());
        public ICommand RefreshViewCommand => new RelayCommand(param => RefreshView());
        public ICommand CreateGroupCommand => new RelayCommand(param => CreateGroup());
        public ICommand CreateSceneCommand => new RelayCommand(param => CreateScene());
        public ICommand CreateScheduleCommand => new RelayCommand(param => CreateSchedule());
        public ICommand CreateRuleCommand => new RelayCommand(param => CreateRule());
        public ICommand CreateSensorCommand => new RelayCommand(param => CreateSensor());
        public ICommand CreateAnimationCommand => new RelayCommand(param => CreateAnimation());
        public ICommand CreateHotKeyCommand => new RelayCommand(param => CreateHotKey());
        public ICommand AllOnCommand => new RelayCommand(param => AllOn());
        public ICommand AllOffCommand => new RelayCommand(param => AllOff());
        public ICommand ShowEventLogCommand => new RelayCommand(param => ShowEventLog());
        public ICommand SearchNewLightsCommand => new RelayCommand(param => CheckForNewBulb());
        public ICommand SearchNewSensorsCommand => new RelayCommand(param => SearchNewSensors());             

        //*************** Sliders Commands ********************
        public ICommand SliderHueChangedCommand => new RelayCommand(param => SliderChangeHue());
        public ICommand SliderBriChangedCommand => new RelayCommand(param => SliderChangeBri());
        public ICommand SliderCtChangedCommand => new RelayCommand(param => SliderChangeCT());
        public ICommand SliderSatChangedCommand => new RelayCommand(param => SliderChangeSat());
        public ICommand SliderXYChangedCommand => new RelayCommand(param => SliderChangeXY());
    
        //*************** App Menu Commands ******************
        public ICommand DoBridgePairingCommand => new RelayCommand(param => DoBridgePairing());

        //*************** Context Menu Commands *************
        public ICommand DeleteObjectCommand => new RelayCommand(param => DeleteObject());
        public ICommand RenameObjectCommand => new RelayCommand(param => RenameObject());
        public ICommand EditObjectCommand => new RelayCommand(param => EditObject());
        public ICommand IdentifyLongCommand => new RelayCommand(param => IdentifyLong());
        public ICommand IdentifyShortCommand => new RelayCommand(param => IdentifyShort());
        public ICommand ReplaceCurrentStateCommand => new RelayCommand(param => ReplaceCurrentState());

        //*************** ListView Commands ********************
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => DoubleClickObject());

        //*************** View Commands ************************

        public ICommand ViewSceneMappingCommand => new RelayCommand(param => ViewSceneMapping());

        #endregion
    }
}