using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using WinHue3.Functions.EventViewer;
using WinHue3.Functions.PropertyGrid;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Resources;
using WinHue3.Utils;
using System.Linq;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Functions.Grouping;


namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private Form_EventLog _eventlogform;


        private ObservableCollection<Bridge> _listBridges;

        private ObservableCollection<IGroup> _listgroups;

        private IHueObject _selectedObject;
        private Bridge _selectedBridge;
        private ushort? _sliderTT;
        private bool _visibleTabs = true;
        private IBaseProperties _newstate;
        private int _sensorStatus;
        private bool _sensorFlag;


        [RefreshProperties(RefreshProperties.All)]
        public Bridge SelectedBridge
        {
            get => _selectedBridge;
            set => SetProperty(ref _selectedBridge,value);
        }

        public object IsMasterDebugger => System.Diagnostics.Debugger.IsAttached;

        public Form_PropertyGrid PropertyGrid => _propertyGrid;

        private bool CanRunTempPlugin => UacHelper.IsProcessElevated();

        public bool AppUpdateAvailable => UpdateManager.UpdateAvailable;

        public ObservableCollection<IHueObject> ListBridgeObjects
        {
            get => _listBridgeObjects;
            set { SetProperty(ref _listBridgeObjects, value); RaisePropertyChanged("MultiBridgeCB");}
        }

        public int SensorStatus
        {
            get => _sensorStatus;
            set => SetProperty(ref _sensorStatus, value);
        }

        public bool SensorFlag
        {
            get => _sensorFlag;
            set => SetProperty(ref _sensorFlag, value);
        }

        public string Title => $"WinHue 3 - {Version}";

        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string Lastmessage
        {
            get => _lastmessage;
            set => SetProperty(ref _lastmessage,value);
        }

        public ObservableCollection<Bridge> ListBridges
        {
            get => _listBridges;
            set { SetProperty(ref _listBridges,value); RaisePropertyChanged("MultiBridgeCB");}
        }

        public IHueObject SelectedHueObject
        {
            get => _selectedObject;
            set => SetProperty(ref _selectedObject, value);
        }

        public Form_EventLog Eventlogform
        {
            get => _eventlogform;
            set => SetProperty(ref _eventlogform,value);
        }

        public string TransitionTimeTooltip
        {
            get
            {
                if (SliderTt >= 0)
                {
                    int time = (int)(SliderTt * 100);
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

        public ushort? SliderTt
        {
            get => _sliderTT;
            set
            {
                SetProperty(ref _sliderTT, value); 
                RaisePropertyChanged("TransitionTimeTooltip");
            }
        }

        public bool MultiBridgeCB => ListBridges?.Count > 1;

        public Visibility UpdateAvailable
        {
            get
            {
                BridgeSettings cr = SelectedBridge?.GetBridgeSettings();
                if (cr == null) return Visibility.Collapsed;
                return cr.swupdate.updatestate == 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool VisibleTabs { get => _visibleTabs; set => SetProperty(ref _visibleTabs,value); }
        public ObservableCollection<IGroup> ListGroups { get => _listgroups; set => SetProperty(ref _listgroups,value); }

    }
}
