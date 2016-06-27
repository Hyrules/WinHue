using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3
{
    public class EventViewerView : View
    {
        private ObservableCollection<DgLogEntry> _listlogs; 

        public EventViewerView()
        {
            _listlogs = new ObservableCollection<DgLogEntry>();
        }

        #region PROPERTIES


        public ObservableCollection<DgLogEntry> ListLogEntries
        {
            get
            {
                return _listlogs;
            }
            set { _listlogs = value; OnPropertyChanged();}
        } 

        #endregion

        #region METHODS

        #endregion
    }

}
