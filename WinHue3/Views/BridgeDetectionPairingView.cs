using System;
using System.Net;
using System.Windows.Input;
using System.Windows.Threading;
using WinHue3.Resources;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using HueLib2;

namespace WinHue3
{
    public class BridgeDetectionPairingView : View
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Bridge _selectedBridge;
        private string _message = GlobalStrings.BridgeDetectionPairing_SelectionMessage;
        private double _pbmin = 0;
        private double _pbmax = 60;
        private readonly DispatcherTimer _pairTimer = new DispatcherTimer();
        private readonly DispatcherTimer _timeoutTimer = new DispatcherTimer();
        private double _pbvalue = 0;
        private bool _canPair = false;
        private bool _canChangeSelection = true;
        private string _scanbuttontext = GUI.BridgeDetectionPairing_Scan;
        private bool _aborted = false;
        private bool _canscan = false;
        private ObservableCollection<Bridge> _listBridge;
        private bool _canaddip = false;
        private bool _defaultset = false;

        #region CTOR

        public BridgeDetectionPairingView()
        {
            _pairTimer.Interval = new TimeSpan(0, 0, 0, 2);
            _pairTimer.Tick += _pairTimer_Tick;
            _timeoutTimer.Interval = new TimeSpan(0, 0, 1, 0);
            _timeoutTimer.Tick += _timeoutTimer_Tick;
            _listBridge = new ObservableCollection<Bridge>();

            if (_listBridge.Count == 0)
            {
                CanAddManualIp = false;
                Cursor_Tools.ShowWaitCursor();
                Hue.OnDetectionComplete += Hue_OnDetectionComplete;
                Hue.OnBridgeDetectionFailed += Hue_OnBridgeDetectionFailed;
                log.Info("Starting bridge detection.");
                Hue.DetectBridge();
                
                _canscan = false;
                
            }               
            else
            {
                AssociateApiKey(_listBridge);
            }
        }

        #endregion

        #region PROPERTIES

        public bool CanDone
        {
            get
            {
                return _defaultset;                
            }

        }

        public bool CanAddManualIp
        {
            get
            {
                return _canaddip;
            }
            set
            {
                _canaddip = value;
                OnPropertyChanged();
            }
        }

        public bool EnableDefault
        {
            get
            {
                if (_selectedBridge == null) return false;
                return !_selectedBridge.IsDefault;
            }
        }

        public ObservableCollection<Bridge> ListViewSource
        {
            get { return _listBridge; }
            set
            {
                _listBridge = value;
                foreach (Bridge br in ListViewSource)
                {
                    log.Debug("ListViewSource : " + br);
                }              
                OnPropertyChanged();
                OnPropertyChanged("CanScan");
            }
        }

        public bool CanChangeSelection
        {
            get { return _canChangeSelection;}
            internal set
            {
                _canChangeSelection = value;
                log.Debug("CanChangeSelection : " + value);
                OnPropertyChanged();
            }
        }

        public double ProgressBarValue
        {
            get { return _pbvalue; }
            internal set
            {
                _pbvalue = value;
                log.Debug("ProgressBarValue : " + value);
                OnPropertyChanged();
            }
        }

        public Bridge SelectedBridge
        {
            get { return _selectedBridge;}
            set
            {
                _selectedBridge = value;
                log.Debug("SelectedBridge : " + value);
                if (value != null)
                {
                    if (value.ApiKey == null)
                        CanPair = true;
                    else
                        CanPair = value.ApiKey == string.Empty;
                }
                else
                    CanPair = false;
                OnPropertyChanged();
                OnPropertyChanged("EnableDefault");
                
            }
        }

        public bool CanPair
        {
            get
            {
                return _canPair;
            }
            internal set
            {
                _canPair = value;
                log.Debug("CanPair : " + value);
                OnPropertyChanged();

            }
        }

        public string UserMessage
        {
            get { return _message; }
            internal set
            {
                _message = value;
                log.Debug("UserMessage : " + value);
                OnPropertyChanged();
            }
        }

        public double ProgressBarMax
        {
            get { return _pbmax; }
            internal set
            {
                _pbmax = value;
                log.Debug("ProgressBarMax" + value);
                OnPropertyChanged();
            }
        }

