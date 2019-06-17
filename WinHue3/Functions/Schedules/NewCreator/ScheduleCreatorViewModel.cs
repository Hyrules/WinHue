using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;
using Group = WinHue3.Philips_Hue.HueObjects.GroupObject.Group;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public enum ContentTypeVm { Light, Sensor, Group, Schedule, Scene };

    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {

        private ObservableCollection<IHueObject> _listTargetHueObject;
        private ValidatableBindableBase _selectedViewModel;
        private ScheduleCreatorHeader _header;
        private ContentTypeVm _content;
        private List<IHueObject> _currentHueObjectList;
        private IHueObject _selectedHueObject;
        private string _effect;
        private string _dateTimeFormat;
        private string _smask;
        private int _repetitions;
        private HueAddress _adrTarget;
        private bool _propGridLG;
        private Bridge _bridge;

        public ICommand SelectTargetObject => new RelayCommand(param => SelectTarget());
        public ICommand ChangeDateTimeFormat => new RelayCommand(param => ChangeDateTime());
        public ICommand UsePropertyGridLGCommand => new RelayCommand(param => UsePropertyGridLG());

        private void UsePropertyGridLG()
        {
            if (Content != ContentTypeVm.Light && Content != ContentTypeVm.Group) return;
            if (_selectedViewModel is ScheduleCreatorSlidersViewModel)
            {
                string json = Serializer.SerializeJsonObject(SelectedViewModel);
                SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                ((ScheduleCreatorPropertyGridViewModel)SelectedViewModel).SelectedObject = Serializer.DeserializeToObject<State>(json);
            }
            else
            {
                string json = Serializer.SerializeJsonObject(((ScheduleCreatorPropertyGridViewModel)SelectedViewModel).SelectedObject);
                SelectedViewModel = Serializer.DeserializeToObject<ScheduleCreatorSlidersViewModel>(json);
            }
        }

        private void ChangeDateTime()
        {
            if (Header.ScheduleType == "W" || Header.ScheduleType == "PT")
            {
                DateTimeFormat = "HH:mm:ss";
            }
            else
            {
                DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
            }
        }

        public void EditSchedule(Schedule sc)
        {
            Header.Description = sc.description;
            Header.Name = sc.name;
            Header.Recycle = sc.recycle;
            Header.Autodelete = sc.autodelete;
            Header.Enabled = sc.status;

            if (sc.localtime.Contains("PT"))
            {
                Header.ScheduleType = "PT";
                Regex timerRegex = new Regex(@"(^R(\d{2})\/?)?PT(\d\d:\d\d:\d\d)(A\d\d:\d\d:\d\d)?$");
                Match mc = timerRegex.Match(sc.localtime);
                Header.Datetime = DateTime.Parse(mc.Groups[3].Value);

                if (mc.Groups[2].Value != string.Empty)
                {
                    Repetitions = Convert.ToInt32(mc.Groups[2].Value);
                }

                if(mc.Groups[4].Value != string.Empty)
                {
                    Header.Randomize = true;
                }
            }
            else if (sc.localtime.Contains("W"))
            {
                Header.ScheduleType = "W";
                Regex alarmRegex = new Regex(@"(^W(\d{3})//?)T(\d{2}:\d{2}:\d{2})(A\d{2}:\d{2}:\d{2})?$");
                Match mc = alarmRegex.Match(sc.localtime);
                Header.Datetime = DateTime.Parse(mc.Groups[3].Value);
                if(mc.Groups[2].Value != string.Empty)
                {
                    ScheduleMask = mc.Groups[2].Value;
                }

                if (mc.Groups[4].Value != string.Empty)
                {
                    Header.Randomize = true;
                }
            }
            else
            {
                Header.ScheduleType = "T";

                Regex scheduleRegex = new Regex(@"^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})(A\d{2}:\d{2}:\d{2})?$");
                Match mc = scheduleRegex.Match(sc.localtime);
                Header.Datetime = DateTime.Parse(mc.Groups[1].Value, CultureInfo.InvariantCulture);

                if (mc.Groups[2].Value != string.Empty)
                {
                    Header.Randomize = true;
                }
            }

            if (sc.command?.address?.objecttype == null) return;

            if (!sc.command.body.Contains("scene"))
            {            
                switch (sc.command.address.objecttype)
                {
                    case "lights":
                        Content = ContentTypeVm.Light;
                        break;
                    case "groups":
                        Content = ContentTypeVm.Group;
                        break;
                    case "schedules":
                        Content = ContentTypeVm.Schedule;
                        break;
                    case "sensors":
                        Content = ContentTypeVm.Sensor;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Content = ContentTypeVm.Scene;
            }

            if (Content != ContentTypeVm.Scene)
            {
                SelectedTarget = _listTargetHueObject.FirstOrDefault(x => x.Id == sc.command.address.id);
            }
            else
            {
                Action scene = Serializer.DeserializeToObject<Action>(sc.command.body);
                SelectedTarget = _listTargetHueObject.FirstOrDefault(x => x.Id == scene.scene);
            }

            if (SelectedTarget == null)
            {
                MessageBox.Show(GlobalStrings.Object_Does_Not_Exists, GlobalStrings.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                switch (SelectedTarget)
                {
                    case Sensor _:
                    {
                        ((ScheduleCreatorPropertyGridViewModel) _selectedViewModel).SelectedObject =
                            Serializer.DeserializeToObject(sc.command.body,HueSensorStateFactory.CreateSensorStateFromSensorType(((Sensor)SelectedTarget).type).GetType());
                        break;
                    }
                    case Schedule _:
                    {
                        ((ScheduleCreatorPropertyGridViewModel)_selectedViewModel).SelectedObject = Serializer.DeserializeToObject<Schedule>(sc.command.body);
                        break;
                    }
                    case Scene _:
                    {
                        SelectedViewModel = null;
                        break;
                    }
                    case Light _:
                    case Group _:
                    {
                        if(_propGridLG)
                        {
                            ((ScheduleCreatorPropertyGridViewModel)_selectedViewModel).SelectedObject = Serializer.DeserializeToObject<State>(sc.command.body);
                        }
                        else
                        {
                            SelectedViewModel = Serializer.DeserializeToObject<ScheduleCreatorSlidersViewModel>(sc.command.body);
                            if (SelectedViewModel == null)
                                SetEmptyViewModel();
                        }

                        break;
                    }
                }
            }

            AdrTarget = sc.command.address;
        }

        private void SetEmptyViewModel()
        {
            switch (SelectedTarget)
            {
                case Sensor _:
                {
                    SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                    ((ScheduleCreatorPropertyGridViewModel)SelectedViewModel).SelectedObject = HueSensorStateFactory.CreateSensorStateFromSensorType(((Sensor)SelectedTarget).type);
                    break;
                }
                case Schedule _:
                {
                    SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                    ((ScheduleCreatorPropertyGridViewModel)SelectedViewModel).SelectedObject = new Schedule();
                    break;
                }
                case Scene _:
                {
                    SelectedViewModel = null;
                    break;
                }
                case Light _:
                case Group _:
                {
                    if (_propGridLG)
                    {
                        ((ScheduleCreatorPropertyGridViewModel)_selectedViewModel).SelectedObject = new State();
                    }
                    else
                    {
                        SelectedViewModel = new ScheduleCreatorSlidersViewModel();
                    }
                    break;
                }
            }
        }

        private void SelectTarget()
        {
            SetEmptyViewModel();

            if (SelectedTarget == null) return;

            AdrTarget = new HueAddress
            {
                api = "api",
                key = _bridge.ApiKey
            };

            switch (_content)
            {
                case ContentTypeVm.Light:
                    AdrTarget.objecttype = "lights";
                    AdrTarget.property = "state";
                    AdrTarget.id = SelectedTarget.Id;                   
                    break;
                case ContentTypeVm.Group:
                    AdrTarget.objecttype = "groups";
                    AdrTarget.property = "action";
                    AdrTarget.id = SelectedTarget.Id;
                    break;
                case ContentTypeVm.Sensor:
                    AdrTarget.objecttype = "sensors";
                    AdrTarget.id = SelectedTarget.Id;
                    AdrTarget.property = "state";
                    break;
                case ContentTypeVm.Schedule:
                    AdrTarget.objecttype = "groups";
                    AdrTarget.id = "0";
                    AdrTarget.property = "action";
                    break;
                case ContentTypeVm.Scene:
                    AdrTarget.objecttype = "groups";
                    AdrTarget.id = "0";
                    AdrTarget.property = "action";
                    break;
                default:
                    break;
            }
            
        }

        public ScheduleCreatorViewModel()
        {
            _propGridLG = WinHueSettings.settings.UsePropertyGrid;
            ListTargetHueObject = new ObservableCollection<IHueObject>();
            _header = new ScheduleCreatorHeader();            
            _content = ContentTypeVm.Light;
            _effect = "none";
            _dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
            _smask = "000";
            if (_propGridLG)
            {
                _selectedViewModel = new ScheduleCreatorPropertyGridViewModel();
            }
            else
            {
                _selectedViewModel = new ScheduleCreatorSlidersViewModel();
            }
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            _currentHueObjectList = await _bridge.GetAllObjectsAsync();
            
            if (_currentHueObjectList == null) return;
            ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Light).ToList());
         
        }

        public Schedule GetSchedule()
        {
            Schedule sc = new Schedule
            {
                name = Header.Name,
                autodelete = Header.Autodelete,
                description = Header.Description,
                recycle = Header.Recycle,
                status = Header.Enabled,
                localtime = BuildScheduleTime(),
                command = new Command
                {
                    address = AdrTarget,
                    method = "PUT",
                },
            };

            string body = string.Empty;
            
            if(_selectedViewModel is ScheduleCreatorPropertyGridViewModel)
            {
                ScheduleCreatorPropertyGridViewModel scpg = _selectedViewModel as ScheduleCreatorPropertyGridViewModel;
                body = Serializer.ModifyJsonObject(scpg.SelectedObject); 
            }
            else if(_selectedViewModel is ScheduleCreatorSlidersViewModel)
            {
                ScheduleCreatorSlidersViewModel scsv = _selectedViewModel as ScheduleCreatorSlidersViewModel;
                body = Serializer.SerializeJsonObject(scsv);
            }
            else
            {
                body = Serializer.ModifyJsonObject(new Action() {scene = SelectedTarget.Id});
            }

            sc.command.body = body;

            return sc;
        }

        private string BuildScheduleTime()
        {
            string time = string.Empty;
            switch (Header.ScheduleType)
            {
                case "T":
                    time = Header.Datetime.ToString("yyyy-MM-ddTHH:mm:ss");
                    break;
                case "PT":
                    time = $"{Header.Datetime:HH:mm:ss}".Insert(0, "PT");
                    break;
                case "W":
                    time = $"{Header.Datetime:HH:mm:ss}".Insert(0, $"W{_smask:000}/T");
                    break;
                default:
                    break;
                
            }

            if (Header.Randomize.GetValueOrDefault())
            {
                Random rdm = new Random();
                time = time + $"A00:{rdm.Next(59):D2}:{rdm.Next(59):D2}";
            }

            return time;
        }

        public ScheduleCreatorHeader Header
        {
            get => _header;
            set => SetProperty(ref _header,value);
        }

        public ValidatableBindableBase SelectedViewModel
        {
            get => _selectedViewModel;
            set => SetProperty(ref _selectedViewModel, value);
        }

        public ICommand ChangeContentCommand => new RelayCommand(param => ChangeContent());

        public ContentTypeVm Content { get => _content; set => SetProperty(ref _content,value); }

        public ObservableCollection<IHueObject> ListTargetHueObject
        {
            get => _listTargetHueObject;
            set => SetProperty(ref _listTargetHueObject,value);
        }

        public IHueObject SelectedTarget
        {
            get => _selectedHueObject;
            set => SetProperty(ref _selectedHueObject,value);
        }

        public string Effect
        {
            get => _effect;
            set => SetProperty(ref _effect,value);
        }

        public string DateTimeFormat
        {
            get => _dateTimeFormat;
            set => SetProperty(ref _dateTimeFormat,value);
        }

        public string ScheduleMask
        {
            get => _smask;
            set => SetProperty(ref _smask,value);
        }

        public int Repetitions
        {
            get => _repetitions;
            set => SetProperty(ref _repetitions,value);
        }

        public HueAddress AdrTarget { get => _adrTarget; set => SetProperty(ref _adrTarget,value); }

        public bool PropGridLG
        {
            get => _propGridLG;
            set => SetProperty(ref _propGridLG,value);
        }

        private void ChangeContent()
        {
            ListTargetHueObject.Clear();
            
            if (_currentHueObjectList == null) return;

            if((Content == ContentTypeVm.Light || Content == ContentTypeVm.Group) && !PropGridLG)
            {
                SelectedViewModel = new ScheduleCreatorSlidersViewModel();
            }
            else if(Content == ContentTypeVm.Scene)
            {
                SelectedViewModel = null;
            }
            else
            {
                SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
            }

            switch (Content)
            {
                case ContentTypeVm.Light:              
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Light).ToList());
                    break;
                case ContentTypeVm.Group:
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Group).ToList());
                    break;
                case ContentTypeVm.Schedule:
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Schedule).ToList());
                    break;
                case ContentTypeVm.Sensor:
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Sensor).Where(x => ((Sensor)x).type.Contains("CLIP")).ToList());
                    break;
                case ContentTypeVm.Scene:
                    List<IHueObject> scenes = _currentHueObjectList.Where(x => x is Scene).ToList();
                    if (!WinHueSettings.settings.ShowHiddenScenes) scenes.RemoveAll(x => x.name.StartsWith("HIDDEN"));
                    ListTargetHueObject.AddRange(scenes);
                    break;
                default:
                    break;
            }



        }

    }
}
