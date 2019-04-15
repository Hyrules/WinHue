using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Interface;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Resources;
using WinHue3.Utils;
using WinHue3.Validations;


namespace WinHue3.Functions.BridgePairing
{
    public class BridgePairingViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BridgePairingModel _bridgePairModel;
        private Bridge _selectedBridge;
        private readonly DispatcherTimer _pairTimer = new DispatcherTimer();
        private string _scanButtonText;
        private ObservableCollection<Bridge> _listBridges;


        public BridgePairingViewModel()
        {
            _bridgePairModel = new BridgePairingModel();
            _pairTimer = new DispatcherTimer() {Interval = new TimeSpan(0, 0, 0, 1) };
            _pairTimer.Tick += _pairTimer_Tick;
            Hue.OnIpScanComplete += Hue_OnIpScanComplete;
            Hue.OnIpScanProgressReport += Hue_OnIpScanProgressReport;
            Hue.OnDetectionComplete += Hue_OnDetectionComplete;
            Hue.OnBridgeDetectionFailed += Hue_OnBridgeDetectionFailed;
            _scanButtonText = GUI.BridgeDetectionPairing_Scan;
            _listBridges = new ObservableCollection<Bridge>();

        }

        public ObservableCollection<Bridge> ListBridges
        {
            get => _listBridges;
            set
            {
                SetProperty(ref _listBridges, value);
                RaisePropertyChanged("DefaultSet");
                RaisePropertyChanged("AnyPaired");
            }
        } 

        [BoolValueValidation(true,ErrorMessageResourceName = "BridgeDetectionPairing_SetDefault", ErrorMessageResourceType = typeof(GlobalStrings))]
        public bool DefaultSet => ListBridges.Any(x => x.IsDefault);

        [BoolValueValidation(true, ErrorMessageResourceName = "BridgeDetectionPairing_PairNeeded", ErrorMessageResourceType = typeof(GlobalStrings))]
        public bool AnyPaired => ListBridges.Any(x => x.ApiKey != string.Empty);

