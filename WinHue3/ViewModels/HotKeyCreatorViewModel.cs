using HueLib2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Build.Framework;
using WinHue3.Models;
using WinHue3.Validation;

namespace WinHue3.ViewModels
{
    
    public class HotKeyCreatorViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<HueObject> _listHueObject;
        private HueObject _selectedHueObject;
        private CommonProperties _propertyObject;
        private ObservableCollection<HotKey> _listHotKeys;
        private readonly DispatcherTimer _hotkeyrecordTimer;
        private Bridge _bridge;
        private HotKeyCreatorModel _hotKeyModel;
        private HotKey _selectedHotKey;
        private int _objectypeindex;
        private bool _canRecordKeyUp;
        private bool _isGeneric;

        public HotKeyCreatorViewModel()
        {
            HotKeyModel = new HotKeyCreatorModel();
            _hotkeyrecordTimer = new DispatcherTimer();
            ListHotKeys = new ObservableCollection<HotKey>();
            _hotkeyrecordTimer.Interval = new TimeSpan(0, 0, 0, 10);
            _hotkeyrecordTimer.Tick += _hotkeyrecordTimer_Tick;
            ListHotKeys = new ObservableCollection<HotKey>(WinHueSettings.settings.listHotKeys);
            ObjectTypeIndex = -1;
        }


        public void Initialize(Bridge bridge)
        {
            CanRecordKeyUp = false;
            _bridge = bridge;            
        }

        public CommonProperties PropertyGridObject
        {
            get { return _propertyObject; }
            set { SetProperty(ref _propertyObject,value); }
        }
        public bool IsGeneric
        {
            get { return _isGeneric; }
            set
            {
                SetProperty(ref _isGeneric, value);
                PropertyGridObject = value ? new CommonProperties() : null;
                OnPropertyChanged("SelectedHueObject");
            }
        }

        public string CurrentHotKey => HotKeyModel.Key == default(Key) && HotKeyModel.ModifierKeys == default(ModifierKeys) ? string.Empty : $"{HotKeyModel.ModifierKeys} + {HotKeyModel.Key}";

        public int ObjectTypeIndex
        {
            get { return _objectypeindex; }
            set
            {
                SetProperty(ref _objectypeindex,value);
                if (_objectypeindex != -1)
                {
                    FetchHueObject();
                }
            }
        }

        public ObservableCollection<HotKey> ListHotKeys
        {
            get { return _listHotKeys; }
            set { SetProperty(ref _listHotKeys,value); }
        }

        public HotKeyCreatorModel HotKeyModel
        {
            get { return _hotKeyModel; }
            set { SetProperty(ref _hotKeyModel, value); }
        }

        public void RecordHotKey()
        {
            _hotkeyrecordTimer.Start();
            CanRecordKeyUp = true;
            HotKeyModel.RecordButtonColor = new SolidColorBrush() { Color = Color.FromRgb(255, 0, 0) };
        }

        public List<HueObject> ListHueObject
        {
            get { return _listHueObject; }
            set { SetProperty(ref _listHueObject, value); }
        }

        [HotKeySelectedValidation(ErrorMessageResourceName = "Hotkey_SelectObject", ErrorMessageResourceType = typeof(GlobalStrings))]
        public HueObject SelectedHueObject
        {
            get { return _selectedHueObject; }
            set
            {
                SetProperty(ref _selectedHueObject, value);
                PropertyGridObject = _selectedHueObject != null ? new CommonProperties() : null;
            }
        }

        public HotKey SelectedHotKey
        {
            get { return _selectedHotKey; }
            set
            {
                SetProperty(ref _selectedHotKey, value);
                if (SelectedHotKey == null) return;
                HotKeyModel.Name = value.Name;
                HotKeyModel.Description = value.Description;
                HotKeyModel.Key = value.Key;
                HotKeyModel.ModifierKeys = value.Modifier;
                if (value.objecType != null)
                {
                    if (value.objecType == typeof(Light))
                    {
                        ObjectTypeIndex = 0;
                    }
                    else if (value.objecType == typeof(Group))
                    {
                        ObjectTypeIndex = 1;
                    }
                    else
                    {
                        ObjectTypeIndex = 2;
                    }
                    SelectedHueObject = _listHueObject.Find(x => x.Id == value.id);
                   

                }
                else
                {
                    IsGeneric = true;
                }
                PropertyGridObject = value.properties;
                OnPropertyChanged("CurrentHotKey");
            }
        }

