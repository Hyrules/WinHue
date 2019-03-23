using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Logs;

namespace WinHue3.Functions.EventViewer
{
    public class EventLog
    {
        public static EventLog Instance = new EventLog();
        private ObservableCollection<DgLogEntry> _logLines;

        private EventLog()
        {
            _logLines = new ObservableCollection<DgLogEntry>();
        }

        public ObservableCollection<DgLogEntry> ListLogs
        {
            get => _logLines;
            set => _logLines = value;
        }
    }
}
