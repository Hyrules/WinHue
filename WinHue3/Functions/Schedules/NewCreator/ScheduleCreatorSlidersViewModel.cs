using System;
using System.Runtime.Serialization;
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleCreatorSlidersViewModel : ValidatableBindableBase
    {
        private ushort? _hue;
        private byte? _bri;
        private byte? _sat;
        private ushort? _ct;
        private decimal? _x;
        private decimal? _y;
        private ushort? _tt;
        private string _effect;
        private string _alert;
        private bool? _on;
        public ScheduleCreatorSlidersViewModel()
        {

        }

        public bool? on
        {
            get => _on;
            set => SetProperty(ref _on, value);
        }

        public ushort? hue
        {
            get => _hue;
            set => SetProperty(ref _hue,value);
        }

        public byte? bri
        {
            get => _bri;
            set => SetProperty(ref _bri,value);
        }

        public byte? sat
        {
            get => _sat;
            set => SetProperty(ref _sat,value);
        }
        
        public ushort? ct
        {
            get => _ct;
            set => SetProperty(ref _ct,value);
        }

        public decimal? x
        {
            get => _x;
            set => SetProperty(ref _x ,value);
        }

        public decimal? y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public ushort? tt
        {
            get => _tt;
            set => SetProperty(ref _tt, value);
        }

        public string effect
        {
            get => _effect;
            set => SetProperty(ref _effect, value);
        }

        public string alert
        {
            get => _alert;
            set => SetProperty(ref _alert, value);
        }
    }
}
