using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {
        private ScheduleCreatorHeader _header;

        public ScheduleCreatorViewModel()
        {
            _header = new ScheduleCreatorHeader();
        }


        public ScheduleCreatorHeader Header
        {
            get => _header;
            set => SetProperty(ref _header,value);
        }
    }
}
