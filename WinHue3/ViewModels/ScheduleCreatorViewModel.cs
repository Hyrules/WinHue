using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HueLib2;
using HueLib2.Objects.HueObject;
using WinHue3.Converters;
using WinHue3.Models;
using WinHue3.Utils;
using WinHue3.Validation;

namespace WinHue3.ViewModels
{
    public class ScheduleCreatorViewModel:ValidatableBindableBase
    {
        private ScheduleCreatorModel _scheduleCreatorModel;
        private string  _selectedType;
        private string _targetobject;
        private byte _smask;
        private string _timeformat;
        private string _randomizetime;
        private bool _isedit;
        private bool _cansetsettings = true;
        private ObservableCollection<IHueObject> _listTarget;
        private Bridge _bridge;
        private IHueObject _selectedObject;

        public ScheduleCreatorViewModel()
        {
            _scheduleCreatorModel = new ScheduleCreatorModel();
            _selectedType = "T";
            _smask = 0;
            _randomizetime = string.Empty;
        }

        public void Initialize(Bridge bridge)
        {
            _bridge = bridge;
            ListTarget = new ObservableCollection<IHueObject>();
            List<IHueObject> listObjects = HueObjectHelper.GetBridgeDataStore(_bridge);
            
            if (listObjects != null)
            {
                ListTarget.AddRange(listObjects.Where(x => x is Light));
                ListTarget.AddRange(listObjects.Where(x => x is Group));
                ListTarget.AddRange(listObjects.Where(x => x is Scene));
            }

        }

        public bool IsEditing
        {
            get { return _isedit; }
            set { SetProperty(ref _isedit, !value); }
        }

        public ObservableCollection<IHueObject> ListTarget
        {
            get { return _listTarget; }
            set { SetProperty(ref _listTarget, value); }
        }

        public IHueObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                SetProperty(ref _selectedObject, value);
                if (value == null) return;
                string id = SelectedObject is Scene ? "0" : SelectedObject.Id;
                string part = SelectedObject is Light ? "state" : "action";
                string type = SelectedObject is Scene
                    ? "groups"
                    : SelectedObject.GetType().Name.LowerFirstLetter() + "s";

