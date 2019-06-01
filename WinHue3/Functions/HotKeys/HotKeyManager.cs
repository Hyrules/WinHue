using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;

namespace WinHue3.Functions.HotKeys
{
    public sealed class HotKeyManager : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<HotKeyHandle> _lhk;
        private ObservableCollection<HotKey> _listHotKeys;
        private readonly DispatcherTimer _ledTimer;
        private bool _hotkeyDetected;
        private Bridge _bridge;

        public HotKeyManager(Bridge bridge)
        {
            _bridge = bridge;
            _lhk = new List<HotKeyHandle>();
            LoadHotkeys();
            _ledTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 2)
            };
            _ledTimer.Tick += _ledTimer_Tick;
        }

        private void _ledTimer_Tick(object sender, EventArgs e)
        {
            _ledTimer.Stop();
            HotkeyDetected = false;
        }

        private void LoadHotkeys()
        {
            StopHotKeyCapture();
            ListHotKeys = new ObservableCollection<HotKey>(WinHueSettings.hotkeys.listHotKeys);
        }

        public bool HotkeyDetected
        {
            get => _hotkeyDetected;
            private set => SetProperty(ref _hotkeyDetected, value);
        }

        public ObservableCollection<HotKey> ListHotKeys
        {
            get => _listHotKeys; 
            private set => SetProperty(ref _listHotKeys,value);
        }

        public void StartHotKeyCapture()
        {
            StopHotKeyCapture();
            LoadHotkeys();
            foreach (HotKey h in ListHotKeys)
            {
                HotKeyHandle hkh = new HotKeyHandle(h, HandleHotkey);
                if (hkh.Register())
                    _lhk.Add(hkh);
                else
                    log.Error($"Cannot register hotkey {h.Name} key seems to be already taken by another process.");
            }
        }

        private void HandleHotkey(HotKeyHandle e)
        {
            ModifierKeys m = e.KeyModifiers;
            Key k = e.Key;
            try
            {
                HotKey h = ListHotKeys.First(x => x.Modifier == m && x.Key == k);
                if (!(h.objecType == null && _bridge == null))
                {
                    HotkeyDetected = true;
                    _ledTimer.Start();
                    Type objtype = h?.objecType == null ? _bridge.GetType() : h.objecType;

                    if (objtype == typeof(Scene))
                    {
                        _bridge.ActivateScene(h.id);
                    }
                    else if (objtype == typeof(Group) || objtype == typeof(Light))
                    {
                        _bridge.SetState(h.properties, h.id);
                    }
                    else
                    {
                        log.Warn($"Type of object {objtype} not supported");
                    }

                    if (h.ProgramPath == null) return;
                    if (File.Exists(h.ProgramPath))
                    {
                        log.Info($"Starting application at {h.ProgramPath}");
                        Process.Start(h.ProgramPath);
                    }
                    else
                    {
                        log.Error($"Application at {h.ProgramPath} does not exist anymore. Ignoring it...");
                    }
                }
                else
                {
                    log.Warn("You must select an object in WinHue in order to apply this hotkey.");
                }
            }
            catch (InvalidOperationException)
            {
                log.Warn("No Hotkey was found.");
                _ledTimer.Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void StopHotKeyCapture()
        {
            if (_lhk.Count > 0)
            {
                while (_lhk.Count != 0)
                {
                    _lhk[0].Unregister();
                    _lhk.Remove(_lhk[0]);
                }
            }
        }
    }
}