using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public enum ContentTypeVm { Sensor, Sliders };

    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {
        

        private ValidatableBindableBase _selectedViewModel;

        private ScheduleCreatorHeader _header;
        private ContentTypeVm _content;

        public ScheduleCreatorViewModel()
        {
            _header = new ScheduleCreatorHeader();
            _selectedViewModel = new ScheduleCreatorSlidersViewModel();
            _content = ContentTypeVm.Sliders;
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

        public ContentTypeVm Content { get => _content; set => SetProperty(ref _content,value); }

        private void ChangeContent()
        {
            if (Content == ContentTypeVm.Sliders)
                SelectedViewModel = new ScheduleCreatorSlidersViewModel();
            else
                SelectedViewModel = new ScheduleCreatorSensorsViewModel();
        }

    }
}
