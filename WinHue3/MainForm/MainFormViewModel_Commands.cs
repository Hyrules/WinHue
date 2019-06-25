using System;
using System.Windows.Input;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.Lights.SupportedDevices;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private bool EnableButtons()
        {
            return SelectedBridge != null;
        }

        private bool CanBridgeSettings()
        {
            return SelectedBridge != null;
        }

        private bool IsObjectSelected()
        {
            RaisePropertyChanged("CanTT");
            return SelectedObject != null;
        }

        private bool IsEditable()
        {
            if (!IsObjectSelected()) return false;
            if (IsGroupZero()) return false;
            switch (SelectedObject)
            {
                case Group group when @group.@class == "TV":
                case Scene scene when scene.version == 1:
                    return false;
                default:
                    return !(SelectedObject is Light);
            }
        }

        public bool CanSearchNewLights()
        {
            if (!EnableButtons()) return false;
            return !SearchingLights;
        }

        private bool CanSearchNewSensor()
        {
            if (!EnableButtons()) return false;
            return !SearchingSensors;
        }

    
        private bool CanHue()
        {
            if (!IsObjectSelected()) return false;
            switch (SelectedObject)
            {
                case Light light when light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix:
                    return false;
                case Light light when light.state.@on == false && WinHueSettings.settings.SlidersBehavior == 0:
                    return false;
                case Light light:
                    return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canhue;
                case Group group:
                    return @group.action?.hue != null;
                default:
                    return false;
            }
        }

        private bool CanBri()
        {
            if (!IsObjectSelected()) return false;
            switch (SelectedObject)
            {
                case Light light when light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix:
                    return false;
                case Light light when light.state.@on == false && WinHueSettings.settings.SlidersBehavior == 0:
                    return false;
                case Light light:
                    return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canbri;
                case Group group:
                    return @group.action?.bri != null;
                default:
                    return false;
            }
        }

        private bool CanCt()
        {
            if (!IsObjectSelected()) return false;
            switch (SelectedObject)
            {
                case Light light when light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix:
                    return false;
                case Light light when light.state.@on == false && WinHueSettings.settings.SlidersBehavior == 0:
                    return false;
                case Light light:
                    return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canct;
                case Group group:
                    return @group.action?.ct != null;
                default:
                    return false;
            }
        }

        private bool CanSat()
        {
            if (!IsObjectSelected()) return false;
            switch (SelectedObject)
            {
                case Light light when light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix:
                    return false;
                case Light light when light.state.@on == false && WinHueSettings.settings.SlidersBehavior == 0:
                    return false;
                case Light light:
                    return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Cansat;
                case Group _:
                    return ((Group)SelectedObject).action?.sat != null;
                default:
                    return false;
            }
        }

        private bool CanXy()
        {
            if (!IsObjectSelected()) return false;
            switch (SelectedObject)
            {
                case Light light when light.state.reachable == false && light.manufacturername != "OSRAM" && WinHueSettings.settings.OSRAMFix:
                    return false;
                case Light light when light.state.@on == false && WinHueSettings.settings.SlidersBehavior == 0:
                    return false;
                case Light light:
                    return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canxy;
                case Group group:
                    return @group.action?.xy != null;
                default:
                    return false;
            }
        }

        private bool IsDoubleClickable()
        {
            return SelectedObject is Light || SelectedObject is Group || SelectedObject is Scene;
        }

        private bool CanIdentify()
        {
            switch (SelectedObject)
            {
                case Group _:
                    return true;
                case Light light:
                    return SupportedDeviceType.DeviceType.ContainsKey(light.type) && SupportedDeviceType.DeviceType[light.type].Canalert;
                default:
                    return false;
            }
        }

        private bool CanSetSensivity()
        {
            return SelectedObject is Sensor sensor && sensor.type == "ZLLPresence";
        }

        private bool CanReplaceState()
        {
            if (!IsObjectSelected()) return false;
            return SelectedObject is Scene;
        }

        private bool CanCloneSensor()
        {
            return SelectedObject is Sensor && ((Sensor) SelectedObject).type.Contains("CLIP");
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
            return SelectedObject is Group group && group.Id == "0";
        }

        private bool CanUpdateBridge()
        {
            if (!EnableButtons()) return false;
            return SelectedBridge != null && SelectedBridge.UpdateAvailable;
        }

        private bool CanStrobe()
        {
            return SelectedObject is Light || SelectedObject is Group;
        }

        private bool CanToggleDim()
        {
            if (!IsObjectSelected()) return false;
            return SelectedObject is Light || SelectedObject is Group;
        }

        private bool CanSetSensorStatus()
        {
            return SelectedObject is Sensor sensor && sensor.type == "CLIPGenericStatus";
        }

        private bool CanSetSensorFlag()
        {
            return SelectedObject is Sensor sensor && sensor.type == "CLIPGenericFlag";
        }

        private bool CanStream()
        {
            return SelectedObject is Group group && group.type == "Entertainment";
        }

        //*************** MainMenu Commands ********************        

        public ICommand OpenSettingsWindowCommand => new AsyncRelayCommand(param => OpenSettingsWindow());
        public ICommand QuitApplicationCommand => new RelayCommand(param => QuitApplication());

        //*************** Initialization Command *************

        public ICommand InitializeCommand => new RelayCommand(param => Initialize());

        //*************** Toolbar Commands ********************        
        public ICommand CheckForUpdateCommand => new AsyncRelayCommand(param => CheckForBridgeUpdate(), param => EnableButtons());
        public ICommand UpdateBridgeCommand => new AsyncRelayCommand(param => DoBridgeUpdate(), param => CanUpdateBridge());
        public ICommand ManageUsersCommand => new AsyncRelayCommand(param => ManageUsers(),  param => EnableButtons());
        public ICommand ChangeBridgeSettingsCommand => new AsyncRelayCommand(param => ChangeBridgeSettings(), param => CanBridgeSettings());
        public ICommand RefreshViewCommand => new AsyncRelayCommand(param => RefreshCurrentListHueObject(), param => EnableButtons());
        public ICommand CreateGroupCommand => new AsyncRelayCommand(param => CreateGroup(), param => EnableButtons());
        public ICommand CreateSceneCommand => new AsyncRelayCommand(param => CreateScene(), param => EnableButtons());
        public ICommand CreateScheduleCommand => new AsyncRelayCommand(param => CreateSchedule(), param => EnableButtons());
        public ICommand CreateRuleCommand => new AsyncRelayCommand(param => CreateRule(), param => EnableButtons());
        public ICommand CreateSensorCommand => new RelayCommand(param => CreateSensor(), param => EnableButtons());
        public ICommand CreateAdvancedCommand => new RelayCommand(param => CreateAdvanced(),  param => EnableButtons());
        public ICommand CreateFloorPlanCommand => new RelayCommand(param => CreateFloorPlan(), (param)=> EnableButtons());
        public ICommand CreateEntertainmentCommand => new AsyncRelayCommand(param => CreateEntertainment(),  param => EnableButtons());

        public ICommand CreateMqttConditionCommand => new RelayCommand(param => CreateMqttConditions(), param => EnableButtons());

        public ICommand CreateAnimationCommand => new RelayCommand(param => CreateAnimation());
        public ICommand TouchLinkCommand => new AsyncRelayCommand(param => DoTouchLink(), param => EnableButtons());
        public ICommand FindLightSerialCommand => new RelayCommand(param => FindLightSerial(), (param) => EnableButtons());
        public ICommand CreateHotKeyCommand => new AsyncRelayCommand(param => CreateHotKey(), param => EnableButtons());
        public ICommand CreateResourceLinkCommand => new AsyncRelayCommand(param => CreateResourceLink(), param => EnableButtons());
        public ICommand AllOnCommand => new AsyncRelayCommand(param => AllOnOff(true), param => EnableButtons());
        public ICommand AllOffCommand => new AsyncRelayCommand(param => AllOnOff(false), param => EnableButtons());
        public ICommand ShowEventLogCommand => new RelayCommand(param => ShowEventLog());
        public ICommand SearchNewLightsCommand => new AsyncRelayCommand(param => CheckForNewBulb(), param => CanSearchNewLights());
        public ICommand SearchNewSensorsCommand => new AsyncRelayCommand(param => SearchNewSensors(), param => CanSearchNewSensor());
        public ICommand ResetTransitionTimeCommand => new RelayCommand(param => ResetTransitionTime(), (param)=> EnableButtons());
        //*************** Sliders Commands ********************
        public ICommand SliderHueChangedCommand => new AsyncRelayCommand(param => SliderChangeHue(), param => CanHue());
        public ICommand SliderBriChangedCommand => new AsyncRelayCommand(param => SliderChangeBri(), param => CanBri());
        public ICommand SliderCtChangedCommand => new AsyncRelayCommand(param => SliderChangeCt(), param => CanCt());
        public ICommand SliderSatChangedCommand => new AsyncRelayCommand(param => SliderChangeSat(), param => CanSat());
        public ICommand SliderXyChangedCommand => new AsyncRelayCommand(param => SliderChangeXy(), param => CanXy());
        public ICommand HueKeyPressCommand => new AsyncRelayCommand(SliderChangeHueKeypress);
        public ICommand BriKeyPressCommand => new AsyncRelayCommand(SliderChangeBriKeypress);
        public ICommand SatKeyPressCommand => new AsyncRelayCommand(SliderChangeSatKeypress);
        public ICommand CtKeyPressCommand => new AsyncRelayCommand(SliderChangeCtKeypress);

        //*************** App Menu Commands ******************
        public ICommand DoBridgePairingCommand => new RelayCommand(param => DoBridgePairing());
        public ICommand ExportDataStoreCommand => new AsyncRelayCommand(ExportDataStore, param => EnableButtons());

        //*************** Context Menu Commands *************
        public ICommand DeleteObjectCommand => new AsyncRelayCommand(param => DeleteObject(), param => CanDelete());
        public ICommand RenameObjectCommand => new RelayCommand(param => RenameObject(), (param) => CanRename());
        public ICommand EditObjectCommand => new AsyncRelayCommand(param => EditObject(), param => IsEditable());
        public ICommand IdentifyLongCommand => new AsyncRelayCommand(param => Identify("lselect"), param => CanIdentify());
        public ICommand IdentifyShortCommand => new AsyncRelayCommand(param => Identify("select"), param => CanIdentify());
        public ICommand IdentifyStopCommand => new AsyncRelayCommand(param => Identify("none"), param => CanIdentify());
        public ICommand ReplaceCurrentStateCommand => new AsyncRelayCommand(param => ReplaceCurrentState(), param => CanReplaceState());
        public ICommand SensitivityHighCommand => new AsyncRelayCommand(param => Sensitivity(2), param => CanSetSensivity());
        public ICommand SensitivityMediumCommand => new AsyncRelayCommand(param => Sensitivity(1), param => CanSetSensivity());
        public ICommand SensitivityLowCommand => new AsyncRelayCommand(param => Sensitivity(0), param => CanSetSensivity());
        public ICommand CloneCommand => new AsyncRelayCommand(param => Clone(false), param => CanClone());
        public ICommand QuickCloneCommand => new AsyncRelayCommand(param => Clone(true), param => CanClone());
        public ICommand CopyToJsonCommand => new RelayCommand(param => CopyToJson(false), (param) => IsObjectSelected());
        public ICommand CopyToJsonRawCommand => new RelayCommand(param => CopyToJson(true), (param) => IsObjectSelected());
        public ICommand ColorloopCommand => new AsyncRelayCommand(param => Colorloop(), param => CanIdentify());
        public ICommand NoEffectCommand => new AsyncRelayCommand(param => NoEffect(), param => CanIdentify());
        public ICommand StrobeCommand => new AsyncRelayCommand(param =>Strobe(), param => CanStrobe());
        public ICommand ToggleDim10Command => new AsyncRelayCommand(param => OnDim(25), param => CanToggleDim());
        public ICommand ToggleDim25Command => new AsyncRelayCommand(param => OnDim(64), param => CanToggleDim());
        public ICommand ToggleDim50Command => new AsyncRelayCommand(param => OnDim(128), param => CanToggleDim());
        public ICommand ToggleDim75Command => new AsyncRelayCommand(param => OnDim(191), param => CanToggleDim());
        public ICommand SetSensorStatusCommand => new AsyncRelayCommand(param => SetSensorStatus(), param => CanSetSensorStatus());
        public ICommand SetSensorFlagCommand => new AsyncRelayCommand(param => SetSensorFlag(), param => CanSetSensorFlag());
        public ICommand EnableStreamCommand => new AsyncRelayCommand(param => EnableStreaming(), (param) => CanStream());

        //*************** ListView Commands ********************
        public ICommand DoubleClickObjectCommand => new AsyncRelayCommand(param => DoubleClickObject(), param => IsDoubleClickable());
        public ICommand ClickObjectCommand => new AsyncRelayCommand(param => ClickObject());

        //*************** Views Commands ************************
        public ICommand ViewSceneMappingCommand => new AsyncRelayCommand(param => ViewSceneMapping(), param => EnableButtons());
        public ICommand ViewBulbsCommand => new AsyncRelayCommand(param => ViewBulbs(), param => EnableButtons());
        public ICommand ViewGroupsCommand => new AsyncRelayCommand(param => ViewGroups(), param => EnableButtons());
        public ICommand SortListViewCommand => new AsyncRelayCommand(param => SortListView(), param => EnableButtons());
        public ICommand ShowPropertyGridCommand => new RelayCommand(param => ShowPropertyGrid());

        public ICommand SwitchViewCommandIcon => new RelayCommand(param => SwitchView(1));
        public ICommand SwitchViewCommandList => new RelayCommand(param => SwitchView(2));
        //*************** StatusBar Commands ************************
        public ICommand ChangeBridgeCommand => new AsyncRelayCommand(param => ChangeBridge());
        public ICommand DoAppUpdateCommand=> new RelayCommand(param => DoAppUpdate());
        //*************** Toolbar ******************************
        public ICommand CpuTempMonCommand => new RelayCommand(param => RunCpuTempMon(), (param) => EnableButtons() && CanRunTempPlugin);
        public ICommand CpuTempMonSettingsCommand => new RelayCommand(param => CpuTempMonSettings(), (param) => EnableButtons() && CanRunTempPlugin);

        //*************** Help ******************************
        public ICommand OpenWinHueWebsiteCommand => new RelayCommand(param => OpenWinHueWebsite());
        public ICommand OpenWinHueSupportCommand => new RelayCommand(param => OpenWinHueSupport());

        public ICommand LoadVirtualBridgeCommand => new RelayCommand(param => LoadVirtualBridge());
        
        //*************** Title bar **************************
        public ICommand MinimizeToTrayCommand => new RelayCommand(param => MinimizeToTray());


        public ICommand SelectHueElementCommand => new AsyncRelayCommand(param => SelectHueElement());
        public ICommand SelectedFloorPlanChangedCommand => new RelayCommand(param => SelectedFloorPlanChanged());
        public ICommand SetPowerModeCommand => new AsyncRelayCommand(param => SetPowerMode(), (param) => EnableButtons());

    }
}
