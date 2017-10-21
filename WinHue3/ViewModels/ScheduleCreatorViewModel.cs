using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.ExtensionMethods;
using WinHue3.Models;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;
using WinHue3.Validation;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;
using Bridge = WinHue3.Philips_Hue.BridgeObject.Bridge;
using IHueObject = WinHue3.Philips_Hue.HueObjects.Common.IHueObject;

namespace WinHue3.ViewModels
{
    public class ScheduleCreatorViewModel:ValidatableBindableBase
    {
        private ScheduleCreatorModel _scheduleCreatorModel;
        private string  _selectedType;
        private HueAddress _targetobject;
        private byte _smask;
        //private string _timeformat;
        private string _randomizetime;
        private bool _isedit;
        private bool _cansetsettings = true;
        private ObservableCollection<IHueObject> _listTarget;
        private Bridge _bridge;
        private IHueObject _selectedObject;
        private IBaseProperties _body;
        private int _effect;

        public ScheduleCreatorViewModel()
        {
            _scheduleCreatorModel = new ScheduleCreatorModel();
            _selectedType = "T";
            _smask = 0;
            _randomizetime = string.Empty;
            _effect = 1;
        }

        public int Effect
        {
            get => _effect;
            set
            {
                SetProperty(ref _effect, value);
              
            } 
        }

        public async Task Initialize(Bridge bridge, IHueObject obj)
        {
            _bridge = bridge;
            ListTarget = new ObservableCollection<IHueObject>();
            List<IHueObject> listObjects = await HueObjectHelper.GetBridgeDataStoreAsyncTask(_bridge);
            
            if (listObjects != null)
            {
                ListTarget.AddRange(listObjects.Where(x => x is Light));
                ListTarget.AddRange(listObjects.Where(x => x is Group));
                ListTarget.AddRange(listObjects.Where(x => x is Scene));
            }


            if (obj is Schedule)
            {

                IsEditing = true;
                Schedule schedule = (Schedule)obj;
                Body = BasePropertiesCreator.CreateBaseProperties(schedule.command.address.objecttype);
                JsonConvert.PopulateObject(schedule.command.body, Body);

                ScheduleModel.Enabled = schedule.status == "enabled";
                ScheduleModel.Name = schedule.name;
                ScheduleModel.Description = schedule.description;
                if (schedule.command.address.id == "0" && schedule.command.address.objecttype == "groups")
                    ((Action)Body).scene = ((Action)Body).scene;
                ScheduleModel.Autodelete = schedule.autodelete;
                ScheduleModel.Transitiontime = Body.transitiontime.ToString();
                
                _targetobject = schedule.command.address;

                SelectedObject = ListTarget.FirstOrDefault(x => x.Id == _targetobject.id && x.GetType() == (HueObjectCreator.CreateHueObject(_targetobject.objecttype)).GetType());

                if (Body.xy != null)
                {
                    Body.xy[0] = Body.xy[0];
                    Body.xy[1] = Body.xy[1];
                }

                Body.on = Body.on ?? (((Action)Body).scene != null);

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
            else
            {
                if (obj is Scene)
                {
                    CanSetSettings = false;
                    IsEditing = true;
                    Body = BasePropertiesCreator.CreateBaseProperties("action");
                    ((Action) Body).scene = ((Scene) obj).Id;
                }
                else
                {
                    Body = BasePropertiesCreator.CreateBaseProperties(obj.GetType());
                }

                TargetObject = new HueAddress($@"/{(obj is Light ? "lights" : "groups")}/{(obj is Scene ? "0" : obj.Id)}/{(obj is Light ? "state" : "action")}");
                SelectedObject = ListTarget.FirstOrDefault(x => x.Id == obj.Id && x.GetType() == obj.GetType());
            }

        }

        public bool IsEditing
        {
            get => _isedit;
            set => SetProperty(ref _isedit, !value);
        }

        public ObservableCollection<IHueObject> ListTarget
        {
            get => _listTarget;
            set => SetProperty(ref _listTarget, value);
        }

        public IHueObject SelectedObject
        {
            get => _selectedObject;
            set
            {
                SetProperty(ref _selectedObject, value);
                if (value == null) return;
                string id = SelectedObject is Scene ? "0" : SelectedObject.Id;
                string part = SelectedObject is Light ? "state" : "action";
                string type = SelectedObject is Scene
                    ? "groups"
                    : SelectedObject.GetType().Name.LowerFirstLetter() + "s";

                TargetObject = new HueAddress($"/api/{_bridge.ApiKey}/{type}/{id}/{part}");
            }
        }

        public Schedule Schedule
        {
            get {
                Schedule schedule = new Schedule
                {
                    
                    description = ScheduleModel.Description,
                    name = ScheduleModel.Name,
                    status = ScheduleModel.Enabled ? "enabled" : "disabled",
                    command = new Command() {method = "PUT",address = TargetObject},
                    localtime = BuildScheduleLocaltime($"{ScheduleModel.Date} {ScheduleModel.Time}",SelectedType),                   
                };

                IBaseProperties scheduleBody = Body;
                         
                if (SelectedType != "T")
                {
                    schedule.autodelete = ScheduleModel.Autodelete;
                }

                if (ScheduleModel.Transitiontime != string.Empty)
                {
                    scheduleBody.transitiontime = Convert.ToUInt32(ScheduleModel.Transitiontime);
                }

                if (schedule.command.address.id != "0" && schedule.command.address.objecttype != "groups")
                {
                    scheduleBody.on = Body.on;
                }

                if(Body.xy != null )
                {
                    scheduleBody.xy = new decimal[] { Body.xy[0], Body.xy[1] };
                }

                schedule.command.body = Serializer.SerializeToJson(scheduleBody);
                return schedule;
            }

        }

        public bool CanSetSettings
        {
            get => _cansetsettings;
            set => SetProperty(ref _cansetsettings,value);
        }

        public ScheduleCreatorModel ScheduleModel
        {
            get => _scheduleCreatorModel;
            set => SetProperty(ref _scheduleCreatorModel, value);
        }

        public string SelectedType
        {
            get => _selectedType;
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
            get => _smask;
            set => SetProperty(ref _smask, value);
        }

        public HueAddress TargetObject
        {
            get => _targetobject;
            set => SetProperty(ref _targetobject,value);
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
            Body.hue = null;
            Body.bri = null;
            Body.sat = null;
            Body.ct = null;
            Body.sat = null;
            ScheduleModel.Autodelete = null;
            Body.on = true;
            ScheduleModel.Time = DateTime.Now.Add(new TimeSpan(0,0,5)).ToString("HH:mm:ss");
            ScheduleModel.Date = DateTime.Now.ToString("yyyy-MM-dd");
            ScheduleModel.Randomize = null;
            ScheduleModel.Repetition = null;
            Body.xy = null;
            ScheduleModel.Transitiontime = null;
            ScheduleModel.Enabled = true;
        }

        public ICommand ClearFieldsCommand => new RelayCommand(param => ClearFields());

        public IBaseProperties Body
        {
            get { return _body; }
            set { SetProperty(ref _body,value); }
        }
    }
}