        public double ProgressBarMin
        {
            get { return _pbmin; }
            internal set
            {
                _pbmin = value;
                log.Debug("ProgressBarMin : " + value);
                OnPropertyChanged();
            }
        }

        public string ScanButtonText
        {
            get { return _scanbuttontext; }
            set
            {
                _scanbuttontext = value;
                log.Debug("ScanButtonText : " + value);
                OnPropertyChanged();
            }
        }

        public bool CanScan => ListViewSource.Count == 0 & _canscan;

        #endregion
        
        #region COMMANDS

        public ICommand PairBridgeCommand => new RelayCommand(param => PairBridge());
        public ICommand ScanForBridgeCommand => new RelayCommand(param => ScanForBridge());
        public ICommand SetDefaultBridgeCommand => new RelayCommand(param => SetDefaultBridge());
        public ICommand AddManualIPCommand => new RelayCommand(param => AddManualIP());
        #endregion

        #region METHODS

        private void AddManualIP()
        {
            Form_AddManualIp fip = new Form_AddManualIp();
            if(fip.ShowDialog() == true)
            {
                BasicConfig bridgeconfig = Hue.GetBridgeBasicConfig(IPAddress.Parse(fip.GetIPAddress()));
                if (bridgeconfig != null)
                {
                    Bridge br = new Bridge()
                    {
                        IpAddress = IPAddress.Parse(fip.GetIPAddress()),
                        Mac = bridgeconfig.mac,
                        SwVersion = bridgeconfig.swversion,
                    };
                    if(!_listBridge.Any(x =>x.IpAddress.Equals(br.IpAddress)))
                        _listBridge.Add(br);
                    else
                    {
                        MessageBox.Show(GlobalStrings.Bridge_Already_Detected, GlobalStrings.Error, MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                }
                else
                {

                    MessageBox.Show(GlobalStrings.Error_Getting_Bridge_Basic_Config, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PairBridge()
        {
            ProgressBarMin = 0;
            ProgressBarValue = 0;
            ProgressBarMax = 60;
            UserMessage = GlobalStrings.BridgeDetectionPairing_Pairing;
            CanPair = false;
            CanAddManualIp = false;
            CanChangeSelection = false;
            _pairTimer.Start();
            _timeoutTimer.Start();   
            log.Info("Start pairing bridge.");         
        }

        private void ScanForBridge()
        {
            if (!Hue.IsScanningForBridge)
            {
                ProgressBarMin = 2;
                ProgressBarValue = 2;
                ProgressBarMax = 254;
                ScanButtonText = GUI.BridgeDetectionPairing_Abort;
                Hue.OnIpScanComplete += Hue_OnIpScanComplete;
                Hue.OnIpScanProgressReport += Hue_OnIpScanProgressReport;
                UserMessage = GlobalStrings.BridgeDetectionPairing_Scanning;
                Hue.ScanIpForBridge();
                _aborted = false;
                log.Info("Start scan for bridge.");
                CanAddManualIp = false;
            }
            else
            {
                Hue.AbortScanForBridge();
                ProgressBarValue = ProgressBarMax;
                UserMessage = GlobalStrings.BridgeDetectionPairing_ScanAborted;
                _aborted = true;
                log.Info("Aborted scan for bridge.");
            }
        }

        private bool SetDefaultBridge()
        {
            bool result = false;
            foreach (Bridge br in _listBridge)
            {
                br.IsDefault = false;
            }
            _selectedBridge.IsDefault = true;
            _defaultset = true;
            OnPropertyChanged("IsDefault");
            OnPropertyChanged("EnableDefault");
            OnPropertyChanged("CanDone");
            log.Info($@"Settings default bridge to {_selectedBridge}");
            return result;
        }


        private void Hue_OnBridgeDetectionFailed(object sender, DetectionErrorEventArgs e)
        {
            Cursor_Tools.ShowNormalCursor();
            _canscan = true;
            OnPropertyChanged("CanScan");
            CanAddManualIp = true;
            MessageBox.Show("Error detecting bridge. Try manual scan.", GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            log.Error("Error detecting bridge.", e.Error);
        }

        private ObservableCollection<Bridge> AssociateApiKey(ObservableCollection<Bridge> bridge)
        {
            foreach (Bridge br in bridge)
            {
                
                if (!WinHueSettings.settings.BridgeInfo.ContainsKey(br.Mac)) continue;
                br.ApiKey = WinHueSettings.settings.BridgeInfo[br.Mac].apikey;
                log.Debug($@"Associating ApiKey :{br.ApiKey} with bridge : {br.IpAddress}");
                HelperResult hr = HueObjectHelper.IsAuthorized(br);
                if (hr.Success)
                {
                    br.ApiKey = string.Empty;                    
                }
                else
                {
                    br.ApiKey = string.Empty;
                    MessageBoxError.ShowLastErrorMessages(br);
                }
                br.IsDefault = br.Mac == WinHueSettings.settings.DefaultBridge;
                _defaultset |= br.IsDefault;
                OnPropertyChanged("CanDone");
            }
            return bridge;
        }


        private void Hue_OnDetectionComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            ListViewSource = AssociateApiKey((ObservableCollection<Bridge>)e.Result);
            Cursor_Tools.ShowNormalCursor();
            _canscan = true;
            OnPropertyChanged("CanScan");
            CanAddManualIp = true;
            log.Info("Bridge detection complete.");
            log.Debug(ListViewSource);

        }

        private void _timeoutTimer_Tick(object sender, EventArgs e)
        {
            ProgressBarValue += 2;
            _pairTimer.Stop();
            _timeoutTimer.Stop();
            CanPair = true;
            CanAddManualIp = true;
            CanChangeSelection = true;
            log.Debug("Timer timout.");
        }

        private void _pairTimer_Tick(object sender, EventArgs e)
        {
            CommandResult bresult = _selectedBridge.CreateUser("WinHue");
            string result = string.Empty;
            if (bresult.Success)
            {
                result = (string) bresult.resultobject;
            }
            
            ProgressBarValue += 2;
            if (result == string.Empty) return;
            ProgressBarValue = ProgressBarMax;
            ListViewSource[ListViewSource.IndexOf(_selectedBridge)].ApiKey = result;
            UserMessage = GlobalStrings.BridgeDetectionPairing_PairingDone;
            if (_listBridge.Count == 1)
            {
                SetDefaultBridge();
            }
            CanChangeSelection = true;
            _pairTimer.Stop();
            _timeoutTimer.Stop();
 
        }

        private void Hue_OnIpScanProgressReport(object sender, IpScanProgressEventArgs e)
        {
            ProgressBarValue = e.Progress;
            log.Debug("Hue IP scan progress report : " + e.Progress);
        }

        private void Hue_OnIpScanComplete(object sender, EventArgs e)
        {
            if (!_aborted)
            {
                UserMessage = GlobalStrings.BridgeDetectionPairing_ScanComplete;
                ListViewSource = (ObservableCollection<Bridge>)((RunWorkerCompletedEventArgs)e).Result;
            }
            ScanButtonText = GUI.BridgeDetectionPairing_Scan;
            log.Info("Scan for bridge completed.");
            CanAddManualIp = true;
        }

        public bool SaveSettings()
        {
            foreach (Bridge br in ListViewSource)
            {
                if (WinHueSettings.settings.BridgeInfo.ContainsKey(br.Mac))
                    WinHueSettings.settings.BridgeInfo[br.Mac] = new BridgeSaveSettings()
                    {
                        ip = br.IpAddress.ToString(),
                        apikey = br.ApiKey,
                        apiversion = br.ApiVersion,
                        swversion = br.SwVersion,
                        name = br.Name
                    };
                else
                    WinHueSettings.settings.BridgeInfo.Add(br.Mac,
                        new BridgeSaveSettings() {ip = br.IpAddress.ToString(), apikey = br.ApiKey, apiversion = br.ApiVersion, swversion = br.SwVersion,name = br.Name});

                if (br.IsDefault) WinHueSettings.settings.DefaultBridge = br.Mac;
            }
            
            return WinHueSettings.Save();
        }

        #endregion

        #region EVENTS

        #endregion

    }

}