                TargetObject = $"/api/{_bridge.ApiKey}/{type}/{id}/{part}";
            }
        }

        public Schedule Schedule
        {
            get {
                Schedule schedule = new Schedule
                {
                    
                    description = ScheduleModel.Description,
                    Name = ScheduleModel.Name,
                    status = ScheduleModel.Enabled ? "enabled" : "disabled",
                    command = new Command() {body = new Body()
                    {
                        scene = ScheduleModel.Scene,
                        bri = ScheduleModel.Bri,
                        sat = ScheduleModel.Sat,
                        hue = ScheduleModel.Hue,
                        ct = ScheduleModel.Ct,
                        

                    },
                    method = "PUT",address = TargetObject},
                    localtime = BuildScheduleLocaltime($"{ScheduleModel.Date} {ScheduleModel.Time}",SelectedType),
                    
                };

                if (SelectedType != "T")
                {
                    schedule.autodelete = ScheduleModel.Autodelete;
                }

                if (ScheduleModel.Transitiontime != string.Empty)
                {
                    schedule.command.body.transitiontime = Convert.ToUInt32(ScheduleModel.Transitiontime);
                }
                
                if (schedule.command.body.scene == null)
                {
                    schedule.command.body.on = ScheduleModel.On;
                }

                if(ScheduleModel.X != null && ScheduleModel.Y != null)
                {
                    schedule.command.body.xy = new XY { x = (decimal)ScheduleModel.X, y = (decimal)ScheduleModel.Y };
                }
                return schedule;
            }
            set
            {
                IsEditing = true;
                Schedule schedule = value;
                ScheduleModel.Enabled = schedule.status == "enabled";
                ScheduleModel.Name = schedule.Name;
                ScheduleModel.Description = schedule.description;
                ScheduleModel.Bri = schedule.command.body.bri;
                ScheduleModel.Sat = schedule.command.body.sat;
                ScheduleModel.Scene = schedule.command.body.scene;
                ScheduleModel.Autodelete = schedule.autodelete;
                ScheduleModel.Transitiontime = schedule.command.body.transitiontime.ToString();

                if (schedule.command.body.hue != null)
                    ScheduleModel.Hue = schedule.command.body.hue;

                if (schedule.command.body.ct != null)
                    ScheduleModel.Ct = schedule.command.body.ct;

                _targetobject = schedule.command.address;
                string[] _split = _targetobject.Split('/');
                string _type = _split[3].TrimEnd(new char[] { 's' }).CapitalizeFirstLetter();
                string _id = _split[4];

                SelectedObject = ListTarget.FirstOrDefault(x => x.Id == _id && x.GetType() == HueObjectHelper.GetTypeFromName(_type));

                if (schedule.command.body.xy != null)
                {
                    ScheduleModel.X = schedule.command.body.xy.x;
                    ScheduleModel.Y = schedule.command.body.xy.y;
                }
                ScheduleModel.On = schedule.command.body.on ?? (ScheduleModel.Scene == null);
                SelectedType = GetScheduleTypeFromTime(schedule.localtime);

                if (schedule.localtime.Contains("A"))
                {
                    int indexA = schedule.localtime.IndexOf("A", StringComparison.Ordinal);
                    _randomizetime = schedule.localtime.Substring(indexA);
                }

                if (schedule.localtime.Contains("R"))
                {
                    ScheduleModel.Repetition = Convert.ToInt32(schedule.localtime.Substring(1, 2));
                    schedule.localtime = schedule.localtime.Remove(0, 4);
                }

                switch (SelectedType)
                {
                    case "W":
                        ScheduleMask = Convert.ToByte(GetMaskFromAlarm(schedule.localtime));
                        ScheduleModel.Time = schedule.localtime.Substring(6);
                        break;
                    case "T":
                        string[] datetime = schedule.localtime.Split('T');
                        ScheduleModel.Date = datetime[0];
                        ScheduleModel.Time = datetime[1];
                        break;
                    case "PT":
                        ScheduleModel.Time = schedule.localtime.Replace("PT", "");
                        break;
                    default:
                        goto case "T";
                }
                
            }
        }

        public bool CanSetSettings
        {
            get { return _cansetsettings; }
            set
            {
                SetProperty(ref _cansetsettings,value);
            }
        }

        public ScheduleCreatorModel ScheduleModel
        {
            get { return _scheduleCreatorModel; }
            set { SetProperty(ref _scheduleCreatorModel, value); }
        }

        public string SelectedType
        {
            get { return _selectedType; }
            set
            {
                SetProperty(ref _selectedType,value);
                RaisePropertyChanged("StartTimeText");
                RaisePropertyChanged("ScheduleMask");
                if (SelectedType != "PT") ScheduleModel.Repetition = null;    
            }
        }

        public string StartTimeText => _selectedType == "PT" ? Resources.GUI.ScheduleCreatorForm_StartTimeTimer : Resources.GUI.ScheduleCreatorForm_StartTime;

        [RequireMask(ErrorMessageResourceName = "Schedule_SelectAtLeastOneDay",ErrorMessageResourceType = typeof(GlobalStrings))]
        public byte ScheduleMask
        {
            get { return _smask; }
            set
            {
                SetProperty(ref _smask, value);
            }
        }

        public string TargetObject
        {
            get { return _targetobject; }
            set { SetProperty(ref _targetobject,value); }
        }

        private string BuildScheduleLocaltime(string timevalue, string type)
        {
            string time;
            DateTime dt = DateTime.ParseExact(timevalue,"yyyy-MM-dd HH:mm:ss",CultureInfo.InvariantCulture);
            switch (type)
            {
                case "T":
                    time = timevalue.Replace(" ", "T");
                    break;
                case "PT":
                    time = $"{dt:HH:mm:ss}".Insert(0, "PT");
                    break;
                case "W":
                    time = $"{dt:HH:mm:ss}".Insert(0, $"W{_smask:000}/T");
                    break;
                default:
                    time = timevalue.Replace(" ", "T");
                    break;
            }

            if (ScheduleModel.Randomize ?? false)
            {
                time += "A" + DateTime.Parse("00:" + new Random().Next(1, 59) + ":00").ToString("HH:mm:ss");
            }

            if (ScheduleModel.Repetition != null)
            {
                time = time.Insert(0, $"R{ScheduleModel.Repetition:00}/" );
            }

            return time;
        }

        private string GetMaskFromAlarm(string timestring)
        {
            return timestring.Substring(1, 3);
        }

        private string GetScheduleTypeFromTime(string timestring)
        {
            if (timestring.Contains("PT")) return "PT";
            if (timestring.Contains("W")) return "W";
            return "T";
        }

        private void ClearFields()
        {
            ScheduleMask = 0;
            SelectedType = "T";
            ScheduleModel.Name = string.Empty;
            ScheduleModel.Description = string.Empty;
            ScheduleModel.Hue = null;
            ScheduleModel.Bri = null;
            ScheduleModel.Sat = null;
            ScheduleModel.Ct = null;
            ScheduleModel.Sat = null;
            ScheduleModel.Autodelete = null;
            ScheduleModel.On = true;
            ScheduleModel.Time = DateTime.Now.Add(new TimeSpan(0,0,5)).ToString("HH:mm:ss");
            ScheduleModel.Date = DateTime.Now.ToString("yyyy-MM-dd");
            ScheduleModel.Randomize = null;
            ScheduleModel.Repetition = null;
            ScheduleModel.X = null;
            ScheduleModel.Y = null;
            ScheduleModel.Transitiontime = null;
            ScheduleModel.Enabled = true;
        }

        public ICommand ClearFieldsCommand => new RelayCommand(param => ClearFields());


    }
}
