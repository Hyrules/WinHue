using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using WinHue3.Addons.CpuTempMon;
using WinHue3.Hotkeys;
using WinHue3.Models;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Settings;
using WinHue3.Utils;


namespace WinHue3.ViewModels.MainFormViewModels
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ObservableCollection<IHueObject> _listBridgeObjects;
        private readonly DispatcherTimer _findlighttimer = new DispatcherTimer();
        private readonly DispatcherTimer _findsensortimer = new DispatcherTimer();
        private readonly List<HotKeyHandle> _lhk;
        private List<HotKey> _listHotKeys;
        private string _lastmessage = string.Empty;
        private MainFormModel _mainFormModel;
        private CpuTempMonitor _ctm;
        private DispatcherTimer _ledTimer;
        private bool _hotkeyDetected;

        public MainFormViewModel()
        {
            _ledTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 2)
                
            };
            _hotkeyDetected = false;
            _ledTimer.Tick += _ledTimer_Tick;
            _lhk = new List<HotKeyHandle>();
            _listBridgeObjects = new ObservableCollection<IHueObject>();
            _listBridges = new ObservableCollection<Bridge>();
            _findlighttimer.Interval = new TimeSpan(0, 1, 0);
            _findlighttimer.Tick += _findlighttimer_Tick;
            _findsensortimer.Interval = new TimeSpan(0, 1, 0);
            _findsensortimer.Tick += _findsensortimer_Tick;
            _listHotKeys = WinHueSettings.hotkeys.listHotKeys;
            _mainFormModel = new MainFormModel();
            SliderTt = WinHueSettings.settings.DefaultTT;

            Comm.Timeout = WinHueSettings.settings.Timeout;
            MainFormModel.Sort = WinHueSettings.settings.Sort;
            MainFormModel.ShowId = WinHueSettings.settings.ShowID;
            MainFormModel.WrapText = WinHueSettings.settings.WrapText;

        }

        private void _ledTimer_Tick(object sender, EventArgs e)
        {
            _ledTimer.Stop();
            HotkeyDetected = false;
        }

        public MainFormModel MainFormModel
        {
            get => _mainFormModel;
            set => SetProperty(ref _mainFormModel,value);
        }

        public bool HotkeyDetected
        {
            get => _hotkeyDetected;
            set => SetProperty(ref _hotkeyDetected,value);
        }

        private bool CheckBridge(Bridge bridge)
        {
            bool bridgeready = false;
            log.Info("Checking if ip is bridge...");
            if (Hue.IsBridge(bridge.IpAddress))
            {
                log.Info("IP is bridge. Checking if bridge is authorized...");
                bridgeready = bridge.CheckAuthorization();
                log.Info($"Bridge authorization : {bridgeready}");
            }
            return bridgeready;
        }

        private void Initialize()
        {
            _eventlogform.Owner = Application.Current.MainWindow;

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
            LoadBridges();
        }

        private void LoadBridges()
        {
            while (true)
            {
                log.Info("Loading bridge(s)...");

                log.Info($"Checking if any bridge already present in settings... found {WinHueSettings.bridges.BridgeInfo.Count}");

                if (WinHueSettings.bridges.BridgeInfo.Count == 0)
                {   // No bridge found in the list of bridge.
                    log.Info("No bridge found in settings. Pairing needed.");
                    if (DoBridgePairing())
                        continue;
                    else
                        break;
                }

                log.Info($"Checking if there is a defaut bridge set...");
                if (WinHueSettings.bridges.DefaultBridge == string.Empty)
                {   // Default not set

                    log.Info("Default bridge not set. Pairing needed.");
                    if (DoBridgePairing())
                        continue;
                    else
                        break;
                }
                else
                {   // Default set
                    if (!WinHueSettings.bridges.BridgeInfo.ContainsKey(WinHueSettings.bridges.DefaultBridge))
                    {
                        // Default Bridge Exists
                        log.Info("Default Bridge does not seem to exist in the list of bridge. Pairing needed.");
                        if (DoBridgePairing())
                            continue;
                        else
                            break;
                    }

                }

                foreach (KeyValuePair<string, BridgeSaveSettings> b in WinHueSettings.bridges.BridgeInfo)
                {
                    log.Info($"Bridge OK. Checking if bridge already in the bridge list...");
                    if (ListBridges.All(x => x.Mac != b.Key))
                    {
                        Bridge bridge = new Bridge()
                        {
                            ApiKey = b.Value.apikey,
                            ApiVersion = b.Value.apiversion,
                            IpAddress = IPAddress.Parse(b.Value.ip),
                            name = b.Value.name,
                            IsDefault = b.Key == WinHueSettings.bridges.DefaultBridge,
                            SwVersion = b.Value.swversion,
                            Mac = b.Key
                        };
                        if (b.Value.apikey == string.Empty) continue;
                        bridge.BridgeNotResponding += Bridge_BridgeNotResponding;
                        bridge.LastCommandMessages.OnMessageAdded += Bridge_OnMessageAdded;
                        bridge.RequiredUpdate = WinHueSettings.settings.CheckForBridgeUpdate && UpdateManager.CheckBridgeNeedUpdate(bridge.ApiVersion);

                        log.Info("Bridge not in the list adding it...");
                        ListBridges.Add(bridge);

                    }
                    else
                    {
                        log.Info("Bridge already in the list skipping...");
                    }
                }

                foreach (Bridge br in ListBridges)
                {
                    log.Info($"Checking bridge {br}");
                    if (!CheckBridge(br))
                    {
                        log.Info("Bridge IP has changed... Pairing needed.");

                        if (DoBridgePairing(ListBridges))
                        {
                            if (br.IsDefault)
                            {
                                SelectedBridge = br;
                                SelectedBridge.CheckOnlineForUpdate();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (br.IsDefault)
                        {
                            SelectedBridge = br;
                            SelectedBridge.CheckOnlineForUpdate();
                        }
                            
                    }
                }

                if (SelectedBridge != null)
                {
                    _ctm = new CpuTempMonitor(SelectedBridge);
                    LoadHotkeys();
                }


                break;
            }

        }

    }
}
