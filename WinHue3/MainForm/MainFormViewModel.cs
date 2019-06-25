using System;
using System.Collections.ObjectModel;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using WinHue3.Addons.CpuTempMon;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.HotKeys;
using WinHue3.Utils;
using WinHue3.Functions.RoomMap;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.BridgeObject;
using MQTTnet.Client;
using MQTTnet;
using WinHue3.Philips_Hue.Communication2;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _lastmessage = string.Empty;
        private MainFormModel _mainFormModel;
        private CpuTempMonitor _ctm;
        private TaskbarIcon _tbt;
        private HotKeyManager _hkm;
        private IMqttClient _mqttClient;

        public MainFormViewModel()
        {
            HueHttpClient.OnCommunicationTimeOut += Comm_CommunicationTimedOut;
            HueHttpClient.Timeout = WinHueSettings.settings.Timeout;

            _mainFormModel = new MainFormModel();
            _sliderTT = WinHueSettings.settings.DefaultTT;            
            _mainFormModel.Sort = WinHueSettings.settings.Sort;
            _mainFormModel.ShowId = WinHueSettings.settings.ShowID;
            _mainFormModel.WrapText = WinHueSettings.settings.WrapText;
            _mainFormModel.Showhiddenscenes = WinHueSettings.settings.ShowHiddenScenes;
            _mainFormModel.ShowFloorPlanTab = WinHueSettings.settings.ShowFloorPlanTab;

            ListBridges = new ObservableCollection<Bridge>();
            CurrentBridgeHueObjectsList = new ObservableCollection<IHueObject>();
            _refreshTimer.Interval = new TimeSpan(0, 0, (int)WinHueSettings.settings.RefreshTime);
            _refreshTimer.Tick += _refreshTimer_Tick;
            _findlighttimer.Interval = new TimeSpan(0, 1, 0);
            _findlighttimer.Tick += _findlighttimer_Tick;
            _findsensortimer.Interval = new TimeSpan(0, 1, 0);
            _findsensortimer.Tick += _findsensortimer_Tick;
            _mqttClient = new MqttFactory().CreateMqttClient();
            LoadFloorPlans();
        }

        public void LoadFloorPlans()
        {
            SelectedFloorPlan = null;
            ListFloorPlans = new ObservableCollection<Floor>(WinHueSettings.LoadFloorPlans());

        }

        public void SetToolbarTray(TaskbarIcon tbt)
        {
            _tbt = tbt;
        }

        private void Comm_CommunicationTimedOut(object sender, EventArgs e)
        {
            MessageBox.Show("Not Responding");
        }

        public MainFormModel MainFormModel
        {
            get => _mainFormModel;
            set => SetProperty(ref _mainFormModel, value);
        }

        private void Initialize()
        {
            UpdateManager.Instance.CheckForWinHueUpdate();

            if (UpdateManager.Instance.UpdateAvailable)
            {
               
                if (WinHueSettings.settings.CheckForUpdate)
                {

                    if (MessageBox.Show(GlobalStrings.UpdateAvailableDownload, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        UpdateManager.Instance.DownloadUpdate();
                    }

                }
            }

            OnBridgeMessageAdded += Bridge_OnMessageAdded;
            OnBridgeNotResponding += Bridge_BridgeNotResponding;
            LoadBridges();

            if (SelectedBridge == null) return;
            _ctm = new CpuTempMonitor(SelectedBridge);
            _hkm = new HotKeyManager(SelectedBridge);
            _hkm.StartHotKeyCapture();
            CurrentView = 1;
        }



    }
}

