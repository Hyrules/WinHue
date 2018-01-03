using WinHue3.Utils;

namespace WinHue3.Functions.EventViewer
{
    public class EventViewerViewModel : ValidatableBindableBase
    {
        private EventViewerModel _eventViewerModel;

        public EventViewerViewModel()
        {
            _eventViewerModel = new EventViewerModel();
        }

        public EventViewerModel EventViewerModel
        {
            get => _eventViewerModel;

            set => SetProperty(ref _eventViewerModel,value);
        }
    }
}
