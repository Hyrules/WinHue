using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using WinHue3.Addons.CpuTempMon;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.HotKeys;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Utils;
using HotKey = WinHue3.Functions.HotKeys.HotKey;
using WinHue3.Functions.BridgeManager;
using WinHue3.Functions.RoomMap;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _lastmessage = string.Empty;
        private MainFormModel _mainFormModel;
        private CpuTempMonitor _ctm;

        private bool _hotkeyDetected;
        private TaskbarIcon _tbt;
                
        public MainFormViewModel()
        {
            Comm.CommunicationTimedOut += Comm_CommunicationTimedOut;
            Comm.Timeout = WinHueSettings.settings.Timeout;
            _hotkeyDetected = false;

            _mainFormModel = new MainFormModel();
            _sliderTT = WinHueSettings.settings.DefaultTT;            
            _mainFormModel.Sort = WinHueSettings.settings.Sort;
            _mainFormModel.ShowId = WinHueSettings.settings.ShowID;
            _mainFormModel.WrapText = WinHueSettings.settings.WrapText;
            _mainFormModel.Showhiddenscenes = WinHueSettings.settings.ShowHiddenScenes;
            _mainFormModel.ShowFloorPlanTab = WinHueSettings.settings.ShowFloorPlanTab;
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

        public bool HotkeyDetected
        {
            get => _hotkeyDetected;
            set => SetProperty(ref _hotkeyDetected, value);
        }

        private void Initialize()
        {
            UpdateManager.CheckForWinHueUpdate();

            if (UpdateManager.UpdateAvailable)
            {
                RaisePropertyChanged("AppUpdateAvailable");
                if (WinHueSettings.settings.CheckForUpdate)
                {

                    if (MessageBox.Show(GlobalStrings.UpdateAvailableDownload, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        UpdateManager.DownloadUpdate();
                    }

                }
            }

            BridgesManager.Instance.OnBridgeMessageAdded += Bridge_OnMessageAdded;
            BridgesManager.Instance.OnBridgeNotResponding += Bridge_BridgeNotResponding;
            BridgesManager.Instance.LoadBridges();

            if (BridgesManager.Instance.SelectedBridge != null)
            {
                _ctm = new CpuTempMonitor(BridgesManager.Instance.SelectedBridge);
                HotKeyManager.Instance.StartHotKeyCapture();
            }
        }



    }
}

