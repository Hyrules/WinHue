using System;
using WinHue3.Utils;

namespace WinHue3.Functions.BridgeSettings
{
    public class BridgeSettingsSoftwareModel : ValidatableBindableBase
    {
        private string _updatestate;
        private bool _autoUpdate;
        private DateTime _updateTime;
        private string _lastChange;
        private string _lastUpdate;

        public BridgeSettingsSoftwareModel()
        {
            _updatestate = "unknown";
        }

        public string Updatestate
        {
            get => _updatestate;
            set => SetProperty(ref _updatestate,value);
        }

        public bool AutoUpdate
        {
            get => _autoUpdate;
            set => SetProperty(ref _autoUpdate,value);
        }

        public DateTime UpdateTime
        {
            get => _updateTime;
            set => SetProperty(ref _updateTime,value);
        }

        public string LastChange
        {
            get => _lastChange;
            set => SetProperty(ref _lastChange,value);
        }

        public string LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate,value);
        }
    }
}
