using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Utils;

namespace WinHue3.Functions.BridgeManager
{
    [Obsolete]
    public sealed partial class BridgesManager : ValidatableBindableBase
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly BridgesManager _bridgeManager = new BridgesManager();
        private IHueObject _selectedObject;
        public static BridgesManager Instance => _bridgeManager;
        private ObservableCollection<IHueObject> _listCurrentBridgeHueObjects;
        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer();
        private readonly DispatcherTimer _findlighttimer = new DispatcherTimer();
        private readonly DispatcherTimer _findsensortimer = new DispatcherTimer();
        private bool _searchingLights;
        private bool _searchingSensors;
        private ObservableCollection<Bridge> _listBridges;
        private Bridge _selectedBridge;

        private BridgesManager()
        {
            ListBridges = new ObservableCollection<Bridge>();
            CurrentBridgeHueObjectsList = new ObservableCollection<IHueObject>();
            _refreshTimer.Interval = new TimeSpan(0, 0, (int)WinHueSettings.settings.RefreshTime);
            _refreshTimer.Tick += _refreshTimer_Tick;
            _findlighttimer.Interval = new TimeSpan(0, 1, 0);
            _findlighttimer.Tick += _findlighttimer_Tick;
            _findsensortimer.Interval = new TimeSpan(0, 1, 0);
            _findsensortimer.Tick += _findsensortimer_Tick;

        }

    }
}
