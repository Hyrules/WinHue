using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;
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
                return ((Light) SelectedObject).state?.hue != null;
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
                return ((Light)SelectedObject).state?.bri != null;
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
                return ((Light)SelectedObject).state?.ct != null;
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
                return ((Light)SelectedObject).state?.sat != null;
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
                return ((Light)SelectedObject).state?.xy != null;
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
            return SelectedObject is Light || SelectedObject is Group;
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
        //  public ICommand CreateAnimationCommand => new RelayCommand(param => CreateAnimation());
        public ICommand CreateHotKeyCommand => new RelayCommand(param => CreateHotKey(), (param) => EnableButtons());
        public ICommand CreateResourceLinkCommand => new RelayCommand(param => CreateResourceLink(), (param) => EnableButtons());
        public ICommand AllOnCommand => new RelayCommand(param => AllOn(), (param) => EnableButtons());
        public ICommand AllOffCommand => new RelayCommand(param => AllOff(), (param) => EnableButtons());
        public ICommand ShowEventLogCommand => new RelayCommand(param => ShowEventLog());
        public ICommand SearchNewLightsCommand => new RelayCommand(param => CheckForNewBulb(), (param) => EnableButtons() && CanSearchNewLights());
        public ICommand SearchNewSensorsCommand => new RelayCommand(param => SearchNewSensors(), (param) => EnableButtons() && CanSearchNewSensor());

        //*************** Sliders Commands ********************
        public ICommand SliderHueChangedCommand => new RelayCommand(param => SliderChangeHue(), (param) => CanHue());
        public ICommand SliderBriChangedCommand => new RelayCommand(param => SliderChangeBri(), (param) => CanBri());
        public ICommand SliderCtChangedCommand => new RelayCommand(param => SliderChangeCt(), (param) => CanCt());
        public ICommand SliderSatChangedCommand => new RelayCommand(param => SliderChangeSat(), (param) => CanSat());
        public ICommand SliderXyChangedCommand => new RelayCommand(param => SliderChangeXy(), (param) => CanXy());

        //*************** App Menu Commands ******************
        public ICommand DoBridgePairingCommand => new RelayCommand(param => DoBridgePairing(ListBridges));

        //*************** Context Menu Commands *************
        public ICommand DeleteObjectCommand => new RelayCommand(param => DeleteObject(), (param) => IsObjectSelected());
        public ICommand RenameObjectCommand => new RelayCommand(param => RenameObject(), (param) => IsObjectSelected());
        public ICommand EditObjectCommand => new RelayCommand(param => EditObject(), (param) => IsEditable());
        public ICommand IdentifyLongCommand => new RelayCommand(param => Identify("lselect"), (param) => CanIdentify());
        public ICommand IdentifyShortCommand => new RelayCommand(param => Identify("select"), (param) => CanIdentify());
        public ICommand ReplaceCurrentStateCommand => new RelayCommand(param => ReplaceCurrentState(), (param) => CanReplaceState());
        public ICommand SensitivityHighCommand => new RelayCommand(param => Sensitivity(2), (param) => CanSetSensivity());
        public ICommand SensitivityMediumCommand => new RelayCommand(param => Sensitivity(1), (param) => CanSetSensivity());
        public ICommand SensitivityLowCommand => new RelayCommand(param => Sensitivity(0), (param) => CanSetSensivity());
        public ICommand CloneCommand => new RelayCommand(param => Clone(false),(param) => CanClone());
        public ICommand QuickCloneCommand => new RelayCommand(param => Clone(true), (param) => CanClone());
        public ICommand IdentifyStopCommand => new RelayCommand(param => Identify("none"), (param) => CanIdentify());
        public ICommand CopyToJsonCommand => new RelayCommand(param => CopyToJson(false), (param) => IsObjectSelected());
        public ICommand CopyToJsonRawCommand => new RelayCommand(param => CopyToJson(true), (param) => IsObjectSelected());

        //*************** ListView Commands ********************
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => DoubleClickObject(), (param) => IsDoubleClickable());

        //*************** Views Commands ************************
        public ICommand ViewSceneMappingCommand => new RelayCommand(param => ViewSceneMapping(), (param) => EnableButtons());
        public ICommand ViewBulbsCommand => new RelayCommand(param => ViewBulbs(), (param) => EnableButtons());
        public ICommand ViewGroupsCommand => new RelayCommand(param => ViewGroups(), (param) => EnableButtons());
        public ICommand SortListViewCommand => new RelayCommand(param => SortListView(), (param) => EnableButtons());

        //*************** Toolbar ******************************

        public ICommand CpuTempMonCommand => new RelayCommand(param => RunCpuTempMon(), (param) => EnableButtons());

  //      public ICommand RssFeedMonCommand => new RelayCommand(param => RunRssFeedMon(), (param) => EnableButtons());
  //      public ICommand CpuTempMonSettingsCommand => new RelayCommand(param => CpuTempMonSettings(), (param) => EnableButtons());
   //     public ICommand RssFeedMonSettingsCommand => new RelayCommand(param => RssFeedMonSettings(), (param) => EnableButtons());
     //   public ICommand ClapperCommand => new RelayCommand(param => Clapper(), (param) => EnableButtons());
    }
}
