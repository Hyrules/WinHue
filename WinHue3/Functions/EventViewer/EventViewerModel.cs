using System.Collections.ObjectModel;
using WinHue3.Logs;
using WinHue3.Utils;

namespace WinHue3.Functions.EventViewer
{
    public class EventViewerModel : ValidatableBindableBase
    {
        private ObservableCollection<DgLogEntry> _listlogs;

        public EventViewerModel()
        {
            _listlogs = new ObservableCollection<DgLogEntry>();
        }

        public ObservableCollection<DgLogEntry> ListLogEntries
        {
            get => _listlogs;
            set => SetProperty(ref _listlogs,value);
        }

    }
}
