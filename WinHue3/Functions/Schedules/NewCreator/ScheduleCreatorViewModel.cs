using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {
        public enum ContentType { Sensor, Sliders };

        private ValidatableBindableBase _selectedViewModel;

        private ScheduleCreatorHeader _header;
        private ContentType _content;

        public ScheduleCreatorViewModel()
        {
            _header = new ScheduleCreatorHeader();
            _selectedViewModel = new ScheduleCreatorSlidersViewModel();
        }

        public async Task Initialize()
        {

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

        public ICommand ChangeContentCommand => new RelayCommand(param => ChangeContent());

        public ContentType Content { get => _content; set => SetProperty(ref _content,value); }

        private void ChangeContent()
        {
            if (Content == ContentType.Sliders)
                SelectedViewModel = new ScheduleCreatorSlidersViewModel();
            else
                SelectedViewModel = new ScheduleCreatorSensorsViewModel();
        }

    }
}
