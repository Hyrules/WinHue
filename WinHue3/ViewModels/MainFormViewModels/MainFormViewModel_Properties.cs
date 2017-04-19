using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using HueLib2;
using WinHue3.Resources;

namespace WinHue3.ViewModels
{
    public partial class MainFormViewModel
    {
        private Form_EventLog _eventlogform;
        private ObservableCollection<Bridge> _listBridges;
        private HueObject _selectedObject;
        private Bridge _selectedBridge;
        private ushort? _sliderTT;

        [RefreshProperties(RefreshProperties.All)]
        public Bridge SelectedBridge
        {
            get { return _selectedBridge; }
            set
            {
                SetProperty(ref _selectedBridge,value);
                RefreshView();     
                RaisePropertyChanged("UpdateAvailable");         
            }
        }

        public ObservableCollection<HueObject> ListBridgeObjects
        {
            get { return _listBridgeObjects; }
            set { SetProperty(ref _listBridgeObjects, value); RaisePropertyChanged("MultiBridgeCB");}
        }

        public string Lastmessage
        {
            get { return _lastmessage; }
            set { SetProperty(ref _lastmessage,value); }
        }

        public ObservableCollection<Bridge> ListBridges
        {
            get { return _listBridges; }
            set { SetProperty(ref _listBridges,value); RaisePropertyChanged("MultiBridgeCB");}
        }

        public HueObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                SetProperty(ref _selectedObject,value);
                if (value != null)
                {
                    MethodInfo mi = typeof(HueObjectHelper).GetMethod("GetObject");
                    MethodInfo generic = mi.MakeGenericMethod(value.GetType());
                    HelperResult hr = (HelperResult)generic.Invoke(SelectedBridge, new object[] {SelectedBridge, value.Id });
                    if (!hr.Success) return;
                    _selectedObject = (HueObject)hr.Hrobject;
                }
                SetMainFormModel();
            }
        }

        public Form_EventLog Eventlogform
        {
            get { return _eventlogform; }
            set { SetProperty(ref _eventlogform,value); }
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
            get { return _sliderTT; }
            set
            {
                SetProperty(ref _sliderTT, value); 
                RaisePropertyChanged("TransitionTimeTooltip");
            }
        }

        public Visibility MultiBridgeCB
        {
            get { return ListBridges?.Count > 1 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility UpdateAvailable
        {
            get
            {
                if(SelectedBridge == null) return Visibility.Collapsed;
               
                CommandResult cr = SelectedBridge.GetBridgeSettings();
                if (!cr.Success) return Visibility.Collapsed;
                BridgeSettings brs = (BridgeSettings) cr.resultobject;
                return brs.swupdate.updatestate == 2 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
