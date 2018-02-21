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

        public ScheduleCreatorSlidersViewModel()
        {

        }

        public ushort? Hue
        {
            get => _hue;
            set => SetProperty(ref _hue,value);
        }

        public byte? Bri
        {
            get => _bri;
            set => SetProperty(ref _bri,value);
        }

        public byte? Sat
        {
            get => _sat;
            set => SetProperty(ref _sat,value);
        }

        public ushort? Ct
        {
            get => _ct;
            set => SetProperty(ref _ct,value);
        }

        public decimal? X
        {
            get => _x;
            set => SetProperty(ref _x ,value);
        }

        public decimal? Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public ushort? Tt
        {
            get => _tt;
            set => SetProperty(ref _tt, value);
        }

        public string Effect
        {
            get => _effect;
            set => SetProperty(ref _effect, value);
        }
    }
}
