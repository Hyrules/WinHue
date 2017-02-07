using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using HueLib2;
using System.Windows;
using System.Reflection;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Action = HueLib2.Action;
using Color = System.Windows.Media.Color;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Documents;
using WinHue3.Resources;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Forms;

namespace WinHue3
{
    public class MainFormView : View
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HueObject _selectedObject;
        private readonly DispatcherTimer _findlighttimer = new DispatcherTimer();
        private readonly DispatcherTimer _findsensortimer = new DispatcherTimer();
        private readonly DispatcherTimer _refreshStates = new DispatcherTimer();
        private ObservableCollection<HueObject> _listBridgeObjects;
        private readonly BackgroundWorker _bgwRefresher = new BackgroundWorker();
    //    private readonly BackgroundWorker _updatebs = new BackgroundWorker();
        private readonly CpuTempMonitor ctm = new CpuTempMonitor();
        private readonly RssFeedMonitor rfm = new RssFeedMonitor();

        public Form_EventLog _fel;
        private Form_SceneMapping _fsm;
        private Form_BulbsView _fbv;
        private Form_GroupView _fgv;
        private ushort? _transitiontime = null;
        private double _ttvalue = -1;
        private string _lastmessage = string.Empty;
        private List<HotKey> _listHotKeys;
        private readonly List<HotKeyHandle> _lhk;

        #region CTOR

        public MainFormView(Form_EventLog fel)
        {
            _lhk = new List<HotKeyHandle>();
            _fel = fel;
            _findlighttimer.Interval = new TimeSpan(0, 1, 0);
            _findlighttimer.Tick += _findlighttimer_Tick;
            _findsensortimer.Interval = new TimeSpan(0, 1, 0);
            _findsensortimer.Tick += _findsensortimer_Tick;
            _refreshStates.Interval = new TimeSpan(0, 0, 3);
            _refreshStates.Tick += _refreshStates_Tick;
            _bgwRefresher.DoWork += _bgwRefresher_DoWork;
            _listHotKeys = WinHueSettings.settings.listHotKeys;
            Communication.Timeout = WinHueSettings.settings.Timeout;

         //   _updatebs.DoWork += _updatebs_DoWork;

            //_refreshStates.Start();
            Cursor_Tools.ShowWaitCursor();

            // Load from the settings.
            log.Info("Loading bridge from saved settings...");
            foreach (KeyValuePair<string, BridgeSaveSettings> kvp in WinHueSettings.settings.BridgeInfo)
            {
                log.Info($"Bridge {kvp.Value.ip} added.");
                BridgeStore.ListBridges.Add(new Bridge(IPAddress.Parse(kvp.Value.ip), kvp.Key, kvp.Value.apiversion, kvp.Value.swversion, kvp.Value.name, kvp.Value.apikey));
                OnPropertyChanged("MultiBridgeCB");
            }
            
            LoadBridge();
            LoadHotkeys();
            
        }

        private void CheckForUpdate()
        {
            if (ListBridges.All(x => x.ApiKey == string.Empty && x.IsDefault == false)) return;

            foreach (Bridge br in ListBridges)
            {

                HelperResult hr = HueObjectHelper.GetBridgeSettings(br);
                if (hr.Success)
                {
                    BridgeSettings brs = (BridgeSettings)hr.Hrobject;
                    WinHueSettings.settings.BridgeInfo[br.Mac].apiversion = brs.apiversion;
                    br.ApiVersion = brs.apiversion;
                    WinHueSettings.settings.BridgeInfo[br.Mac].name = brs.name;
                    br.Name = brs.name;
                    WinHueSettings.settings.BridgeInfo[br.Mac].swversion = brs.swversion;
                    br.SwVersion = brs.swversion;
                    WinHueSettings.Save();
                }
                else
                {
                    log.Error($"Unable to update {br.Name} winhue settings.");
                }
            }
        }

        public void Initialize(Form_EventLog fel)
        {

        }

        private void LoadHotkeys()
        {

            if (_lhk.Count > 0)
            {
                while (_lhk.Count != 0)
                {
                    _lhk[0].Unregister();
                    _lhk.Remove(_lhk[0]);                    
                }
            }

            foreach (HotKey h in _listHotKeys)
            {
                HotKeyHandle hkh = new HotKeyHandle(h, HandleHotkey);
                if(hkh.Register())
                    _lhk.Add(hkh);
                else
                    log.Error($"Cannot register hotkey {h.Name} key seems to be already taken by another process.");
            }


        }

        public Visibility CanSeeIdentify
        {
            get
            {
                if(_selectedObject == null) return Visibility.Collapsed;
                if(_selectedObject is Light || _selectedObject is Group) return Visibility.Visible;
                return Visibility.Collapsed;

            }
        }

