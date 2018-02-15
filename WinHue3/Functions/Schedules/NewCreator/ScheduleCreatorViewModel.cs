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
using WinHue3.Utils;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public enum ContentTypeVm { Sensor, Sliders };

    public class ScheduleCreatorViewModel : ValidatableBindableBase
    {

        private ObservableCollection<IHueObject> _listTargetHueObject;
        private ValidatableBindableBase _selectedViewModel;
        private Bridge _bridge;
        private ScheduleCreatorHeader _header;
        private ContentTypeVm _content;
        private DataStore _ds;
        private List<IHueObject> _currentHueObjectList;

        public ScheduleCreatorViewModel()
        {
            _header = new ScheduleCreatorHeader();
            _selectedViewModel = new ScheduleCreatorSlidersViewModel();
            _content = ContentTypeVm.Sliders;
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
            get { return _listTargetHueObject; }
            set { SetProperty(ref _listTargetHueObject,value); }
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
            else
            {
                SelectedViewModel = new ScheduleCreatorSensorsViewModel();
                ListTargetHueObject = new ObservableCollection<IHueObject>();
                if (_currentHueObjectList == null) return;
                ListTargetHueObject.AddRange(_currentHueObjectList.Where(x => x is Sensor).ToList());
            }
                
        }

    }
}
