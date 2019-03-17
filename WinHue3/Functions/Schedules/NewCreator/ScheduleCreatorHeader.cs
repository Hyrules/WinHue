using System;
using System.ComponentModel;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleCreatorHeader : ValidatableBindableBase
    {
        public string _name;
        public string _description;
        public DateTime _datetime;
        public ushort? _transitiontime;
        public bool? _autodelete;
        public bool? _on;
        public bool? _randomize;
        public string _enabled;
        public string _scheduletype;
        public bool? _recycle;

        public ScheduleCreatorHeader()
        {
            _datetime = DateTime.Now;
            _datetime += new TimeSpan(0,10,0);
            _enabled = "enabled";
            _scheduletype = "T";
            _on = true;
        }

        [PropertyOrder(1)]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        [PropertyOrder(2)]
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description,value);
        }

        [PropertyOrder(3)]
        [Description("Date and time of the schedule")]
        public DateTime Datetime
        {
            get => _datetime;
            set => SetProperty(ref _datetime,value);
        }

        [PropertyOrder(4), ItemsSource(typeof(ScheduleTypeItemSource))]
        public string ScheduleType
        {
            get => _scheduletype;
            set => SetProperty(ref _scheduletype, value);
        }

        [Category("Schedule Settings"), DefaultValue(null),Description("Transition time in multiple of 100ms (1000ms = 1 sec)")]
        public ushort? Transitiontime
        {
            get => _transitiontime;
            set => SetProperty(ref _transitiontime,value);
        }

        [Category("Schedule Settings"), DefaultValue(null),Description("Autodelete the schedule after running it.")]
        public bool? Autodelete
        {
            get => _autodelete;
            set => SetProperty(ref _autodelete,value);
        }

        [Category("Schedule Settings"), Description("Turn on or off the schedule."), DefaultValue(null)]
        public bool? On
        {
            get => _on;
            set => SetProperty(ref _on,value);
        }

        [Category("Schedule Settings"),Description("Randomize the schedule time by adding or removing minutes to the selected time."), DefaultValue(null)]
        public bool? Randomize
        {
            get => _randomize;
            set => SetProperty(ref _randomize,value);
        }

        [Category("Schedule Settings"), Description("Enable or disable the schedule."),  ItemsSource(typeof(EnabledItemSource))]
        public string Enabled
        {
            get => _enabled;
            set => SetProperty(ref _enabled,value);
        }

        [Category("Schedule Settings"), Description("Recycle the schedule."), ItemsSource(typeof(EnabledItemSource))]
        public bool? Recycle
        {
            get => _recycle;
            set => SetProperty(ref _recycle, value);
        }
    }
}
