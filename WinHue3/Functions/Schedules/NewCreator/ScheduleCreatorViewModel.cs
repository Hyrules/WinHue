using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public enum ContentTypeVm { Sensor, Sliders, Schedule };

    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {

        private ObservableCollection<IHueObject> _listTargetHueObject;
        private ValidatableBindableBase _selectedViewModel;
        private Bridge _bridge;
        private ScheduleCreatorHeader _header;
        private ContentTypeVm _content;
        private List<IHueObject> _currentHueObjectList;
        private IHueObject _selectHueObject;
        private string _effect;
        private string _dateTimeFormat;

        public ICommand SelectTargetObject => new RelayCommand(param => SelectTarget());
        public ICommand ChangeDateTimeFormat => new RelayCommand(param => ChangeDateTime());

        private void ChangeDateTime()
        {
            if (Header.ScheduleType == "alarm" || Header.ScheduleType == "timer")
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
        }

        public ScheduleCreatorViewModel()
        {
            _header = new ScheduleCreatorHeader();
            _selectedViewModel = new ScheduleCreatorSlidersViewModel();
            _content = ContentTypeVm.Sliders;
            _effect = "none";
            _dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            _currentHueObjectList = await HueObjectHelper.GetBridgeDataStoreAsyncTask(_bridge);
            ListTargetHueObject = new ObservableCollection<IHueObject>();
            if (_currentHueObjectList == null) return;
            ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Light).ToList()); 
            ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Group).ToList());

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
            get => _selectHueObject;
            set => SetProperty(ref _selectHueObject,value);
        }

        public string Effect
        {
            get { return _effect; }
            set { SetProperty(ref _effect,value); }
        }

        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }
            set { SetProperty(ref _dateTimeFormat,value); }
        }

        private void ChangeContent()
        {

            if (Content == ContentTypeVm.Sliders)
            {
                SelectedViewModel = new ScheduleCreatorSlidersViewModel();
                ListTargetHueObject = new ObservableCollection<IHueObject>();
                if (_currentHueObjectList == null) return;
                ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Light).ToList());
                ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Group).ToList());
            }
            else if (Content == ContentTypeVm.Sensor)
            {
                SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                ListTargetHueObject = new ObservableCollection<IHueObject>();
                if (_currentHueObjectList == null) return;
                ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Sensor).Where(x => ((Sensor)x).type.Contains("CLIP")).ToList());
            }
            else
            {
                SelectedViewModel = new ScheduleCreatorPropertyGridViewModel();
                ListTargetHueObject = new ObservableCollection<IHueObject>();
                if (_currentHueObjectList == null) return;
                ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Schedule).ToList());
            }
        }

    }
}
