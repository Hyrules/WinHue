using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {
        private ValidatableBindableBase _selectedViewModel;

        private ScheduleCreatorHeader _header;

        public ScheduleCreatorViewModel()
        {
            _header = new ScheduleCreatorHeader();
            _selectedViewModel = new ScheduleCreatorSlidersViewModel();
        }

        public ScheduleCreatorHeader Header
        {
            get => _header;
            set => SetProperty(ref _header,value);
        }
        public ValidatableBindableBase SelectedViewModel
        {
            get => _selectedViewModel;
            set => SetProperty(ref _selectedViewModel, value);
        }
    }
}