        public bool CanRecordKeyUp
        {
            get { return _canRecordKeyUp; }
            set { SetProperty(ref _canRecordKeyUp, value); }
        }

        public void SaveHotKeys()
        {
            WinHueSettings.settings.listHotKeys = _listHotKeys.ToList();
            WinHueSettings.Save();
        }

        public void CaptureHotkey(KeyEventArgs e)
        {
            StopRecording();
            CanRecordKeyUp = false;
            HotKeyModel.ModifierKeys = e.KeyboardDevice.Modifiers;
            HotKeyModel.Key = e.Key;
            OnPropertyChanged("CurrentHotKey");
        }

        private void StopRecording()
        {
            _hotkeyrecordTimer.Stop();
            HotKeyModel.RecordButtonColor = new SolidColorBrush() { Color = Color.FromRgb(240, 240, 240) };
        }

        private void DeleteHotkey()
        {
            ListHotKeys.Remove(SelectedHotKey);
            OnPropertyChanged("ListHotKeys");
        }

        private void _hotkeyrecordTimer_Tick(object sender, EventArgs e)
        {
            StopRecording();
        }

        private void FetchHueObject()
        {
            if (_objectypeindex == -1)
            {
                ListHueObject = null;
                return;
            }

            HelperResult hr;

            switch (_objectypeindex)
            {
                case 0:
                    hr = HueObjectHelper.GetBridgeLights(_bridge);
                    break;
                case 1:
                    hr = HueObjectHelper.GetBridgeGroups(_bridge);
                    break;
                case 2:
                    hr = HueObjectHelper.GetBridgeScenes(_bridge);
                    break;
                default:
                    hr = new HelperResult() { Success = false };
                    break;
            }

            if (hr.Success)
            {
                ListHueObject = (List<Light>)hr.Hrobject;
            }
        }

        public bool ValidateHotKeyProperties()
        {
            bool valid = false;
            if (_propertyObject != null)
            {
                PropertyInfo[] prop = _propertyObject.GetType().GetProperties();
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
                HotKey hotkey = new HotKey()
                {
                    Modifier = HotKeyModel.ModifierKeys,
                    Key = HotKeyModel.Key,
                    properties = _propertyObject,
                    Name = HotKeyModel.Name,
                    Description = HotKeyModel.Description,
                };

                if (!IsGeneric)
                {
                    hotkey.id = SelectedHueObject.Id;

                    switch (_objectypeindex)
                    {
                        case 0:
                            hotkey.objecType = typeof(Light);
                            break;
                        case 1:
                            hotkey.objecType = typeof(Group);
                            break;
                        case 2:
                            hotkey.objecType = typeof(Scene);
                            break;
                        default:
                            return;

                    }
                }

                HotKey existingKey;

                if (!HotkeyAlreadyExists(hotkey, out existingKey))
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

                OnPropertyChanged("ListHotKeys");
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
            ObjectTypeIndex = -1;
            PropertyGridObject = null;
            HotKeyModel.Name = string.Empty;
            HotKeyModel.Description = string.Empty;
            ListHueObject = null;
            ObjectTypeIndex = -1;
            HotKeyModel.Key = Key.None;
 
        }

        private bool CanAddHotKey()
        {
            if (IsGeneric)
                return HotKeyModel.Name != string.Empty && HotKeyModel.Key != default(Key) &&
                       HotKeyModel.ModifierKeys != default(ModifierKeys);
            else
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

        public ICommand AddHotkeyCommand => new RelayCommand(param => AddHotkey(),(param) => CanAddHotKey());
        public ICommand RecordHotKeyCommand => new RelayCommand(param => RecordHotKey(), (param) => CanRecord());
        public ICommand DeleteHotKeyCommand => new RelayCommand(param => DeleteHotkey(), (param)=> IsObjectSelected());
        public ICommand ClearFieldsCommand => new RelayCommand(param => Clearfields());


    }
}
