using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get
            {
                return _eventViewerModel;
            }

            set
            {
                SetProperty(ref _eventViewerModel,value);
            }
        }
    }
}
