using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using WinHue3.Annotations;
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    [JsonObject(MemberSerialization.OptIn)]
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

        [JsonProperty]
        public bool? on
        {
            get => _on;
            set => SetProperty(ref _on, value);
        }

        [JsonProperty]
        public ushort? hue
        {
            get => _hue;
            set => SetProperty(ref _hue,value);
        }

        [JsonProperty]
        public byte? bri
        {
            get => _bri;
            set => SetProperty(ref _bri,value);
        }

        [JsonProperty]
        public byte? sat
        {
            get => _sat;
            set => SetProperty(ref _sat,value);
        }

        [JsonProperty]
        public ushort? ct
        {
            get => _ct;
            set => SetProperty(ref _ct,value);
        }

        [CanBeNull]
        [JsonProperty]
        public decimal[] xy
        {
            get
            {
                if (_x == null || _y == null)
                    return null;
                return new decimal[2]{(decimal)_x,(decimal)_y};
            }
            set
            {
                if (value == null)
                {
                    x = null;
                    y = null;
                }
                else
                {
                    x = value[0];
                    y = value[1];
                }
            }
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

        [JsonProperty]
        public ushort? transitiontime
        {
            get => _tt;
            set => SetProperty(ref _tt, value);
        }

        [JsonProperty]
        public string effect
        {
            get => _effect;
            set => SetProperty(ref _effect, value);
        }

        [JsonProperty]
        public string alert
        {
            get => _alert;
            set => SetProperty(ref _alert, value);
        }
    }
}
