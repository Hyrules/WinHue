using System;
using System.ComponentModel.DataAnnotations;
using WinHue3.Utils;
using WinHue3.Validations;

namespace WinHue3.Functions.Schedules.OldCreator
{
    public class ScheduleCreatorModel : ValidatableBindableBase
    {
 
        private string _name;
        private string _description;
        private bool? _randomize;
        private bool? _autodelete;
        private bool _enabled = true;
        private string _transitiontime;
        private string _time;
        private int? _repetition;
        private string _date;

        public ScheduleCreatorModel()
        {
            _enabled = true;
            _time = DateTime.Now.Add(new TimeSpan(0,0,5)).ToString("HH:mm:ss");
            _date = DateTime.Now.ToString("yyyy-MM-dd");
            _transitiontime = string.Empty;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool? Randomize
        {
            get => _randomize;
            set => SetProperty(ref _randomize, value);
        }

        public bool? Autodelete
        {
            get => _autodelete;
            set => SetProperty(ref _autodelete, value);
        }

        public bool Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled,value);
        }

        [UIntValidation(ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Error_ScheduleInvalidTT")]
        public string Transitiontime
        {
            get => _transitiontime;
            set => SetProperty(ref _transitiontime, value);
        }

        [RegularExpression(@"^([0-1][0-9]|[2][0-3]):([0-5][0-9]):([0-5]?\d)$", ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "Error_ScheduleTimeInvalid" )]
        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        public int? Repetition
        {
            get => _repetition;
            set => SetProperty(ref _repetition,value);
        }

        public string Date
        {
            get => _date;
            set => SetProperty(ref _date,value);
        }


    }
}

