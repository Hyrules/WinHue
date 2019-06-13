using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Resources;
using WinHue3.Utils;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Functions.RoomMap;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        
        private ushort? _sliderTT;
        private IBaseProperties _newstate;
        private int _sensorStatus;
        private bool _sensorFlag;
        private ObservableCollection<Floor> _listFloorPlans;
        private Floor _selectedFloorPlan;
        private HueElement _selectedHueElement;
        private int _currentView;

        public bool CanTT
        {
            get
            {
                if (SelectedObject == null) return false;
                return SelectedObject is Light || SelectedObject is Group;
            }
        }

        public object IsMasterDebugger => System.Diagnostics.Debugger.IsAttached;

        private bool CanRunTempPlugin => UacHelper.IsProcessElevated();

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
            set => SetProperty(ref _lastmessage, value);

        }

        public int CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string TransitionTimeTooltip
        {
            get
            {
                if (SliderTt >= 0)
                {
                    int time = (int) (SliderTt * 100);
                    if (time == 0)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {GUI.MainForm_Sliders_TransitionTime_Instant}";
                    }
                    else if (time > 0 && time < 1000)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {(double) time:0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Millisec}";
                    }
                    else if (time >= 1000 && time < 60000)
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {((double) time / 1000):0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Seconds}";
                    }
                    else
                    {
                        return $"{GUI.MainForm_Sliders_TransitionTime} : {((double) time / 60000):0.##} {GUI.MainForm_Sliders_TransitionTime_Unit_Minutes}";
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

        public bool MultiBridgeCB => ListBridges.Count > 1;

        public Visibility UpdateAvailable
        {
            get
            {
                BridgeSettings cr = SelectedBridge?.GetBridgeSettings();
                if (cr == null) return Visibility.Collapsed;
                return cr.swupdate.updatestate == 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ObservableCollection<Floor> ListFloorPlans
        {
            get => _listFloorPlans;
            set => SetProperty(ref _listFloorPlans, value);
        }

        public Floor SelectedFloorPlan
        {
            get => _selectedFloorPlan;
            set => SetProperty(ref _selectedFloorPlan, value);
        }

        public HueElement SelectedHueElement
        {
            get => _selectedHueElement;
            set => SetProperty(ref _selectedHueElement, value);
        }
    }
}