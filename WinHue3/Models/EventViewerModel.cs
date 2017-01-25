using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.Models
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
            get
            {
                return _listlogs;
            }
            set { SetProperty(ref _listlogs,value); }
        }

    }
}
