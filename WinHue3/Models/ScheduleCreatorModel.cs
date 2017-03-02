using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HueLib2;
using System.ComponentModel.DataAnnotations;
using WinHue3.Validation;

namespace WinHue3.Models
{
    public class ScheduleCreatorModel : ValidatableBindableBase
    {
 
        private string _name;
        private string _description;
        private bool? _randomize;
        private bool? _autodelete;
        private bool _on = true;
        private bool _enabled = true;
        private string _transitiontime;
        private string _time;
        private ushort? _hue;
        private byte? _bri;
        private byte? _sat;
        private ushort? _ct;
        private decimal? _x;
        private decimal? _y;
        private string _scene;
        private int? _repetition;
        private string _date;

        public ScheduleCreatorModel()
        {
            On = true;
            Enabled = true;
            Time = DateTime.Now.Add(new TimeSpan(0,0,5)).ToString("HH:mm:ss");
            Date = DateTime.Now.ToString("yyyy-MM-dd");
            Transitiontime = string.Empty;
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public bool? Randomize
        {
            get { return _randomize; }
            set { SetProperty(ref _randomize, value); }
        }

        public bool? Autodelete
        {
            get { return _autodelete; }
            set { SetProperty(ref _autodelete, value); }
        }

        public bool On
        {
            get { return _on; }
            set { SetProperty(ref _on,value);}
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { SetProperty(ref _enabled,value);}
        }

        [UIntValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Error_ScheduleInvalidTT")]
        public string Transitiontime
        {
            get { return _transitiontime; }
            set{ SetProperty(ref _transitiontime, value); }
        }

        [RegularExpression(@"^([0-1][0-9]|[2][0-3]):([0-5][0-9]):([0-5]?\d)$", ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Error_ScheduleTimeInvalid" )]
        public string Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        public ushort? Hue
        {
            get { return _hue; }
            set
            {

                SetProperty(ref _hue,value);
                _x = null;
                _y = null;
                _ct = null;
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
                OnPropertyChanged("Ct");
            }
        }

        public byte? Bri
        {
            get { return _bri; }
            set { SetProperty(ref _bri,value); }
        }

        public byte? Sat
        {
            get { return _sat; }
            set { SetProperty(ref _sat, value); }
        }

        public ushort? Ct
        {
            get { return _ct; }
            set
            {
                SetProperty(ref _ct,value);
                _x = null;
                _y = null;
                _hue = null;
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
                OnPropertyChanged("Hue");
            }
        }

        public decimal? X
        {
            get { return _x; }
            set
            {
                SetProperty(ref _x,value);
                _hue = null;
                _ct = null;
                if (_y == 0)
                {
                    _y = 0;
                    OnPropertyChanged("Y");
                }
                OnPropertyChanged("Hue");
                OnPropertyChanged("Ct");
            }
        }

        public decimal? Y
        {
            get { return _y; }
            set
            {
                SetProperty(ref _y,value);
                _hue = null;
                _ct = null;
                if (_x == null)
                {
                    _x = 0;
                    OnPropertyChanged("X");
                }
                OnPropertyChanged("Hue");
                OnPropertyChanged("Ct");
            }
        }

        public int? Repetition
        {
            get { return _repetition; }
            set { SetProperty(ref _repetition,value); }
        }

        public string Scene
        {
            get { return _scene; }
            set { SetProperty(ref _scene,value); }
        }

        public string Date
        {
            get { return _date; }
            set
            {
                SetProperty(ref _date,value);
            }
        }
    }
}

