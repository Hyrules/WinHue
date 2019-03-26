using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Functions.BridgeManager
{
    public sealed partial class BridgeManager : ValidatableBindableBase
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly BridgeManager _bridgeManager = new BridgeManager();
        private IHueObject _selectedObject;
        public static BridgeManager Instance => _bridgeManager;
        private ObservableCollection<IHueObject> _listCurrentBridgeHueObjects;
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();
        private readonly DispatcherTimer _findlighttimer = new DispatcherTimer();
        private readonly DispatcherTimer _findsensortimer = new DispatcherTimer();
        private bool _searchingLights;
        private bool _searchingSensors;
        private ObservableCollection<Bridge> _listBridges;
        private Bridge _selectedBridge;

        private BridgeManager()
        {
            ListBridges = new ObservableCollection<Bridge>();
            CurrentBridgeHueObjectsList = new ObservableCollection<IHueObject>();
            _refreshTimer.Interval = new TimeSpan(0, 0, WinHueSettings.settings.RefreshTime);
            _refreshTimer.Tick += _refreshTimer_Tick;
            _findlighttimer.Interval = new TimeSpan(0, 1, 0);
            _findlighttimer.Tick += _findlighttimer_Tick;
            _findsensortimer.Interval = new TimeSpan(0, 1, 0);
            _findsensortimer.Tick += _findsensortimer_Tick;

        }

    }
}
