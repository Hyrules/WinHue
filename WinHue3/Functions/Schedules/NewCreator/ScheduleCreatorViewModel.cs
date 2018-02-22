using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public enum ContentTypeVm { Light, Sensor, Group, Schedule, Scene };

    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {

        private ObservableCollection<IHueObject> _listTargetHueObject;
        private ValidatableBindableBase _selectedViewModel;
        private Bridge _bridge;
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

        public ICommand SelectTargetObject => new RelayCommand(param => SelectTarget());
        public ICommand ChangeDateTimeFormat => new RelayCommand(param => ChangeDateTime());
        public ICommand UsePropertyGridLGCommand => new RelayCommand(param => UsePropertyGridLG());

        private void UsePropertyGridLG()
        {
            if (Content != ContentTypeVm.Light && Content != ContentTypeVm.Group) return;
            if (_selectedViewModel is ScheduleCreatorSlidersViewModel)
            {
                SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
            }
            else
            {
                SelectedViewModel = new ScheduleCreatorSlidersViewModel();
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

        private void SelectTarget()
        {

            if (SelectedTarget is Sensor)
            {
                ScheduleCreatorPropertyGridViewModel _scvm = _selectedViewModel as ScheduleCreatorPropertyGridViewModel;
                _scvm.SelectedObject = HueSensorStateFactory.CreateSensorStateFromSensorType(((Sensor)SelectedTarget).type);
            }
            else if(SelectedTarget is Schedule)
            {
                ScheduleCreatorPropertyGridViewModel _scvm = _selectedViewModel as ScheduleCreatorPropertyGridViewModel;
                _scvm.SelectedObject = new Schedule();
            }

            if (SelectedTarget is Light || SelectedTarget is Group)
            {
                ScheduleCreatorPropertyGridViewModel _scvm = _selectedViewModel as ScheduleCreatorPropertyGridViewModel;
                _scvm.SelectedObject = new State();
            }

            if (SelectedTarget == null) return;

            AdrTarget = new HueAddress();
            AdrTarget.api = "api";
            AdrTarget.key = _bridge.ApiKey;

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
                    AdrTarget.objecttype = "schedules";
                    AdrTarget.id = SelectedTarget.Id;
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
            ListTargetHueObject = new ObservableCollection<IHueObject>();
            _header = new ScheduleCreatorHeader();
            _selectedViewModel = new ScheduleCreatorSlidersViewModel();
            _content = ContentTypeVm.Light;
            _effect = "none";
            _dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
            _smask = "000";
        }

        public async Task Initialize(Bridge bridge, Schedule schedule = null)
        {
            _bridge = bridge;
            _currentHueObjectList = await HueObjectHelper.GetBridgeDataStoreAsyncTask(_bridge);
            
            if (_currentHueObjectList == null) return;
            ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Light).ToList());

            if (schedule == null) return;

            Header.Autodelete = schedule.autodelete;
           // Header.Datetime = schedule.localtime;
            Header.Description = schedule.description;
            Header.Name = schedule.name;
         
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
                body = Serializer.SerializeToJson(scpg.SelectedObject); 
            }
            else if(_selectedViewModel is ScheduleCreatorSlidersViewModel)
            {
                ScheduleCreatorSlidersViewModel scsv = _selectedViewModel as ScheduleCreatorSlidersViewModel;
                body = Serializer.SerializeToJson(scsv);
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

            switch (Content)
            {
                case ContentTypeVm.Light:
                    if (PropGridLG)
                    {
                        SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                    }
                    else
                    {
                        SelectedViewModel = new ScheduleCreatorSlidersViewModel();
                    }                    
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Light).ToList());
                    break;
                case ContentTypeVm.Group:
                    if (PropGridLG)
                    {
                        SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                    }
                    else
                    {
                        SelectedViewModel = new ScheduleCreatorSlidersViewModel();
                    }

                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Group).ToList());
                    break;
                case ContentTypeVm.Schedule:
                    SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Schedule).ToList());
                    break;
                case ContentTypeVm.Sensor:
                    SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Sensor).Where(x => ((Sensor)x).type.Contains("CLIP")).ToList());
                    break;
                case ContentTypeVm.Scene:
                    ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Scene).ToList());
                    break;
                default:
                    break;
            }

        }

    }
}
