using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleCreatorSlidersViewModel : ValidatableBindableBase
    {
        private ushort? _hue;
        private byte? _bri;
        private byte? _sat;
        private ushort? _ct;
        private decimal? x;
        private decimal? y;

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
            get => x;
            set => SetProperty(ref x ,value);
        }

        public decimal? Y
        {
            get => y;
            set => SetProperty(ref y, value);
        }
    }
}
