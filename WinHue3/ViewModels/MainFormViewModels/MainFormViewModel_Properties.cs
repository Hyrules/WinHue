using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Resources;
using WinHue3.Utils;
using WinHue3.Views;


namespace WinHue3.ViewModels.MainFormViewModels
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private Form_EventLog _eventlogform;
        private ObservableCollection<Bridge> _listBridges;
        private IHueObject _selectedObject;
        private Bridge _selectedBridge;
        private ushort? _sliderTT;
        private bool _editName;

        [RefreshProperties(RefreshProperties.All)]
        public Bridge SelectedBridge
        {
            get => _selectedBridge;
            set => SetProperty(ref _selectedBridge,value);
        }

        private bool CanRunTempPlugin => UacHelper.IsProcessElevated;

        public ObservableCollection<IHueObject> ListBridgeObjects
        {
            get => _listBridgeObjects;
            set { SetProperty(ref _listBridgeObjects, value); RaisePropertyChanged("MultiBridgeCB");}
        }

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

        public IHueObject SelectedObject
        {
            get => _selectedObject;
            set => SetProperty(ref _selectedObject,value);
        }

        public Views.Form_EventLog Eventlogform
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
    }
}
