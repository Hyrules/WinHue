using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using log4net;
using Microsoft.Win32;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.HotKeys.Validation;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.HotKeys.Creator
{
    
    public class HotKeyCreatorViewModel : ValidatableBindableBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private List<IHueObject> _listAvailbleHueObjects;
        private ObservableCollection<IHueObject> _listHueObject;
        private IHueObject _selectedHueObject;
        private IBaseProperties _propertyObject;
        private ObservableCollection<HotKey> _listHotKeys;
        private readonly DispatcherTimer _hotkeyrecordTimer;
        private HotKeyCreatorModel _hotKeyModel;
        private HotKey _selectedHotKey;
        private Type _objectype;
        private bool _canRecordKeyUp;
        private bool _isGeneric;
        private OpenFileDialog ofd;
        private Bridge _bridge;

        public HotKeyCreatorViewModel()
        {
            _hotKeyModel = new HotKeyCreatorModel();
            _hotkeyrecordTimer = new DispatcherTimer();
            _listHotKeys = new ObservableCollection<HotKey>();
            _hotkeyrecordTimer.Interval = new TimeSpan(0, 0, 0, 10);
            _hotkeyrecordTimer.Tick += _hotkeyrecordTimer_Tick;
            ofd = new OpenFileDialog
            {
                DefaultExt = "exe",
                Multiselect = false,
                Filter = "Executable files (*.exe)|*.exe|Batch files (*.bat)|*.bat|Command files(*.cmd)|*.cmd"
            };  
        }

        public bool NotGeneric => !_isGeneric;

        public async Task Initialize(Bridge bridge, ObservableCollection<HotKey> listHotkeys)
        {
            _bridge = bridge;
            ListHotKeys = listHotkeys;
            CanRecordKeyUp = false;
            _listAvailbleHueObjects = await _bridge.GetAllObjectsAsync();
        }

        public IBaseProperties PropertyGridObject
        {
            get => _propertyObject;
            set => SetProperty(ref _propertyObject,value);
        }
        public bool IsGeneric
        {
            get => _isGeneric;
            set
            {
                SetProperty(ref _isGeneric, value);
                PropertyGridObject = value ? BasePropertiesCreator.CreateBaseProperties(_objectype) : null;
                RaisePropertyChanged("SelectedHueObject");
                RaisePropertyChanged("NotGeneric");
            }
        }

        public string CurrentHotKey => HotKeyModel.Key == default(Key) && HotKeyModel.ModifierKeys == default(ModifierKeys) ? string.Empty : $"{HotKeyModel.ModifierKeys} + {HotKeyModel.Key}";

        public Type ObjectType
        {
            get => _objectype;
            set => SetProperty(ref _objectype,value);
        }

        public ObservableCollection<HotKey> ListHotKeys
        {
            get => _listHotKeys;
            set => SetProperty(ref _listHotKeys,value);
        }

        public HotKeyCreatorModel HotKeyModel
        {
            get => _hotKeyModel;
            set => SetProperty(ref _hotKeyModel, value);
        }

        private void RecordHotKey()
        {
            _hotkeyrecordTimer.Start();
            CanRecordKeyUp = true;
            HotKeyModel.RecordButtonColor = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(255, 0, 0) };
        }

        public ObservableCollection<IHueObject> ListHueObject
        {
            get => _listHueObject;
            set => SetProperty(ref _listHueObject, value);
        }

        [HotKeySelectedValidation(ErrorMessageResourceName = "Hotkey_SelectObject", ErrorMessageResourceType = typeof(GlobalStrings))]
        public IHueObject SelectedHueObject
        {
            get => _selectedHueObject;
            set
            {
                SetProperty(ref _selectedHueObject, value);
                PropertyGridObject = _selectedHueObject != null && !(_objectype ==  typeof(Scene) )  ? BasePropertiesCreator.CreateBaseProperties(_objectype) : null;
            }
        }

        public HotKey SelectedHotKey
        {
            get => _selectedHotKey;
            set => SetProperty(ref _selectedHotKey, value);
        }

        public bool CanRecordKeyUp
        {
            get => _canRecordKeyUp;
            set => SetProperty(ref _canRecordKeyUp, value);
        }

        public void SaveHotKeys()
        {
            WinHueSettings.hotkeys.listHotKeys = _listHotKeys.ToList();
            WinHueSettings.SaveHotkeys();
        }

        public void CaptureHotkey(KeyEventArgs e)
        {
            StopRecording();
            CanRecordKeyUp = false;
            HotKeyModel.ModifierKeys = e.KeyboardDevice.Modifiers;
            HotKeyModel.Key = e.KeyboardDevice.Modifiers == ModifierKeys.Alt ? e.SystemKey : e.Key;
            
            RaisePropertyChanged("CurrentHotKey");
        }

        private void StopRecording()
        {
            _hotkeyrecordTimer.Stop();
            HotKeyModel.RecordButtonColor = new SolidColorBrush { Color = System.Windows.Media.Color.FromRgb(240, 240, 240) };
        }

        private void DeleteHotkey()
        {           
            ListHotKeys.Remove(SelectedHotKey);
            RaisePropertyChanged("ListHotKeys");
        }


        private void _hotkeyrecordTimer_Tick(object sender, EventArgs e)
        {
            StopRecording();
        }

        private bool ValidateHotKeyProperties()
        {
            bool valid = false;
            if (_propertyObject != null)
            {
                PropertyInfo[] prop = _propertyObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo p in prop)
                {
                    if (p.GetValue(_propertyObject) != null)
                        valid = true;
                }
            }
            else
            {
                valid = true;
            }
            return valid;
        }

        private void AddHotkey()
        {
            if (ValidateHotKeyProperties())
            {
                HotKey hotkey = new HotKey
                {
                    Modifier = HotKeyModel.ModifierKeys,
                    Key = HotKeyModel.Key,
                    properties = _propertyObject,
                    Name = HotKeyModel.Name,
                    Description = HotKeyModel.Description,
                    ProgramPath = HotKeyModel.ProgramPath
                };

                if (!IsGeneric)
                {
                    hotkey.id = SelectedHueObject.Id;
                    hotkey.objecType = ObjectType;

                }
                if (!HotkeyAlreadyExists(hotkey, out HotKey existingKey))
                {
                    HotKeyHandle hkh = new HotKeyHandle(hotkey, null);
                    if (hkh.Register())
                    {
                        hkh.Unregister();
                        ListHotKeys.Add(hotkey);
                        log.Info($"Adding new hotkey : {hotkey}");
                        Clearfields();
                    }
                    else
                    {
                        MessageBox.Show(GlobalStrings.Error_Hotkey_Already_Assigned, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        log.Error("Hotkey already assigned by another process. Please select another key combo.");
                    }
                }
                else
                {
                    if (
                        MessageBox.Show(GlobalStrings.HotKey_Already_Exists, GlobalStrings.Warning,
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        ListHotKeys.Remove(existingKey);
                        ListHotKeys.Add(hotkey);
                        log.Info($"Replacing hotkey with : {hotkey}");
                        Clearfields();
                    }
                }
                RaisePropertyChanged("ListHotKeys");
            }
            else
            {
                MessageBox.Show(GlobalStrings.Hotkey_FillOneProperty, GlobalStrings.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool HotkeyAlreadyExists(HotKey h, out HotKey existingKey)
        {
            bool exists = false;
            existingKey = null;
            foreach (HotKey hk in ListHotKeys)
            {
                if (hk.Modifier != h.Modifier || hk.Key != h.Key) continue;
                exists = true;
                existingKey = hk;
            }
            return exists;

        }

        private void Clearfields()
        {
            SelectedHueObject = null;
            SelectedHotKey = null;
            
            ObjectType = null;
            PropertyGridObject = null;
            HotKeyModel.Name = string.Empty;
            HotKeyModel.Description = string.Empty;
            HotKeyModel.Key = Key.None;
            HotKeyModel.ModifierKeys = ModifierKeys.None;
            HotKeyModel.ProgramPath = null;
            ListHueObject = null;
            IsGeneric = false;
            RaisePropertyChanged("CurrentHotKey");
        }

        private bool CanAddHotKey()
        {
            if (HotKeyModel.Name.Length < 1 || HotKeyModel.Name.Length > 30) return false;
            if (IsGeneric)
                return HotKeyModel.Name != string.Empty && HotKeyModel.Key != default(Key) &&
                       HotKeyModel.ModifierKeys != default(ModifierKeys);
            return HotKeyModel.Name != string.Empty && IsObjectSelected() && HotKeyModel.Key != default(Key) &&
                   HotKeyModel.ModifierKeys != default(ModifierKeys);
        }

        private bool IsObjectSelected()
        {
            return SelectedHueObject != null;
        }

        private bool CanRecord()
        {
            return !CanRecordKeyUp;
        }

        public ICommand AddHotkeyCommand => new RelayCommand(param => AddHotkey(),param => CanAddHotKey());
        public ICommand RecordHotKeyCommand => new RelayCommand(param => RecordHotKey(), param => CanRecord());
        public ICommand DeleteHotKeyCommand => new RelayCommand(param => DeleteHotkey(), param=> IsObjectSelected());
        public ICommand ClearFieldsCommand => new RelayCommand(param => Clearfields());
        public ICommand ChangeObjectTypeCommand => new RelayCommand(param => ChangeObject());
        public ICommand SelectExistingHotkeyCommand => new RelayCommand(param => SelectExistingHotkey());
        public ICommand ChooseProgramCommand => new RelayCommand(param => ChooseProgram());
        public ICommand RemoveProgramCommand => new RelayCommand(param => RemoveProgram(), (param) => HotKeyModel.ProgramPath != null);

        private void RemoveProgram()
        {
            HotKeyModel.ProgramPath = null;
        }

        private void ChooseProgram()
        {


            if (ofd.ShowDialog() == true)
            {
                HotKeyModel.ProgramPath = ofd.FileName;
            }
        }

        private void SelectExistingHotkey()
        {
            if (SelectedHotKey == null) return;
            HotKeyModel.Name = SelectedHotKey.Name;
            HotKeyModel.Description = SelectedHotKey.Description;
            HotKeyModel.Key = SelectedHotKey.Key;
            HotKeyModel.ModifierKeys = SelectedHotKey.Modifier;
            HotKeyModel.Id = SelectedHotKey.id;
            HotKeyModel.ProgramPath = SelectedHotKey.ProgramPath;
            if (HotKeyModel.ProgramPath != null)
            {
                ofd.FileName = HotKeyModel.ProgramPath;
            }
            if (SelectedHotKey.objecType != null)
            {
                ObjectType = SelectedHotKey.objecType;
                SelectedHueObject = ListHueObject.FirstOrDefault(x => x.Id == SelectedHotKey.id);

            }
            else
            {
                IsGeneric = true;
            }
            PropertyGridObject = SelectedHotKey.properties;
            RaisePropertyChanged("CurrentHotKey");
        }

        private void ChangeObject()
        {
            switch (ObjectType)
            {
                case Type light when light == typeof(Light):
                    ListHueObject = new ObservableCollection<IHueObject>(_listAvailbleHueObjects.OfType<Light>().ToList<IHueObject>());

                    break;
                case Type group when group == typeof(Group):
                    ListHueObject = new ObservableCollection<IHueObject>(_listAvailbleHueObjects.OfType<Group>().ToList<IHueObject>());

                    break;

                case Type scene when scene == typeof(Scene):
                    ListHueObject = new ObservableCollection<IHueObject>(_listAvailbleHueObjects.OfType<Scene>().ToList<IHueObject>());

                    break;
                default:
                    ListHueObject = null;
                    break;

            }
        }
    }
}
