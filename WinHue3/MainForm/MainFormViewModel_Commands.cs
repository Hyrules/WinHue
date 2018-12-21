using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Windows.Input;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.Lights.SupportedDevices;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private bool EnableButtons()
        {
            return SelectedBridge != null && EnableListView.GetValueOrDefault(false);
        }

        private bool CanBridgeSettings()
        {
            return SelectedBridge != null;
        }

        private bool IsObjectSelected()
        {
            return SelectedHueObject != null;
        }

        private bool IsEditable()
        {
            if (!IsObjectSelected()) return false;
            if (IsGroupZero()) return false;
            if (SelectedHueObject is Scene && ((Scene) SelectedHueObject).version == 1) return false;
            return !(SelectedHueObject is Light);
        }

        private bool CanSchedule()
        {
            if (!IsObjectSelected() ) return false;
            return SelectedHueObject is Light || SelectedHueObject is Group || SelectedHueObject is Scene ;
        }

        public bool? EnableListView
        {
            get
            {
                return !_selectedBridge?.RequiredUpdate;
            }
        }

        public bool SearchingLights => _findlighttimer.IsEnabled;

        public bool CanSearchNewLights()
        {
            return !_findlighttimer.IsEnabled ;
        }

        private bool CanSearchNewSensor()
        {
            return !_findsensortimer.IsEnabled ;
        }

        private void Expand(Type objecttype)
        {
            switch (objecttype)
            {
                case Type light when light == typeof(Light):
                    break;
                case Type group when group == typeof(Group):
                    break;
                case Type scene when scene == typeof(Scene):
                    break;
                case Type sensor when sensor == typeof(Sensor):
                    break;
                case Type schedule when schedule == typeof(Schedule):
                    break;
                case Type rl when rl == typeof(Resourcelink):
                    break;
                case Type rule when rule == typeof(Rule):
                    break;
                default:
                    break;
            }

        }


        private void Collapse(Type objecttype)
        {
            switch (objecttype)
            {
                case Type light when light == typeof(Light):
                    break;
                case Type group when group == typeof(Group):
                    break;
                case Type scene when scene == typeof(Scene):
                    break;
                case Type sensor when sensor == typeof(Sensor):
                    break;
                case Type schedule when schedule == typeof(Schedule):
                    break;
                case Type rl when rl == typeof(Resourcelink):
                    break;
                case Type rule when rule == typeof(Rule):
                    break;
                default:
                    break;
            }
        }

        private bool CanHue()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedHueObject is Light light)
            {
                if (light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix) return false;
                if (light.state.on == false && WinHueSettings.settings.SlidersBehavior == 0) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canhue;
            }
            else if (SelectedHueObject is Group)
            {
                return ((Group)SelectedHueObject).action?.hue != null;
            }
            return false;
        }

        private bool CanBri()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedHueObject is Light light)
            {
                if (light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix) return false;
                if (light.state.on == false && WinHueSettings.settings.SlidersBehavior == 0) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canbri;
            }
            else if (SelectedHueObject is Group)
            {
                return ((Group)SelectedHueObject).action?.bri != null;
            }
            return false;
        }

        private bool CanCt()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedHueObject is Light light)
            {
                if (light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix) return false;
                if (light.state.on == false && WinHueSettings.settings.SlidersBehavior == 0) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canct;
            }
            else if (SelectedHueObject is Group)
            {
                return ((Group)SelectedHueObject).action?.ct != null;
            }
            return false;
        }

        private bool CanSat()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedHueObject is Light light)
            {
                if (light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix) return false;
                if (light.state.on == false && WinHueSettings.settings.SlidersBehavior == 0) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Cansat;
            }
            else if (SelectedHueObject is Group)
            {
                return ((Group)SelectedHueObject).action?.sat != null;
            }
            return false;
        }

        private bool CanXy()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedHueObject is Light light)
            {
                if (light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix) return false;
                if (light.state.on == false && WinHueSettings.settings.SlidersBehavior == 0) return false;
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canxy;
            }
            else if (SelectedHueObject is Group)
            {
                return ((Group)SelectedHueObject).action?.xy != null;
            }
            return false;
        }

        private bool IsDoubleClickable()
        {
            return SelectedHueObject is Light || SelectedHueObject is Group || SelectedHueObject is Scene;
        }

        private bool CanIdentify()
        {
            if (SelectedHueObject is Group) return true;
            if (SelectedHueObject is Light light)
            {
                return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canalert;
            }
            return false;
        }

        private bool CanSetSensivity()
        {
            if (!(SelectedHueObject is Sensor)) return false;

            return ((Sensor) SelectedHueObject).type == "ZLLPresence";
        }

        private bool CanReplaceState()
        {
            if (!IsObjectSelected()) return false;
            return SelectedHueObject is Scene;
        }

        private bool CanCloneSensor()
        {
            if (SelectedHueObject is Sensor)
            {
                return ((Sensor) SelectedHueObject).type.Contains("CLIP");
            }
            return false;
        }

        private bool CanClone()
        {
            if (!IsObjectSelected()) return false;
            return SelectedHueObject is Scene | SelectedHueObject is Group | SelectedHueObject is Rule | CanCloneSensor() | SelectedHueObject is Resourcelink;
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
            return SelectedHueObject is Group && SelectedHueObject.Id == "0";
        }

        private bool CanUpdateBridge()
        {
            if (_selectedBridge == null) return false;
            return SelectedBridge.UpdateAvailable;
        }

        private bool CanStrobe()
        {
            return SelectedHueObject is Light || SelectedHueObject is Group;
        }

        private bool CanToggleDim()
        {
            if (!IsObjectSelected()) return false;
            if (!(SelectedHueObject is Light || SelectedHueObject is Group)) return false;
            return true;
        }

        private bool CanSetSensorStatus()
        {
            if (!(SelectedHueObject is Sensor)) return false;
            return ((Sensor)SelectedHueObject).type == "CLIPGenericStatus";
        }

        private bool CanSetSensorFlag()
        {
            if (!(SelectedHueObject is Sensor)) return false;
            return ((Sensor)SelectedHueObject).type == "CLIPGenericFlag";
        }

        //*************** MainMenu Commands ********************        

        public ICommand OpenSettingsWindowCommand => new AsyncCommand<object>(param => OpenSettingsWindow());
        public ICommand QuitApplicationCommand => new RelayCommand(param => QuitApplication());

        //*************** Initialization Command *************

        public ICommand InitializeCommand => new RelayCommand(param => Initialize());

        //*************** Toolbar Commands ********************        
        public ICommand CheckForUpdateCommand => new AsyncCommand<object>(param => CheckForBridgeUpdate(), (param) => EnableButtons());
        public ICommand UpdateBridgeCommand => new AsyncCommand<object>(param => DoBridgeUpdate(), (param) => EnableButtons() && CanUpdateBridge());
        public ICommand ManageUsersCommand => new AsyncCommand<object>(param => ManageUsers(), (param) => EnableButtons());
        public ICommand ChangeBridgeSettingsCommand => new AsyncCommand<object>(param => ChangeBridgeSettings(), (param) => CanBridgeSettings());
        public ICommand RefreshViewCommand => new AsyncCommand<object>(param => RefreshView(), (param) => EnableButtons());
        public ICommand CreateGroupCommand => new AsyncCommand<object>(param => CreateGroup(), (param) => EnableButtons());
        public ICommand CreateSceneCommand => new AsyncCommand<object>(param => CreateScene(), (param) => EnableButtons());
        public ICommand CreateScheduleCommand => new AsyncCommand<object>(param => CreateSchedule(), (param) => EnableButtons());
        public ICommand CreateRuleCommand => new AsyncCommand<object>(param => CreateRule(), (param) => EnableButtons());
        public ICommand CreateSensorCommand => new RelayCommand(param => CreateSensor(), (param) => EnableButtons());
        public ICommand CreateAdvancedCommand => new RelayCommand(param => CreateAdvanced(), (param) => EnableButtons());
        public ICommand CreateFloorPlanCommand => new RelayCommand(param => CreateFloorPlan(), (param)=> EnableButtons());
        public ICommand CreateEntertainmentCommand => new RelayCommand(param => CreateEntertainment(), (param) => EnableButtons());

        public ICommand CreateAnimationCommand => new RelayCommand(param => CreateAnimation());
        public ICommand TouchLinkCommand => new AsyncCommand<object>(param => DoTouchLink(), (param) => EnableButtons());
        public ICommand FindLightSerialCommand => new RelayCommand(param => FindLightSerial(), (param) => EnableButtons());
        public ICommand CreateHotKeyCommand => new AsyncCommand<object>(param => CreateHotKey(), (param) => EnableButtons());
        public ICommand CreateResourceLinkCommand => new AsyncCommand<object>(param => CreateResourceLink(), (param) => EnableButtons());
        public ICommand AllOnCommand => new AsyncCommand<object>(param => AllOn(), (param) => EnableButtons());
        public ICommand AllOffCommand => new AsyncCommand<object>(param => AllOff(), (param) => EnableButtons());
        public ICommand ShowEventLogCommand => new RelayCommand(param => ShowEventLog());
        public ICommand SearchNewLightsCommand => new AsyncCommand<object>(param => CheckForNewBulb(), (param) => EnableButtons() && CanSearchNewLights());
        public ICommand SearchNewSensorsCommand => new AsyncCommand<object>(param => SearchNewSensors(), (param) => EnableButtons() && CanSearchNewSensor());
        public ICommand ResetTransitionTimeCommand => new RelayCommand(param => ResetTransitionTime(), (param)=> EnableButtons());
        //*************** Sliders Commands ********************
        public ICommand SliderHueChangedCommand => new AsyncCommand<object>(param => SliderChangeHue(), (param) => CanHue());
        public ICommand SliderBriChangedCommand => new AsyncCommand<object>(param => SliderChangeBri(), (param) => CanBri());
        public ICommand SliderCtChangedCommand => new AsyncCommand<object>(param => SliderChangeCt(), (param) => CanCt());
        public ICommand SliderSatChangedCommand => new AsyncCommand<object>(param => SliderChangeSat(), (param) => CanSat());
        public ICommand SliderXyChangedCommand => new AsyncCommand<object>(param => SliderChangeXy(), (param) => CanXy());
        public ICommand HueKeyPressCommand => new AsyncCommand<object>(SliderChangeHueKeypress);
        public ICommand BriKeyPressCommand => new AsyncCommand<object>(SliderChangeBriKeypress);
        public ICommand SatKeyPressCommand => new AsyncCommand<object>(SliderChangeSatKeypress);
        public ICommand CtKeyPressCommand => new AsyncCommand<object>(SliderChangeCtKeypress);

        //*************** App Menu Commands ******************
        public ICommand DoBridgePairingCommand => new RelayCommand(param => DoBridgePairing(ListBridges));
        public ICommand ExportDataStoreCommand => new AsyncCommand<object>(ExportDataStore, (param) => EnableButtons());

        //*************** Context Menu Commands *************
        public ICommand DeleteObjectCommand => new AsyncCommand<object>(param => DeleteObject(), (param) => CanDelete());
        public ICommand RenameObjectCommand => new RelayCommand(param => RenameObject(), (param) => CanRename());
        public ICommand EditObjectCommand => new AsyncCommand<object>(param => EditObject(), (param) => IsEditable());
        public ICommand IdentifyLongCommand => new AsyncCommand<string>(param => Identify("lselect"), (param) => CanIdentify());
        public ICommand IdentifyShortCommand => new AsyncCommand<string>(param => Identify("select"), (param) => CanIdentify());
        public ICommand IdentifyStopCommand => new AsyncCommand<string>(param => Identify("none"), (param) => CanIdentify());
        public ICommand ReplaceCurrentStateCommand => new AsyncCommand<object>(param => ReplaceCurrentState(), (param) => CanReplaceState());
        public ICommand SensitivityHighCommand => new AsyncCommand<object>(param => Sensitivity(2), (param) => CanSetSensivity());
        public ICommand SensitivityMediumCommand => new AsyncCommand<object>(param => Sensitivity(1), (param) => CanSetSensivity());
        public ICommand SensitivityLowCommand => new AsyncCommand<object>(param => Sensitivity(0), (param) => CanSetSensivity());
        public ICommand CloneCommand => new AsyncCommand<bool>(param => Clone(false),(param) => CanClone());
        public ICommand QuickCloneCommand => new AsyncCommand<bool>(param => Clone(true), (param) => CanClone());
        public ICommand CopyToJsonCommand => new RelayCommand(param => CopyToJson(false), (param) => IsObjectSelected());
        public ICommand CopyToJsonRawCommand => new RelayCommand(param => CopyToJson(true), (param) => IsObjectSelected());
        public ICommand ColorloopCommand => new AsyncCommand<object>(param => Colorloop(), (param) => CanIdentify());
        public ICommand NoEffectCommand => new AsyncCommand<object>(param => NoEffect(), (param) => CanIdentify());
        public ICommand StrobeCommand => new AsyncCommand<object>(param =>Strobe(),(param) => CanStrobe());
        public ICommand ToggleDim10Command => new AsyncCommand<object>(param => OnDim(25), (param) => CanToggleDim());
        public ICommand ToggleDim25Command => new AsyncCommand<object>(param => OnDim(64), (param) => CanToggleDim());
        public ICommand ToggleDim50Command => new AsyncCommand<object>(param => OnDim(128), (param) => CanToggleDim());
        public ICommand ToggleDim75Command => new AsyncCommand<object>(param => OnDim(191), (param) => CanToggleDim());
        public ICommand SetSensorStatusCommand => new AsyncCommand<object>(param => SetSensorStatus(), (param) => CanSetSensorStatus());
        public ICommand SetSensorFlagCommand => new AsyncCommand<object>(param => SetSensorFlag(), (param)=> CanSetSensorFlag());

        //*************** ListView Commands ********************
        public ICommand DoubleClickObjectCommand => new AsyncCommand<object>(param => DoubleClickObject(), (param) => IsDoubleClickable());
        public ICommand ClickObjectCommand => new AsyncCommand<object>(param => ClickObject());
        public ICommand ExpandCommand => new RelayCommand((param) => Expand((Type)param), (param) => EnableButtons());
        public ICommand CollapseCommand => new RelayCommand((param) => Collapse((Type)param), (param) => EnableButtons());
        //*************** Views Commands ************************
        public ICommand ViewSceneMappingCommand => new AsyncCommand<object>(param => ViewSceneMapping(), (param) => EnableButtons());
        public ICommand ViewBulbsCommand => new AsyncCommand<object>(param => ViewBulbs(), (param) => EnableButtons());
        public ICommand ViewGroupsCommand => new AsyncCommand<object>(param => ViewGroups(), (param) => EnableButtons());
        public ICommand SortListViewCommand => new AsyncCommand<object>(param => SortListView(), (param) => EnableButtons());
        public ICommand ShowPropertyGridCommand => new RelayCommand(param => ShowPropertyGrid());
        //*************** StatusBar Commands ************************
        public ICommand ChangeBridgeCommand => new AsyncCommand<object>(param => ChangeBridge());
        public ICommand DoAppUpdateCommand=> new RelayCommand(param => DoAppUpdate());
        //*************** Toolbar ******************************
        public ICommand CpuTempMonCommand => new RelayCommand(param => RunCpuTempMon(), (param) => EnableButtons() && CanRunTempPlugin);
        public ICommand CpuTempMonSettingsCommand => new RelayCommand(param => CpuTempMonSettings(), (param) => EnableButtons() && CanRunTempPlugin);
        public ICommand RssFeedMonCommand => new RelayCommand(param => RssFeedMon(), (param) => EnableButtons());
        public ICommand RssFeedMonSettingsCommand => new RelayCommand(param => RssFeedMonSettings(), (param) => EnableButtons());
        //*************** Help ******************************
        public ICommand OpenWinHueWebsiteCommand => new RelayCommand(param => OpenWinHueWebsite());
        public ICommand OpenWinHueSupportCommand => new RelayCommand(param => OpenWinHueSupport());

        public ICommand LoadVirtualBridgeCommand => new RelayCommand(param => LoadVirtualBridge());
        
        //*************** Title bar **************************
        public ICommand MinimizeToTrayCommand => new RelayCommand(param => MinimizeToTray());


        public ICommand SelectHueElementCommand => new AsyncCommand<object>(param => SelectHueElement());
        public ICommand SelectedFloorPlanChangedCommand => new RelayCommand(param => SelectedFloorPlanChanged());
        public ICommand SetPowerModeCommand => new RelayCommand(param => SetPowerMode(), (param) => EnableButtons());

        //      public ICommand RssFeedMonCommand => new RelayCommand(param => RunRssFeedMon(), (param) => EnableButtons());
        //      
        //     public ICommand RssFeedMonSettingsCommand => new RelayCommand(param => RssFeedMonSettings(), (param) => EnableButtons());
        //   public ICommand ClapperCommand => new RelayCommand(param => Clapper(), (param) => EnableButtons());
    }
}
