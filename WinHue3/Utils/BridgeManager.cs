using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.BridgeFinder;
using WinHue3.Functions.BridgePairing;
using WinHue3.Functions.Lights.SupportedDevices;
using WinHue3.Interface;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.Utils
{
    public static class BridgeManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static ObservableCollection<Bridge> _listBridges;
        private static Bridge _selectedBridge;

        #region EVENTS
        public static event BridgeRemoved OnBridgeRemoved;
        public delegate void BridgeRemoved(Bridge b);

        public static event BridgeAdded OnBridgeAdded;
        public delegate void BridgeAdded(Bridge b);

        public static event EventHandler OnBridgeLoaded;

        public static event Func<Bridge, Task> OnSelectedBridgeChanged;
        public delegate void BridgeSelected(object sender, Bridge b);

        public static event BridgeNotResponding OnBridgeNotResponding;
        public delegate void BridgeNotResponding(object sender, BridgeNotRespondingEventArgs e);

        public static event BridgeAddedMessage OnBridgeMessageAdded;
        public delegate void BridgeAddedMessage(object sender, MessageAddedEventArgs e);
        #endregion

        #region CTOR
        static BridgeManager()
        {
            _listBridges = new ObservableCollection<Bridge>(); 
        }

        public static Bridge SelectedBridge
        {
            get => _selectedBridge;
            set
            {
                _selectedBridge = value;
                OnSelectedBridgeChanged?.Invoke(value);
            }
        }
        #endregion

        #region METHODS

        public static ObservableCollection<IHueObject> LoadVirtualBridge()
        {
            System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = File.ReadAllText(fd.FileName);
                DataStore ds = JsonConvert.DeserializeObject<DataStore>(file);
                List<IHueObject> hueobjects = SelectedBridge.GetBridgeDataStoreAsync();
                Bridge vbridge = new Bridge() { Virtual = true, Name = "Virtual Bridge", RequiredUpdate = false };
                _listBridges.Add(vbridge);
                _selectedBridge = vbridge;
                return new ObservableCollection<IHueObject>(hueobjects);
            }
            return null;
        }

        public static void AddBridge(Bridge bridge)
        {
            _listBridges.Add(bridge);
            OnBridgeAdded?.Invoke(bridge);
        }

        public static void RemoveBridge(Bridge bridge)
        {
            _listBridges.Remove(bridge);
            OnBridgeRemoved?.Invoke(bridge);
        }

        public static bool DoBridgePairing()
        {
            Form_BridgeDetectionPairing dp = new Form_BridgeDetectionPairing(new ObservableCollection<Bridge>(_listBridges)) { Owner = Application.Current.MainWindow };
            bool result = dp.ShowDialog().GetValueOrDefault(false);
            if (!result) return result;
            _listBridges = dp.ViewModel.ListBridges;
            SaveSettings();
            return result;
        }

        private static bool CheckBridge(Bridge bridge)
        {
            log.Info("Checking if ip is bridge...");
            BasicConfig bc = bridge.GetBridgeBasicConfig();
            if (bc != null)
            {
                bridge.ApiVersion = bc.apiversion;
                bridge.Name = bc.name;
                bridge.SwVersion = bc.swversion;
                WinHueSettings.bridges.BridgeInfo[bridge.Mac].name = bridge.Name;
                WinHueSettings.SaveBridges();
                return true;
            }

            return false;
        }

        private static bool SaveSettings()
        {
            foreach (Bridge br in _listBridges)
            {
                if (br.Mac == string.Empty) continue;
                if (WinHueSettings.bridges.BridgeInfo.ContainsKey(br.Mac))
                    WinHueSettings.bridges.BridgeInfo[br.Mac] = new BridgeSaveSettings
                    {
                        ip = br.IpAddress.ToString(),
                        apikey = br.ApiKey,
                        name = br.Name,
                    };
                else
                    WinHueSettings.bridges.BridgeInfo.Add(br.Mac,
                        new BridgeSaveSettings { ip = br.IpAddress.ToString(), apikey = br.ApiKey, name = br.Name });

                if (br.IsDefault) WinHueSettings.bridges.DefaultBridge = br.Mac;
            }

            return WinHueSettings.SaveBridges();
        }

        public static void LoadBridges()
        {
            while (true)
            {
                log.Info("Loading bridge(s)...");

                log.Info($"Checking if any bridge already present in settings... found {WinHueSettings.bridges.BridgeInfo.Count}");

                if (WinHueSettings.bridges.BridgeInfo.Count == 0 || WinHueSettings.bridges.DefaultBridge == string.Empty || !WinHueSettings.bridges.BridgeInfo.ContainsKey(WinHueSettings.bridges.DefaultBridge))
                {   // No bridge found in the list of bridge.
                    log.Info("Either no bridge found in settings or no default bridge. Pairing needed.");
                    if (DoBridgePairing())
                        continue;
                    else
                        break;
                }

                foreach (KeyValuePair<string, BridgeSaveSettings> b in WinHueSettings.bridges.BridgeInfo)
                {
                    log.Info($"Bridge OK. Checking if bridge already in the bridge list...");
                    if (_listBridges.All(x => x.Mac != b.Key))
                    {
                        Bridge bridge = new Bridge()
                        {
                            ApiKey = b.Value.apikey,
                            IpAddress = IPAddress.Parse(b.Value.ip),
                            Name = b.Value.name,
                            IsDefault = b.Key == WinHueSettings.bridges.DefaultBridge,
                            Mac = b.Key
                        };
                        if (b.Value.apikey == string.Empty) continue;

                        bridge.LastCommandMessages.OnMessageAdded += LastCommandMessages_OnMessageAdded;
                        bridge.RequiredUpdate = WinHueSettings.settings.CheckForBridgeUpdate && UpdateManager.CheckBridgeNeedUpdate(bridge.ApiVersion);

                        log.Info("Bridge not in the list adding it...");
                        _listBridges.Add(bridge);

                    }
                    else
                    {
                        log.Info("Bridge already in the list skipping...");
                    }
                }

                foreach (Bridge br in _listBridges)
                {
                    log.Info($"Checking bridge {br}");
                    if (!CheckBridge(br))
                    {
                        log.Info("Bridge IP has changed... Pairing needed.");
                        Form_BridgeFinder fbf = new Form_BridgeFinder(br) { Owner = Application.Current.MainWindow };
                        fbf.ShowDialog();

                        if (fbf.IpFound())
                        {
                            br.BridgeNotResponding += Br_BridgeNotResponding;
                            br.IpAddress = fbf.newip;
                            if (!br.IsDefault) continue;
                            _selectedBridge = br;
                            OnSelectedBridgeChanged?.Invoke(_selectedBridge);
                        }
                        else
                        {
                            DoBridgePairing();
                            break;
                        }
                    }
                    else
                    {
                        if (!br.IsDefault) continue;
                        _selectedBridge = br;
                        OnSelectedBridgeChanged?.Invoke(_selectedBridge);
                    }
                }

                OnBridgeLoaded?.Invoke(null,null);
                break;
            }
        }

        private static void Br_BridgeNotResponding(object sender, BridgeNotRespondingEventArgs e)
        {
            OnBridgeNotResponding?.Invoke(sender, e);
        }

        private static void LastCommandMessages_OnMessageAdded(object sender, MessageAddedEventArgs e)
        {
            OnBridgeMessageAdded?.Invoke(sender, e);
        }
        #endregion
    }
}