        public Visibility CanSeeSensitivity
        {
            get
            {
                if(_selectedObject == null) return Visibility.Collapsed;
                if (_selectedObject.GetType() == typeof(Sensor))
                {
                    if(((Sensor) _selectedObject).type == "ZLLPresence") return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public bool CanSetCpuTempSettings => !ctm.IsRunning;

        private void _bgwRefresher_DoWork(object sender, DoWorkEventArgs e)
        {

            if (_listBridgeObjects == null) return;
            //log.Debug("Automatic refresh in progress");

            List<HueObject> ll = _listBridgeObjects.Where(x => x.GetType() == typeof(Light)).ToList();
            HueObject _selectedHueObject = SelectedObject;
            foreach (HueObject obj in ll)
            {

                RefreshObject(obj);

            }
            SelectedObject = _selectedHueObject;

        }

        private void _refreshStates_Tick(object sender, EventArgs e)
        {
            if (!_bgwRefresher.IsBusy)
                _bgwRefresher.RunWorkerAsync();
        }

        #endregion

        #region PROPERTIES

        public bool IsValidScene
        {
            get
            {
                if (!(_selectedObject is Scene)) return false;
                return ((Scene)_selectedObject).version != 1;
            }
        }

        public Visibility MultiBridgeCB => BridgeStore.ListBridges.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

        public ObservableCollection<Bridge> ListBridges => new ObservableCollection<Bridge>(BridgeStore.ListBridges.Where(x => x.ApiKey != string.Empty));

        public bool EnableSearchLights
        {
            get
            {
                if (BridgeStore.SelectedBridge == null) return false;
                bool result = false;
                result = !_findlighttimer.IsEnabled;
                return EnableControls && result;
            }
        }

        public bool EnableSearchSensors
        {

            get
            {
                if (BridgeStore.SelectedBridge == null) return false;
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
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light) _selectedObject).state.hue == null ? 0 : Convert.ToDouble(((Light) _selectedObject).state.hue);
                if(_selectedObject is Group)
                    return ((Group) _selectedObject).action.hue == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.hue);
                return 0;
            }
           set
           {
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
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
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light)_selectedObject).state.bri == null ? 0 : Convert.ToDouble(((Light)_selectedObject).state.bri);
                if (_selectedObject is Group)
                    return ((Group)_selectedObject).action.bri == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.bri);
                return 0;
            }
            set
            {
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
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
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light)_selectedObject).state.ct == null ? 0 : Convert.ToDouble(((Light)_selectedObject).state.ct);
                if (_selectedObject is Group)
                    return ((Group)_selectedObject).action.ct == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.ct);
                return 0;
            }
            set
            {
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
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
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return ((Light)_selectedObject).state.sat == null ? 0 : Convert.ToDouble(((Light)_selectedObject).state.sat);
                if (_selectedObject is Group)
                    return ((Group)_selectedObject).action.sat == null ? 0 : Convert.ToDouble(((Group)_selectedObject).action.sat);
                return 0;
            }
            set
            {
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
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
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return Convert.ToDouble(((Light)_selectedObject).state.xy?.x);
                if (_selectedObject is Group)
                    return Convert.ToDouble(((Group)_selectedObject).action.xy?.x);
                return 0;
            }
            set
            {
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
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
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return 0;
                if (_selectedObject is Light)
                    return Convert.ToDouble(((Light)_selectedObject).state.xy?.y);
                if (_selectedObject is Group)
                    return Convert.ToDouble(((Group)_selectedObject).action.xy?.y);
                return 0;
            }
            set
            {
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
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
                if (_selectedObject == null || BridgeStore.SelectedBridge == null) return -1;
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
            get { return BridgeStore.SelectedBridge; }
            set
            {
                BridgeStore.SelectedBridge = value;
                OnPropertyChanged();
                if (BridgeStore.SelectedBridge == null) return;
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

                if (value != null)
                {
                    MethodInfo mi = typeof(HueObjectHelper).GetMethod("GetObject");
                    MethodInfo generic = mi.MakeGenericMethod(value.GetType());
                    HelperResult hr = (HelperResult) generic.Invoke(BridgeStore.SelectedBridge, new object[] {BridgeStore.SelectedBridge, value.Id});
                    if (!hr.Success)
                    {
                        log.Error(hr.Hrobject);
                        return;
                    }
                    _selectedObject = (HueObject)hr.Hrobject;
                }
                
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
                OnPropertyChanged("CanSeeSensitivity");
                OnPropertyChanged("CanSeeIdentify");
            }
        }

        public ObservableCollection<HueObject> ListBridgeObjects => _listBridgeObjects;

        public bool EnableControls => BridgeStore.SelectedBridge != null;

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
                return _selectedObject is Light || _selectedObject is Group || _selectedObject is Scene;
            }
        }

        public Visibility UpdateAvailable
        {
            get
            {
                if (BridgeStore.SelectedBridge == null) return Visibility.Collapsed;
                CommandResult bresult = BridgeStore.SelectedBridge.GetSwUpdate();
                SwUpdate update;
                if (bresult.Success)
                {
                    update = (SwUpdate) bresult.resultobject;
                }
                else
                {
                    update = new SwUpdate() {updatestate = 0};
                }
                return update.updatestate == 2 ? Visibility.Visible : Visibility.Collapsed;
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

        public string StatusBarMessage => BridgeStore.SelectedBridge == null ? string.Empty : _lastmessage;

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
                HelperResult hr = HueObjectHelper.IsAuthorized(br);
                if (hr.Success)
                {
                    if (!(bool)hr.Hrobject) br.ApiKey = string.Empty;
                }
                br.IsDefault = br.Mac == WinHueSettings.settings.DefaultBridge;
            }
            return bridge;
        }

        private void LoadBridge()
        {
            log.Info("Loading bridge...");
            BridgeStore.ListBridges = AssociateBridgeWithApiKey(BridgeStore.ListBridges);
            bool result = false;
            // if None of the bridge are paired or if there is no default bridge
            if (BridgeStore.ListBridges.All(x => x.ApiKey == string.Empty) || (WinHueSettings.settings.DefaultBridge == string.Empty))
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
                Bridge temp = BridgeStore.ListBridges.FirstOrDefault(x => x.Mac == WinHueSettings.settings.DefaultBridge);
                log.Info($"Selecting default bridge {temp.IpAddress}");
                if (temp.Equals(default(Bridge)))
                {
                    log.Warn("The default bridge not found. Using the first bridge in the list if any.");
                    if (BridgeStore.ListBridges.Count == 0)
                    {
                        log.Error("No other bridge to chose. Please check that your default bridge is on.");
                    }
                    else
                    {
                        log.Error("Selecting the first bridge in the list as default bridge.");
                        temp = BridgeStore.ListBridges.FirstOrDefault();
                    }
                }
                            
                temp.OnMessageAdded += MessageAdded;
                temp.BridgeNotResponding += Temp_BridgeNotResponding;

                SelectedBridge = temp;

                if (SelectedBridge.ApiVersion != "1.16.0")
                {
                    MessageBox.Show(GlobalStrings.Warning_Bridge_Not_Updated, GlobalStrings.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                //LoadPlugins();
                Cursor_Tools.ShowNormalCursor();
                CheckForUpdate();
            }
            
        }

        private void Temp_BridgeNotResponding(object sender, EventArgs e)
        {
            //BridgeStore.SelectedBridge = null;
            MessageBox.Show(GlobalStrings.Error_Bridge_Not_Responding, GlobalStrings.Error, MessageBoxButton.OK,MessageBoxImage.Error);
            if (e is BridgeNotRespondingEventArgs)
            {
                log.Error($"{sender}");
                log.Error(Serializer.SerializeToJson(((BridgeNotRespondingEventArgs) e).ex?.ex?.ToString()));             
                /*log.Error(((BridgeNotRespondingEventArgs) e).ex.ToString());
                log.Error(((BridgeNotRespondingEventArgs)e).ex.ex.ToString());
                log.Error(((BridgeNotRespondingEventArgs) e).ex.ex.InnerException?.ToString());*/
            }
            log.Error($"{sender} : {e.ToString()}");
            //SelectedBridge = null;
            Cursor_Tools.ShowNormalCursor();
            ctm.Stop();
            rfm.Stop();
            //DoBridgePairing();
        }


        void MessageAdded(object sender, EventArgs e)
        {
            if (BridgeStore.SelectedBridge.lastMessages != null)
            {
                log.Info(BridgeStore.SelectedBridge.lastMessages);
                if(BridgeStore.SelectedBridge.lastMessages.Count > 0)
                    _lastmessage = BridgeStore.SelectedBridge.lastMessages[BridgeStore.SelectedBridge.lastMessages.Count - 1].ToString();
            }

            OnPropertyChanged("StatusBarMessage");

        }

        private void DoBridgeUpdate()
        {
            if (BridgeStore.SelectedBridge == null) return;
            if (MessageBox.Show(GlobalStrings.Update_Confirmation, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            log.Info("Updating bridge to the latest firmware.");
            CommandResult bresult = BridgeStore.SelectedBridge.DoSwUpdate();
            if (!bresult.Success)
            {
                log.Error("An error occured while trying to start a bridge update. Please try again later.");
                return;
            }
            else
            {
                Form_Wait fw = new Form_Wait();
                fw.ShowWait(GlobalStrings.ApplyingUpdate,new TimeSpan(0,0,0,180), Application.Current.MainWindow );
                CommandResult bsettings = BridgeStore.SelectedBridge.GetBridgeSettings();
                if (bsettings.Success)
                {
                    BridgeSettings brs = (BridgeSettings) bsettings.resultobject;

                    switch (brs.swupdate.updatestate)
                    {
                        case 0:
                            log.Info("Bridge updated succesfully");
                            BridgeStore.SelectedBridge.SetNotify();
                            break;
                        case 1:
                            log.Info("Bridge update incoming. Please wait for it to be ready.");
                            break;
                        case 2:
                            log.Info("Bridge update still available or a new update is available.");
                            break;
                        case null:
                            log.Error("Bride update state was null.");
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
            if (BridgeStore.SelectedBridge == null) return;
            CommandResult bresult = BridgeStore.SelectedBridge.FindNewObjects<Light>();
            if (bresult.Success)
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
            HelperResult hr = HueObjectHelper.GetBridgeNewLights(BridgeStore.SelectedBridge);
            if (!hr.Success) return;
            List<HueObject> newlights = (List<HueObject>) hr.Hrobject;
            log.Info($"Found {newlights.Count} new lights.");
            _listBridgeObjects.AddRange(newlights);
            OnPropertyChanged("ListBridgeObjects");
            OnPropertyChanged("EnableSearchLights");
        }

        private void _findsensortimer_Tick(object sender, EventArgs e)
        {
            _findsensortimer.Stop();
            log.Info("Done searching for new sensors.");
            HelperResult hr = HueObjectHelper.GetBridgeNewSensors(BridgeStore.SelectedBridge);
            if (!hr.Success) return;
            List<HueObject> newsensors = (List<HueObject>) hr.Hrobject;
            log.Info($"Found {newsensors.Count} new sensors.");
            _listBridgeObjects.AddRange(newsensors);
            OnPropertyChanged("ListBridgeObjects");
            OnPropertyChanged("EnableSearchSensors");
        }

        private void SliderChangeHue()
        {
            if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
            ExecuteGenericMethod<CommandResult>(BridgeStore.SelectedBridge, "SetState",new object[] {new CommonProperties() {hue = (ushort) SliderHue, transitiontime = _transitiontime},_selectedObject.Id});

        }

        private T ExecuteGenericMethod<T>(object objectmethod,string methodname,object[] paramsarray) where T : new()
        {
            MethodInfo mi = objectmethod.GetType().GetMethod(methodname);
            MethodInfo generic = mi.MakeGenericMethod(_selectedObject.GetType());
            object result = generic.Invoke(BridgeStore.SelectedBridge, paramsarray);
            return (T) result;

        }

        private void SliderChangeBri()
        {
            if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
            ExecuteGenericMethod<CommandResult>(BridgeStore.SelectedBridge, "SetState", new object[] { new CommonProperties() { bri = (byte)SliderBri, transitiontime = _transitiontime }, _selectedObject.Id});
        }

        private void SliderChangeCT()
        {
            if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
            ExecuteGenericMethod<CommandResult>(BridgeStore.SelectedBridge, "SetState", new object[] { new CommonProperties() { ct = (ushort)SliderCT, transitiontime = _transitiontime }, _selectedObject.Id });
        }

        private void SliderChangeSat()
        {
            if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
            ExecuteGenericMethod<CommandResult>(BridgeStore.SelectedBridge, "SetState", new object[] { new CommonProperties() { sat = (byte)SliderSat, transitiontime = _transitiontime }, _selectedObject.Id });
        }

        private void SliderChangeXY()
        {
            if (_selectedObject == null || BridgeStore.SelectedBridge == null) return;
            ExecuteGenericMethod<CommandResult>(BridgeStore.SelectedBridge, "SetState", new object[] { new CommonProperties() { xy = new XY() { x = (decimal)SliderX, y = (decimal)SliderY }, transitiontime = _transitiontime }, _selectedObject.Id });

        }

        private void DoBridgePairing()
        {
            Form_BridgeDetectionPairing dp = new Form_BridgeDetectionPairing() {Owner = Application.Current.MainWindow };
            if (dp.ShowDialog() != true) return;
            OnPropertyChanged("ListBridges");
            OnPropertyChanged("SelectedBridge");
            LoadBridge();
        }

        #region CONTEXT MENU COMMANDS
        private void DeleteObject()
        {
            if (_selectedObject == null) return;
            if (
                MessageBox.Show(string.Format(GlobalStrings.Confirm_Delete_Object, _selectedObject.GetName()),
                    GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            MethodInfo method = typeof(Bridge).GetMethod("RemoveObject");
            MethodInfo generic = method.MakeGenericMethod(_selectedObject.GetType());
            CommandResult bresult = (CommandResult) generic.Invoke(BridgeStore.SelectedBridge, new object[]{ _selectedObject.Id });
   
            log.Debug("Result : " + bresult.resultobject);
            if (bresult.Success)
            {
                int index =
                    _listBridgeObjects.FindIndex(
                        x => x.Id == _selectedObject.Id && x.GetType() == _selectedObject.GetType());
                if (index == -1) return;
                _listBridgeObjects.RemoveAt(index);
                OnPropertyChanged("ListBridgeObjects");
                SelectedObject = null;
                log.Info($"Object ID : {index} removed.");
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
        }

        private void RenameObject()
        {
            Form_RenameObject fro = new Form_RenameObject(BridgeStore.SelectedBridge,_selectedObject) {Owner = Application.Current.MainWindow};
            if (fro.ShowDialog() != true) return;
            _selectedObject.SetName(fro.GetNewName());
            int index = _listBridgeObjects.FindIndex(x => x.Id == _selectedObject.Id && x.GetType() == _selectedObject.GetType());
            if (index == -1) return;
            _listBridgeObjects[index].SetName(fro.GetNewName());
            log.Info($"Renamed object ID : {index} renamed.");

        }

        private void RefreshObject(HueObject obj, bool logging = false)
        {
            int index = _listBridgeObjects.FindIndex(x => x.Id == obj.Id && x.GetType() == obj.GetType());
            if (index == -1) return;
       
            MethodInfo mi = typeof(HueObjectHelper).GetMethod("GetObject");
            MethodInfo generic = mi.MakeGenericMethod(obj.GetType());
            HelperResult hr = (HelperResult)generic.Invoke(BridgeStore.SelectedBridge, new object[] {BridgeStore.SelectedBridge,obj.Id});

            if (!hr.Success) return;
            HueObject newobj = (HueObject)hr.Hrobject;
            _listBridgeObjects[index].Image = newobj.Image;
            PropertyInfo[] pi = newobj.GetType().GetProperties();
            foreach (PropertyInfo p in pi)
            {
                if (_listBridgeObjects[index].HasProperty(p.Name))
                    p.SetValue(_listBridgeObjects[index], _listBridgeObjects[index].GetType().GetProperty(p.Name).GetValue(newobj));
            }
        }

        private void EditObject()
        {
            if (_selectedObject == null) return;
            if (_selectedObject is Light) return;
            log.Debug("Editing object : " + _selectedObject);

            if(_selectedObject is Group)
            {
                Form_GroupCreator fgc = new Form_GroupCreator(_selectedObject) { Owner = Application.Current.MainWindow };
                if(fgc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Schedule)
            {
                Form_ScheduleCreator fsc = new Form_ScheduleCreator(_selectedObject) { Owner = Application.Current.MainWindow };
                if (fsc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Sensor)
            {
                Sensor obj = (Sensor)_selectedObject;
                switch (obj.modelid)
                {
                    case "PHDL00":
                        CommandResult cr= BridgeStore.SelectedBridge.GetObject<Sensor>(obj.Id);
                        if (cr.Success)
                        {
                            Sensor daylight = (Sensor) cr.resultobject;
                            daylight.Id = obj.Id;
                            Form_Daylight dl = new Form_Daylight(daylight) { Owner = Application.Current.MainWindow };
                            if (dl.ShowDialog() == true)
                            {
                                RefreshObject(_selectedObject);
                            }

                        }

                        break;
                    case "ZGPSWITCH":
                        Form_HueTapConfig htc = new Form_HueTapConfig(obj.Id)
                        {
                            Owner = Application.Current.MainWindow
                        };
                        if(htc.ShowDialog() == true)
                        {
                            RefreshObject(_selectedObject);
                        }
                        break;
                    default:
                        CommandResult crs = BridgeStore.SelectedBridge.GetObject<Sensor>(obj.Id);
                        if (crs.Success)
                        {
                            Form_SensorCreator fsc = new Form_SensorCreator((Sensor)crs.resultobject)
                            {
                                Owner = Application.Current.MainWindow
                            };
                            if (fsc.ShowDialog() == true)
                            {
                                RefreshObject(_selectedObject);
                            }

                        }
                        break;
                }
            }
            else if (_selectedObject is Rule)
            {
                Form_RulesCreator2 frc = new Form_RulesCreator2(BridgeStore.SelectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
                if (frc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Scene)
            {
                Form_SceneCreator fscc = new Form_SceneCreator(BridgeStore.SelectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
                if (fscc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Resourcelink)
            {
                Form_ResourceLinksCreator frlc = new Form_ResourceLinksCreator((Resourcelink)_selectedObject) { Owner = Application.Current.MainWindow};

                if (frlc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }

            }
        }


        #endregion

        #region HOME TOOLBAR 

        private void ChangeBridgeSettings()
        {
            Form_BridgeSettings frs = new Form_BridgeSettings() { Owner = Application.Current.MainWindow };
            if (frs.ShowDialog() == true)
            {
                CommandResult bresult = BridgeStore.SelectedBridge.GetBridgeSettings();
                if (bresult.Success)
                {
                    string newname = ((BridgeSettings)(bresult.resultobject)).name;
                    if (BridgeStore.SelectedBridge.Name != newname)
                    {
                        BridgeStore.SelectedBridge.Name = newname;
                        BridgeStore.ListBridges[BridgeStore.ListBridges.IndexOf(BridgeStore.SelectedBridge)].Name = newname;
                        Bridge selbr = BridgeStore.SelectedBridge;
                        SelectedBridge = null;
                        SelectedBridge = selbr;
                        OnPropertyChanged("ListBridges");
                    }
                    RefreshView();
                    CheckForUpdate();
                }
            }

        }

        private void RefreshView()
        {
            if(_listBridgeObjects != null) _listBridgeObjects.Clear();
            OnPropertyChanged("ListBridgeObjects");
            log.Info($"Getting list of objects from bridge at {BridgeStore.SelectedBridge.IpAddress}.");
            HelperResult hr = HueObjectHelper.GetBridgeDataStore(BridgeStore.SelectedBridge);
            if (hr.Success)
            {
                _listBridgeObjects = new ObservableCollection<HueObject>((List<HueObject>)hr.Hrobject);
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
            else
            {
                log.Error(hr.Hrobject);
            }
            

        }



        private void CreateGroup()
        {
            Form_GroupCreator fgc = new Form_GroupCreator { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the Group creator window for bridge {BridgeStore.SelectedBridge.IpAddress} ");
            if (fgc.ShowDialog() != true) return;
            if (fgc.GetCreatedOrModifiedID() == null) return;

            HelperResult hr = HueObjectHelper.GetObject<Group>(BridgeStore.SelectedBridge, fgc.GetCreatedOrModifiedID());
            if (hr.Success)
            {
                if(_listBridgeObjects == null) _listBridgeObjects = new ObservableCollection<HueObject>();
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
        }

        private void CreateScene()
        {
            Form_SceneCreator fsc = new Form_SceneCreator(BridgeStore.SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the scene creator window for bridge {BridgeStore.SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created scene ID {fsc.GetCreatedOrModifiedID()} from bridge {BridgeStore.SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Scene>(BridgeStore.SelectedBridge, fsc.GetCreatedOrModifiedID());
            if (hr.Success)
            {
                
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
        }

        private void CreateSchedule()
        {
            Form_ScheduleCreator fscc = new Form_ScheduleCreator(_selectedObject) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the schedule creator window passing bridge {BridgeStore.SelectedBridge.IpAddress} ");
            if (fscc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created schedule ID {fscc.GetCreatedOrModifiedID()} from bridge {BridgeStore.SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Schedule>(BridgeStore.SelectedBridge, fscc.GetCreatedOrModifiedID());
            if (hr.Success)
            {
                
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }

        }

        private void CreateRule()
        {
            Form_RulesCreator2 frc = new Form_RulesCreator2(BridgeStore.SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the rule creator window passing bridge {BridgeStore.SelectedBridge.IpAddress} ");
            if (frc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created rule ID {frc.GetCreatedOrModifiedId()} from bridge {BridgeStore.SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Rule>(BridgeStore.SelectedBridge, frc.GetCreatedOrModifiedId());
            if (hr.Success)
            {
                
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
            
        }

        private void CreateSensor()
        {
            Form_SensorCreator fsc = new Form_SensorCreator() { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the sensor creator window passing bridge {BridgeStore.SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created sensor ID {fsc.GetCreatedOrModifiedID()} from bridge {BridgeStore.SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Sensor>(BridgeStore.SelectedBridge, fsc.GetCreatedOrModifiedID());
            if (hr.Success)
            {
                
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
        }

        private void CreateAnimation()
        {
            Form_AnimationCreator fac = new Form_AnimationCreator(SelectedBridge) {Owner = Application.Current.MainWindow};
            log.Debug($@"Opening the animation creator windows passing bridge {BridgeStore.SelectedBridge.IpAddress} ");
            if (fac.ShowDialog() != true) return;
        }

        private void AllOn()
        {
            if (BridgeStore.SelectedBridge == null) return;
            log.Info("Sending all on command to bridge" + BridgeStore.SelectedBridge.IpAddress);
            Action act = new Action() {@on = true};
            if (WinHueSettings.settings.AllOnTT != null)
                act.transitiontime = WinHueSettings.settings.AllOnTT;
            CommandResult bresult = BridgeStore.SelectedBridge.SetState<Group>(act, "0");
            if (bresult.Success)
            {
                log.Debug("Refreshing the main view.");
                RefreshView();
            }
        }

        private void AllOff()
        {
            if (BridgeStore.SelectedBridge == null) return;
            log.Info("Sending all off command to bridge" + BridgeStore.SelectedBridge.IpAddress);
            Action act = new Action() { @on = false };
            if (WinHueSettings.settings.AllOnTT != null)
                act.transitiontime = WinHueSettings.settings.AllOnTT;
            CommandResult bresult = BridgeStore.SelectedBridge.SetState<Group>(act, "0");
            if (bresult.Success)
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
            if (BridgeStore.SelectedBridge == null) return;
            CommandResult bresult = BridgeStore.SelectedBridge.FindNewObjects<Sensor>();
            if(bresult.Success)
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
                HelperResult hr = HueObjectHelper.ToggleObjectOnOffState(BridgeStore.SelectedBridge, _selectedObject);
                if (hr.Success)
                {
                    ImageSource newimg = (ImageSource)hr.Hrobject;
                    if (newimg == null) return;
                    _selectedObject.Image = newimg;
                    int index = _listBridgeObjects.FindIndex(x => x.Id == _selectedObject.Id && x.GetType() == _selectedObject.GetType());
                    if (index == -1) return;
                    if (_selectedObject.HasProperty("state"))
                    {
                        ((Light)_selectedObject).state.on = !((Light)_selectedObject).state.on;
                        ((Light)_listBridgeObjects[index]).state.on = !((Light)_listBridgeObjects[index]).state.on;
                    }
                    else
                    {
                        ((Group)_selectedObject).action.on = !((Group)_selectedObject).action.on;
                        ((Group)_listBridgeObjects[index]).action.on = !((Group)_listBridgeObjects[index]).action.on;
                    }

                    _listBridgeObjects[index].Image = newimg;
                    OnPropertyChanged("SelectedObject");
                }
            }
            else
            {
                log.Info($"Activating scene : {_selectedObject.Id}");
                BridgeStore.SelectedBridge.ActivateScene(_selectedObject.Id);

            }
            
        }
          
        private void IdentifyLong()
        {
            if (_selectedObject == null) return;
            if (!(_selectedObject is Light) && !(_selectedObject is Group)) return;

            MethodInfo mi = typeof(Bridge).GetMethod("SetState");
            MethodInfo generic = mi.MakeGenericMethod(_selectedObject.GetType());
            log.Info($@"Sending the long Identify command to object ID : {_selectedObject.Id}");
            CommandResult hr = (CommandResult) generic.Invoke(BridgeStore.SelectedBridge, new object[] {new CommonProperties() {alert = "lselect"},_selectedObject.Id});
        }

        private void IdentifyShort()
        {
            if (_selectedObject == null) return;
            if (!(_selectedObject is Light) && !(_selectedObject is Group)) return;

            MethodInfo mi = typeof(Bridge).GetMethod("SetState");
            MethodInfo generic = mi.MakeGenericMethod(_selectedObject.GetType());
            log.Info($@"Sending the long Identify command to object ID : {_selectedObject.Id}");
            CommandResult hr = (CommandResult)generic.Invoke(BridgeStore.SelectedBridge, new object[] { new CommonProperties() { alert = "select" }, _selectedObject.Id });
        }


        public void ReplaceCurrentState()
        {
            if (_selectedObject == null) return;
            if (!(_selectedObject is Scene)) return;
            if (MessageBox.Show(GlobalStrings.Scene_Replace_Current_States, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            log.Info($@"Replacing scene {((Scene)_selectedObject).name} lights state with current one.");
            BridgeStore.SelectedBridge.StoreCurrentLightState(_selectedObject.Id);
       
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
            Form_HotKeyCreator fhkc = new Form_HotKeyCreator() {Owner = Application.Current.MainWindow};
            fhkc.ShowDialog();
            _listHotKeys = fhkc.GetHotKeys();
            LoadHotkeys();
        }

        #endregion

        public void HandleHotkey(HotKeyHandle e)
        {

            ModifierKeys m = e.KeyModifiers;
            Key k = e.Key;
            try
            {
                
                HotKey h = _listHotKeys.First(x => x.Modifier == m && x.Key == k);
                if (!(h.objecType == null && _selectedObject == null))
                {
                    Type objtype = h.objecType == null ? _selectedObject.GetType() : h.objecType;

                    if (objtype == typeof(Scene))
                    {
                        BridgeStore.SelectedBridge.ActivateScene(h.id);
                    }
                    else if (objtype == typeof(Group) || objtype == typeof(Light))
                    {

                        List<object> listparams = new List<object>();
                        listparams.Add(Convert.ChangeType(h.properties, typeof(CommonProperties)));
                        listparams.Add(h.objecType != null ? h.id : _selectedObject.Id);
                        ExecuteGenericMethod<CommandResult>(BridgeStore.SelectedBridge, "SetState", listparams.ToArray());
                    }
                    else
                    {
                        log.Warn($"Type of object {objtype} not supported");
                    }
                }
                else
                {
                    log.Warn("You must select an object in WinHue in order to apply this hotkey.");
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
            _fsm = new Form_SceneMapping() {Owner = Application.Current.MainWindow};
            _fsm.Show();
        }

        private void ViewBulbs()
        {
            _fbv = new Form_BulbsView() {Owner = Application.Current.MainWindow};
            _fbv.Show();
        }

        private void ViewGroups()
        {
            _fgv = new Form_GroupView() {Owner = Application.Current.MainWindow};
            _fgv.Show();
        }

        private void RunCpuTempMon()
        {
            if(!ctm.IsRunning)
                ctm.Start();
            else
                ctm.Stop();
            OnPropertyChanged("CanSetCpuTempSettings");
        }

        private void RunRssFeedMon()
        {
            if (!rfm.IsRunning)
                rfm.Start();
            else
                rfm.Stop();
        }

        private void CpuTempMonSettings()
        {
            ctm.ShowSettingsForm();
        }

        private void RssFeedMonSettings()
        {
            rfm.ShowSettingsForm();
        }

        private void SensitivityHigh()
        {
            Sensor sensor = (Sensor)_selectedObject;
            ((HueMotionSensorConfig)sensor.config).sensitivity = 0;
            // sensor.Id = _selectedObject.Id;
            sensor.type = ((Sensor)_selectedObject).type;
            SelectedBridge.ModifyObject<Sensor>(sensor, sensor.Id);
        }

        private void SensitivityMedium()
        {
            Sensor sensor = (Sensor)_selectedObject;
            ((HueMotionSensorConfig)sensor.config).sensitivity = 0;
            // sensor.Id = _selectedObject.Id;
            sensor.type = ((Sensor)_selectedObject).type;
            SelectedBridge.ModifyObject<Sensor>(sensor, sensor.Id);
        }

        private void SensitivityLow()
        {
            Sensor sensor = (Sensor) _selectedObject;
            ((HueMotionSensorConfig)sensor.config).sensitivity = 0;
           // sensor.Id = _selectedObject.Id;
            sensor.type = ((Sensor) _selectedObject).type;
            SelectedBridge.ModifyObject<Sensor>(sensor, sensor.Id);
        }


        #region PLUGINS

        /// <summary>
        /// Load all the plugins in the plugin folder.
        /// </summary>
        /// <returns></returns>

        /*
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
            container.ComposeExportedValue("Bridge", BridgeStore.SelectedBridge);
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
    */
        /*
        private static void GeneratePluginExceptionFile(Exception excpt, IWinHuePluginModule plugin)
        {
            log.Debug($@"Exception in plugin {plugin.pluginName}", excpt);
        }
        */
        /*
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
        }*/

        private void Clapper()
        {
            Clapper clapper = new Clapper();
            clapper.Start();
        }

        private void CreateResourceLink()
        {
            Form_ResourceLinksCreator frc = new Form_ResourceLinksCreator() { Owner = Application.Current.MainWindow };
            if (frc.ShowDialog() == true)
            {
                
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
        public ICommand CreateResourceLinkCommand => new RelayCommand(param => CreateResourceLink());
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
        public ICommand SensitivityHighCommand => new RelayCommand(param => SensitivityHigh());
        public ICommand SensitivityMediumCommand => new RelayCommand(param => SensitivityMedium());
        public ICommand SensitivityLowCommand => new RelayCommand(param => SensitivityLow());

        //*************** ListView Commands ********************
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => DoubleClickObject());

        //*************** View Commands ************************

        public ICommand ViewSceneMappingCommand => new RelayCommand(param => ViewSceneMapping());
        public ICommand ViewBulbsCommand => new RelayCommand(param => ViewBulbs());
        public ICommand ViewGroupsCommand => new RelayCommand(param => ViewGroups());
        
        //*************** Toolbar ******************************

        public ICommand CpuTempMonCommand => new RelayCommand(param => RunCpuTempMon());
        public ICommand RssFeedMonCommand => new RelayCommand(param => RunRssFeedMon());
        public ICommand CpuTempMonSettingsCommand => new RelayCommand(param => CpuTempMonSettings());
        public ICommand RssFeedMonSettingsCommand => new RelayCommand(param => RssFeedMonSettings());
        public ICommand ClapperCommand => new RelayCommand(param => Clapper());

        #endregion
    }
}