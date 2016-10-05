using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.MainForm
{
    public class ButtonsView : View
    {
        private bool _btnsearchlight;
        private bool _btnsearchsensor;
        private bool _enabled;
        private bool _btnallon;
        private bool _btnalloff;
        private bool _btnrefreshview;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged();
                OnPropertyChanged("BtnSearchLight");
                OnPropertyChanged("BtnSearchSensor");
                OnPropertyChanged("BtnAllOn");
                OnPropertyChanged("BtnAllOff");
                OnPropertyChanged("BtnRefreshView");
            }
        }

        public bool BtnSearchLight
        {
            get { return _btnsearchlight && _enabled; }
            set { _btnsearchlight = value;OnPropertyChanged(); }
        }

        public bool BtnSearchSensor
        {
            get { return _btnsearchsensor && _enabled; }
            set { _btnsearchsensor = value;OnPropertyChanged(); }
        }

        public bool BtnAllOn
        {
            get { return _btnallon && _enabled; }
            set { _btnallon = value; OnPropertyChanged(); }
        }

        public bool BtnAllOff
        {
            get { return _btnallon && _enabled; }
            set { _btnallon = value; OnPropertyChanged(); }
        }

        public bool BtnRefreshView
        {
            get { return _btnrefreshview; }
            set { _btnrefreshview = value; OnPropertyChanged(); }
        }

        //public bool
    }
}
