using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;
using WinHue3.SupportedLights;
using System.Diagnostics;

namespace WinHue3.ViewModels
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        public bool EnableButtons()
        {
            return SelectedBridge != null;
        }

        private bool IsObjectSelected()
        {
            return SelectedObject != null;
        }

        private bool IsEditable()
        {
            if (!IsObjectSelected()) return false;
            if (IsGroupZero()) return false;
            if (SelectedObject is Scene && ((Scene) SelectedObject).version == 1) return false;
            return !(SelectedObject is Light);
        }

        private bool CanSchedule()
        {
            if (!IsObjectSelected()) return false;
            return SelectedObject is Light || SelectedObject is Group || SelectedObject is Scene ;
        }

        private bool CanSearchNewLights()
        {
            return !_findlighttimer.IsEnabled;
        }

        private bool CanSearchNewSensor()
        {
            return !_findsensortimer.IsEnabled;
        }

        private bool CanHue()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedObject is Light)
            {
                Light light = ((Light)SelectedObject);
                if (light.state.on == false) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canhue;
            }
            else if (SelectedObject is Group)
            {
                return ((Group)SelectedObject).action?.hue != null;
            }
            return false;
        }

        private bool CanBri()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedObject is Light)
            {
                Light light = ((Light)SelectedObject);
                if (light.state.on == false) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canbri;
            }
            else if (SelectedObject is Group)
            {
                return ((Group)SelectedObject).action?.bri != null;
            }
            return false;
        }

        private bool CanCt()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedObject is Light)
            {
                Light light = ((Light)SelectedObject);
                if (light.state.on == false) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canct;
            }
            else if (SelectedObject is Group)
            {
                return ((Group)SelectedObject).action?.ct != null;
            }
            return false;
        }

        private bool CanSat()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedObject is Light)
            {
                Light light = ((Light)SelectedObject);
                if (light.state.on == false) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Cansat;
            }
            else if (SelectedObject is Group)
            {
                return ((Group)SelectedObject).action?.sat != null;
            }
            return false;
        }

        private bool CanXy()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedObject is Light)
            {
                Light light = ((Light)SelectedObject);
                if (light.state.on == false) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canxy;
            }
            else if (SelectedObject is Group)
            {
                return ((Group)SelectedObject).action?.xy != null;
            }
            return false;
        }

        private bool IsDoubleClickable()
        {
            return SelectedObject is Light || SelectedObject is Group || SelectedObject is Scene;
        }

        private bool CanIdentify()
        {
            if (SelectedObject is Group) return true;
            if (SelectedObject is Light)
            {
                Light light = ((Light)SelectedObject);
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canalert;
            }
            return false;
        }

        public bool CanSetSensivity()
        {
            if (!(SelectedObject is Sensor)) return false;

            return ((Sensor) SelectedObject).type == "ZLLPresence";
        }

        private bool CanReplaceState()
        {
            if (!IsObjectSelected()) return false;
            return SelectedObject is Scene;
        }

        private bool CanCloneSensor()
        {
            if (SelectedObject is Sensor)
            {
                return ((Sensor) SelectedObject).type.Contains("CLIP");
            }
            return false;
        }

        private bool CanClone()
        {
            if (!IsObjectSelected()) return false;
            return SelectedObject is Scene | SelectedObject is Group | SelectedObject is Rule | CanCloneSensor() | SelectedObject is Resourcelink;
        }

        private bool CanRename()
        {
            if (!IsObjectSelected()) return false;
            return !IsGroupZero();
        }

        private bool CanDelete()
        {
            if (!IsObjectSelected()) return false;
            return !IsGroupZero();
        }

        private bool IsGroupZero()
        {
            return SelectedObject is Group && SelectedObject.Id == "0";
        }

        //*************** MainMenu Commands ********************        

        public ICommand OpenSettingsWindowCommand => new RelayCommand(param => OpenSettingsWindow());

        public ICommand QuitApplicationCommand => new RelayCommand(param => QuitApplication());

        //*************** Initialization Command *************

        public ICommand InitializeCommand => new RelayCommand(param => Initialize());

        //*************** Toolbar Commands ********************        
        public ICommand CheckForNewBulbCommand => new RelayCommand(param => CheckForNewBulb(), (param) => EnableButtons());
        public ICommand UpdateBridgeCommand => new RelayCommand(param => DoBridgeUpdate());
        public ICommand ManageUsersCommand => new RelayCommand(param => ManageUsers(), (param) => EnableButtons());
        public ICommand ChangeBridgeSettingsCommand => new RelayCommand(param => ChangeBridgeSettings(), (param) => EnableButtons());
        public ICommand RefreshViewCommand => new RelayCommand(param => RefreshView(), (param) => EnableButtons());
        public ICommand CreateGroupCommand => new RelayCommand(param => CreateGroup(), (param) => EnableButtons());
        public ICommand CreateSceneCommand => new RelayCommand(param => CreateScene(), (param) => EnableButtons());
        public ICommand CreateScheduleCommand => new RelayCommand(param => CreateSchedule(), (param) => EnableButtons() && CanSchedule());
        public ICommand CreateRuleCommand => new RelayCommand(param => CreateRule(), (param) => EnableButtons());
        public ICommand CreateSensorCommand => new RelayCommand(param => CreateSensor(), (param) => EnableButtons());
        public ICommand CreateAdvancedCommand => new RelayCommand(param => CreateAdvanced(), (param) => EnableButtons());

        //  public ICommand CreateAnimationCommand => new RelayCommand(param => CreateAnimation());
        public ICommand CreateHotKeyCommand => new RelayCommand(param => CreateHotKey(), (param) => EnableButtons());
        public ICommand CreateResourceLinkCommand => new RelayCommand(param => CreateResourceLink(), (param) => EnableButtons());
        public ICommand AllOnCommand => new RelayCommand(param => AllOn(), (param) => EnableButtons());
        public ICommand AllOffCommand => new RelayCommand(param => AllOff(), (param) => EnableButtons());
        public ICommand ShowEventLogCommand => new RelayCommand(param => ShowEventLog());
        public ICommand SearchNewLightsCommand => new RelayCommand(param => CheckForNewBulb(), (param) => EnableButtons() && CanSearchNewLights());
        public ICommand SearchNewSensorsCommand => new RelayCommand(param => SearchNewSensors(), (param) => EnableButtons() && CanSearchNewSensor());
        public ICommand ResetTransitionTimeCommand => new RelayCommand(param => ResetTransitionTime(), (param)=> EnableButtons());
        //*************** Sliders Commands ********************
        public ICommand SliderHueChangedCommand => new RelayCommand(param => SliderChangeHue(), (param) => CanHue());
        public ICommand SliderBriChangedCommand => new RelayCommand(param => SliderChangeBri(), (param) => CanBri());
        public ICommand SliderCtChangedCommand => new RelayCommand(param => SliderChangeCt(), (param) => CanCt());
        public ICommand SliderSatChangedCommand => new RelayCommand(param => SliderChangeSat(), (param) => CanSat());
        public ICommand SliderXyChangedCommand => new RelayCommand(param => SliderChangeXy(), (param) => CanXy());

        //*************** App Menu Commands ******************
        public ICommand DoBridgePairingCommand => new RelayCommand(param => DoBridgePairing(ListBridges));

        //*************** Context Menu Commands *************
        public ICommand DeleteObjectCommand => new RelayCommand(param => DeleteObject(), (param) => CanDelete());
        public ICommand RenameObjectCommand => new RelayCommand(param => RenameObject(), (param) => CanRename());
        public ICommand EditObjectCommand => new RelayCommand(param => EditObject(), (param) => IsEditable());
        public ICommand IdentifyLongCommand => new RelayCommand(param => Identify("lselect"), (param) => CanIdentify());
        public ICommand IdentifyShortCommand => new RelayCommand(param => Identify("select"), (param) => CanIdentify());
        public ICommand IdentifyStopCommand => new RelayCommand(param => Identify("none"), (param) => CanIdentify());
        public ICommand ReplaceCurrentStateCommand => new RelayCommand(param => ReplaceCurrentState(), (param) => CanReplaceState());
        public ICommand SensitivityHighCommand => new RelayCommand(param => Sensitivity(2), (param) => CanSetSensivity());
        public ICommand SensitivityMediumCommand => new RelayCommand(param => Sensitivity(1), (param) => CanSetSensivity());
        public ICommand SensitivityLowCommand => new RelayCommand(param => Sensitivity(0), (param) => CanSetSensivity());
        public ICommand CloneCommand => new RelayCommand(param => Clone(false),(param) => CanClone());
        public ICommand QuickCloneCommand => new RelayCommand(param => Clone(true), (param) => CanClone());
        public ICommand CopyToJsonCommand => new RelayCommand(param => CopyToJson(false), (param) => IsObjectSelected());
        public ICommand CopyToJsonRawCommand => new RelayCommand(param => CopyToJson(true), (param) => IsObjectSelected());
        public ICommand ColorloopCommand => new RelayCommand(param => Colorloop(), (param) => CanIdentify());
        public ICommand NoEffectCommand => new RelayCommand(param => NoEffect(), (param) => CanIdentify());
        //*************** ListView Commands ********************
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => DoubleClickObject(), (param) => IsDoubleClickable());

        //*************** Views Commands ************************
        public ICommand ViewSceneMappingCommand => new RelayCommand(param => ViewSceneMapping(), (param) => EnableButtons());
        public ICommand ViewBulbsCommand => new RelayCommand(param => ViewBulbs(), (param) => EnableButtons());
        public ICommand ViewGroupsCommand => new RelayCommand(param => ViewGroups(), (param) => EnableButtons());
        public ICommand SortListViewCommand => new RelayCommand(param => SortListView(), (param) => EnableButtons());



        //*************** Toolbar ******************************
        public ICommand CpuTempMonCommand => new RelayCommand(param => RunCpuTempMon(), (param) => EnableButtons() && CanRunTempPlugin);
        public ICommand CpuTempMonSettingsCommand => new RelayCommand(param => CpuTempMonSettings(), (param) => EnableButtons() && CanRunTempPlugin);
        public ICommand RssFeedMonCommand => new RelayCommand(param => RssFeedMon(), (param) => EnableButtons());
        public ICommand RssFeedMonSettingsCommand => new RelayCommand(param => RssFeedMonSettings(), (param) => EnableButtons());
        //*************** Help ******************************
        public ICommand OpenWinHueWebsiteCommand => new RelayCommand(param => OpenWinHueWebsite());

        public ICommand OpenWinHueSupportCommand => new RelayCommand(param => OpenWinHueSupport());

        public ICommand OpenAboutWindowCommand => new RelayCommand(param => OpenAboutWindow());

        public ICommand StartProcDumpCommand => new RelayCommand(param => StartProcDump());
        //      public ICommand RssFeedMonCommand => new RelayCommand(param => RunRssFeedMon(), (param) => EnableButtons());
        //      
        //     public ICommand RssFeedMonSettingsCommand => new RelayCommand(param => RssFeedMonSettings(), (param) => EnableButtons());
        //   public ICommand ClapperCommand => new RelayCommand(param => Clapper(), (param) => EnableButtons());
    }
}
