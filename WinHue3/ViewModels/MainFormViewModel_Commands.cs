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
        private bool EnableButtons()
        {
            return SelectedBridge != null;
        }

        private bool IsObjectSelected()
        {
            return SelectedObject != null;
        }

        private bool IsEditable()
        {
            return IsObjectSelected() && !(SelectedObject is Light);
        }

        private bool CanSchedule()
        {
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

        private bool SliderEnabled()
        {
            return SelectedObject is Light || SelectedObject is Group;
        }

        private bool CanHue()
        {
            if (!IsObjectSelected()) return false;
            if (SelectedObject is Light)
            {
                return ((Light) SelectedObject).state.hue != null;
            }
            else if (SelectedObject is Group)
            {
                return ((Group)SelectedObject).action.hue != null;
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

        public ICommand InitializeCommand => new RelayCommand(param => Initialize());

        //*************** Toolbar Commands ********************        
        public ICommand CheckForNewBulbCommand => new RelayCommand(param => CheckForNewBulb(), (param) => EnableButtons());
        public ICommand UpdateBridgeCommand => new RelayCommand(param => DoBridgeUpdate());
    //    public ICommand ChangeBridgeSettingsCommand => new RelayCommand(param => ChangeBridgeSettings(), (param) => EnableButtons());
        public ICommand RefreshViewCommand => new RelayCommand(param => RefreshView(), (param) => EnableButtons());
        public ICommand CreateGroupCommand => new RelayCommand(param => CreateGroup(), (param) => EnableButtons());
        public ICommand CreateSceneCommand => new RelayCommand(param => CreateScene(), (param) => EnableButtons());
        public ICommand CreateScheduleCommand => new RelayCommand(param => CreateSchedule(), (param) => EnableButtons() && CanSchedule() && IsObjectSelected());
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
        public ICommand SliderHueChangedCommand => new RelayCommand(param => SliderChangeHue(), (param) => EnableButtons() && CanHue());
        public ICommand SliderBriChangedCommand => new RelayCommand(param => SliderChangeBri(), (param) => EnableButtons() && SliderEnabled());
        public ICommand SliderCtChangedCommand => new RelayCommand(param => SliderChangeCt(), (param) => EnableButtons() && SliderEnabled());
        public ICommand SliderSatChangedCommand => new RelayCommand(param => SliderChangeSat(), (param) => EnableButtons() && SliderEnabled());
        public ICommand SliderXyChangedCommand => new RelayCommand(SliderChangeXy, (param) => EnableButtons() && SliderEnabled());

        //*************** App Menu Commands ******************
        public ICommand DoBridgePairingCommand => new RelayCommand(param => DoBridgePairing(ListBridges));

        //*************** Context Menu Commands *************
        public ICommand DeleteObjectCommand => new RelayCommand(param => DeleteObject(), (param) => IsObjectSelected());
        public ICommand RenameObjectCommand => new RelayCommand(param => RenameObject(), (param) => IsObjectSelected());
        public ICommand EditObjectCommand => new RelayCommand(param => EditObject(), (param) => IsObjectSelected() && IsEditable());
        public ICommand IdentifyLongCommand => new RelayCommand(param => IdentifyLong(), (param) => CanIdentify());
        public ICommand IdentifyShortCommand => new RelayCommand(param => IdentifyShort(), (param) => CanIdentify());
  //      public ICommand ReplaceCurrentStateCommand => new RelayCommand(param => ReplaceCurrentState());
        public ICommand SensitivityHighCommand => new RelayCommand(param => Sensitivity(2), (param) => CanSetSensivity());
        public ICommand SensitivityMediumCommand => new RelayCommand(param => Sensitivity(1), (param) => CanSetSensivity());
        public ICommand SensitivityLowCommand => new RelayCommand(param => Sensitivity(0), (param) => CanSetSensivity());
        public ICommand DuplicateObjectCommand => new RelayCommand(param => DuplicateObject());
        //*************** ListView Commands ********************
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => DoubleClickObject(), (param) => IsDoubleClickable());

        //*************** View Commands ************************

   //     public ICommand ViewSceneMappingCommand => new RelayCommand(param => ViewSceneMapping(), (param) => EnableButtons());
  //      public ICommand ViewBulbsCommand => new RelayCommand(param => ViewBulbs(), (param) => EnableButtons());
   //     public ICommand ViewGroupsCommand => new RelayCommand(param => ViewGroups(), (param) => EnableButtons());

        //*************** Toolbar ******************************

  //      public ICommand CpuTempMonCommand => new RelayCommand(param => RunCpuTempMon(), (param) => EnableButtons());
  //      public ICommand RssFeedMonCommand => new RelayCommand(param => RunRssFeedMon(), (param) => EnableButtons());
  //      public ICommand CpuTempMonSettingsCommand => new RelayCommand(param => CpuTempMonSettings(), (param) => EnableButtons());
   //     public ICommand RssFeedMonSettingsCommand => new RelayCommand(param => RssFeedMonSettings(), (param) => EnableButtons());
     //   public ICommand ClapperCommand => new RelayCommand(param => Clapper(), (param) => EnableButtons());
    }
}