        private void Hue_OnBridgeDetectionFailed(object sender, DetectionErrorEventArgs e)
        {
            Cursor_Tools.ShowNormalCursor();
            MessageBox.Show("Error detecting bridge. Try manual scan.", GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            log.Error("Bridge Detection Failed.");
        }

        private void Hue_OnDetectionComplete(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if(e.Result != null)
            { 
                Dictionary<string, Bridge> brlist = (Dictionary<string, Bridge>)e.Result;
                UpdateBridgeList(brlist);
                log.Info($"Bridge Dectected : {brlist.Count}");
                log.Info("Bridge detection complete.");                
            }

            Cursor_Tools.ShowNormalCursor();
            CommandManager.InvalidateRequerySuggested();
        }

        private void Hue_OnIpScanProgressReport(object sender, IpScanProgressEventArgs e)
        {
            BridgePairModel.ProgressBarValue = e.Progress;
            log.Debug("Hue IP scan progress report : " + e.Progress);
        }

        private void Hue_OnIpScanComplete(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Result != null)
                {
                    BridgePairModel.UserMessage = GlobalStrings.BridgeDetectionPairing_ScanComplete;
                    Dictionary<string, Bridge> brlist = (Dictionary<string, Bridge>) e.Result;
                    UpdateBridgeList(brlist);
                    log.Info("Scan for bridge completed.");
                }

            }
            else
            {
                BridgePairModel.UserMessage = GlobalStrings.BridgeDetectionPairing_ScanAborted;
                log.Info("Scan for bridge aborted.");
            }
            CommandManager.InvalidateRequerySuggested();
            ScanButtonText = GUI.BridgeDetectionPairing_Scan;
        }

        private void _pairTimer_Tick(object sender, EventArgs e)
        {
            BridgePairModel.ProgressBarValue++;
            
            if (BridgePairModel.ProgressBarValue.Equals(BridgePairModel.ProgressBarMax))
            {
                StopPairTimer();
            }
            else
            {
                string bresult = _selectedBridge.CreateUser("WinHue");
                if (bresult == null) return;
                
                log.Info("Bridge Pairing complete.");
                SelectedBridge.ApiKey = bresult;
                BridgePairModel.UserMessage = GlobalStrings.BridgeDetectionPairing_PairingDone;
                StopPairTimer();
                RaisePropertyChanged("AnyPaired");
            }
        }

        public BridgePairingModel BridgePairModel
        {
            get => _bridgePairModel;
            set => SetProperty(ref _bridgePairModel,value);
        }

        public Bridge SelectedBridge
        {
            get => _selectedBridge;
            set => SetProperty(ref _selectedBridge, value);
        }

        public string ScanButtonText
        {
            get => _scanButtonText;
            set => SetProperty(ref _scanButtonText, value);
        }

        private void PairBridge()
        {
            if (!_pairTimer.IsEnabled)
            {
                StartPairTimer();
            }
            else
            {
                StopPairTimer();
            }
        }

        private void StopPairTimer()
        {
            
            BridgePairModel.ProgressBarValue = BridgePairModel.ProgressBarMax;
            _pairTimer.Stop();
            
        }

        private void StartPairTimer()
        {
            BridgePairModel.ProgressBarValue = BridgePairModel.ProgressBarMin;
            BridgePairModel.UserMessage = GlobalStrings.BridgeDetectionPairing_Pairing;
            _pairTimer.Start();

        }

        private void ScanForBridge()
        {
            

            if (!Hue.IsScanningForBridge)
            {
                BridgePairModel.ProgressBarMin = 2;
                BridgePairModel.ProgressBarValue = 2;
                BridgePairModel.ProgressBarMax = 254;              
                Hue.ScanIpForBridge();
                log.Info("Start scan for bridge.");
                ScanButtonText = GUI.BridgeDetectionPairing_Abort;
            }
            else
            {
                Hue.AbortScanForBridge();
                BridgePairModel.ProgressBarValue = BridgePairModel.ProgressBarMax;
                log.Info("Aborted scan for bridge.");
                ScanButtonText = GUI.BridgeDetectionPairing_Scan;
            }

        }

        private void SetDefaultBridge()
        {
            foreach (Bridge br in ListBridges)
            {
                br.IsDefault = false;
            }
            SelectedBridge.IsDefault = true;
            RaisePropertyChanged("DefaultSet");
            log.Info($@"Settings default bridge to {_selectedBridge}");

        }

        private void UpdateBridgeList(Dictionary<string, Bridge> brlist)
        {
            foreach (KeyValuePair<string, Bridge> kvp in brlist)
            {
                if (WinHueSettings.settings.CheckForBridgeUpdate)
                {
                    kvp.Value.RequiredUpdate = UpdateManager.Instance.CheckBridgeNeedUpdate(kvp.Value.ApiVersion);
                }
                else
                {
                    kvp.Value.RequiredUpdate = false;
                }

                if (ListBridges.Any(x => (x.Mac == kvp.Value.Mac) && (Equals(x.IpAddress, kvp.Value.IpAddress)))) continue;
                if (ListBridges.Any(x => (x.Mac == kvp.Value.Mac) && !(Equals(x.IpAddress, kvp.Value.IpAddress))))
                {
                    Bridge firstOrDefault = ListBridges.FirstOrDefault(x => x.Mac == kvp.Value.Mac);
                    if (firstOrDefault == null ||
                        MessageBox.Show(string.Format(GlobalStrings.Bridge_IP_Different, firstOrDefault.Name),
                            GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                        MessageBoxResult.Yes) continue;
                    int index = ListBridges.IndexOf(firstOrDefault);
                    ListBridges[index].IpAddress = kvp.Value.IpAddress;
                }
                else
                {
                    ListBridges.Add(kvp.Value);
                }
            }
        }

        private void DetectBridge()
        {
            Cursor_Tools.ShowWaitCursor();
            Hue.DetectBridge();
        }

        public void AddManualIp()
        {
            Form_AddManualIp fip = new Form_AddManualIp();
            if (fip.ShowDialog() == true)
            {
                Bridge bridge = new Bridge(){IpAddress = IPAddress.Parse(fip.GetIPAddress()) };
                switch (AddManualBridge(bridge))
                {
                    case AddManualBridgeResult.Success:
                        log.Info($"Adding manual ip {fip.TbIpAddress}");
                        break;
                    case AddManualBridgeResult.Alreadyexists:
                        MessageBox.Show(GlobalStrings.Bridge_Already_Detected, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        log.Error($"ip {fip.TbIpAddress} already exists.");
                        break;
                    case AddManualBridgeResult.NotResponding:
                        MessageBox.Show(GlobalStrings.Error_Bridge_Not_Responding, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        log.Error($"Bridge at ip {fip.TbIpAddress} is not responding.");
                        break;
                    default:
                        MessageBox.Show(GlobalStrings.Error_ErrorHasOccured, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        log.Error($"Unknown error at ip {fip.TbIpAddress}");
                        break;
                }

            }
        }

        public enum AddManualBridgeResult
        {
            Success,
            Alreadyexists,
            NotResponding,
            UnknownError
        }

        /// <summary>
        /// Add a bridge manually.
        /// </summary>
        /// <param name="bridge">Nwew bridge to add.</param>
        /// <returns>AddManualBridgeResult</returns>
        private AddManualBridgeResult AddManualBridge(Bridge bridge)
        {
            Bridge newBridge = bridge;
            BasicConfig bresult = newBridge.GetBridgeBasicConfig();
            if (bresult == null)
            {
                return newBridge.LastCommandMessages.LastError.type == -1 ? AddManualBridgeResult.NotResponding : AddManualBridgeResult.UnknownError;
            }

            newBridge.Mac = bresult.mac;
            newBridge.SwVersion = bresult.swversion;
            newBridge.ApiVersion = bresult.apiversion;

            newBridge.RequiredUpdate = WinHueSettings.settings.CheckForBridgeUpdate && UpdateManager.Instance.CheckBridgeNeedUpdate(newBridge.ApiVersion);

            if (ListBridges.Any(x => x.Mac == newBridge.Mac))
            {
                return AddManualBridgeResult.Alreadyexists;
            }

            ListBridges.Add(newBridge);

            return AddManualBridgeResult.Success;

        }

        public bool CanPair()
        {
            return SelectedBridge != null && !_pairTimer.IsEnabled && !Hue.IsDetectingBridge && !Hue.IsScanningForBridge;
        }

        public bool CanScan()
        {
            return !Hue.IsDetectingBridge;
        }

        public bool CanSetDefault()
        {
            return SelectedBridge != null && SelectedBridge.IsDefault == false && !_pairTimer.IsEnabled;
        }

        public bool CanDetectBridge()
        {
            return !_pairTimer.IsEnabled && !Hue.IsScanningForBridge;
        }

        public ICommand PairBridgeCommand => new RelayCommand(param => PairBridge(), (param) => CanPair() && CanDetectBridge() && CanScan());
        public ICommand ScanForBridgeCommand => new RelayCommand(param => ScanForBridge(), (param)=> CanScan());
        public ICommand SetDefaultBridgeCommand => new RelayCommand(param => SetDefaultBridge(), (param) => CanSetDefault());
        public ICommand DetectBridgeCommand => new RelayCommand(param => DetectBridge(), (param) => CanDetectBridge() && CanScan());
        public ICommand AddManualIpCommand => new RelayCommand(param => AddManualIp(), (param) => CanDetectBridge() && CanScan());
    }
}
