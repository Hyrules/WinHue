using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleCreatorPropertyGridViewModel : ValidatableBindableBase
    {
        public ScheduleCreatorPropertyGridViewModel()
        {
        }

        private object _selectedObject;

        public object SelectedObject
        {
            get { return _selectedObject; }
            set { SetProperty(ref _selectedObject,value); }
        }
    }
}
