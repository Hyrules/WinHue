using WinHue3.Models;

namespace WinHue3.ViewModels
{
    public class EventViewerViewModel : ValidatableBindableBase
    {
        private EventViewerModel _eventViewerModel;

        public EventViewerViewModel()
        {
            EventViewerModel = new EventViewerModel();
        }

        public EventViewerModel EventViewerModel
        {
            get => _eventViewerModel;

            set => SetProperty(ref _eventViewerModel,value);
        }
    }
}
