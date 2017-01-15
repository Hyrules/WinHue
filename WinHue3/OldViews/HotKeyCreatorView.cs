using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using HueLib2;

namespace WinHue3
{
    public class HotKeyCreatorView : View
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<HueObject> _listHueObject;
        private HueObject _selectedHueObject;
        private CommonProperties _propertyObject;
        private ObservableCollection<HotKey> _listHotKeys;
        private Key _key;
        private ModifierKeys _modifierKeys;
        private HotKey _selectedHotKey;
        private DispatcherTimer _hotkeyrecordTimer = new DispatcherTimer();
        private bool _canrecordkeyup;
        private Brush _recordbuttoncolor = new SolidColorBrush() {Color =  Color.FromRgb(240, 240, 240) };
        private KeyEventArgs _recordedKeys;
        private string _name;
        private string _description;
        private int _objectypeindex;
        private bool _isgeneric;
        private readonly Bridge _bridge;

        //*********************************** CTOR **********************************************

        public HotKeyCreatorView(Bridge bridge)
        {
            _bridge = bridge;
            _listHotKeys = new ObservableCollection<HotKey>();
            _hotkeyrecordTimer.Interval = new TimeSpan(0,0,0,10);
            _hotkeyrecordTimer.Tick += _hotkeyrecordTimer_Tick;
            _canrecordkeyup = false;
            _isgeneric = false;
            _objectypeindex = -1;
            ListHotkeys = new ObservableCollection<HotKey>(WinHueSettings.settings.listHotKeys);
            SetError(GlobalStrings.Hotkey_SelectObjectType, "ObjectTypeIndex");
            SetError(GlobalStrings.Hotkey_SelectObject, "SelectedHueObject");
            SetError(GlobalStrings.Hotkey_RecordHotkey,"CurrentHotkey");
            
        }

        //********************************** PROPERTIES *****************************************

        public int ObjectTypeIndex
        {
            get
            {
                return _objectypeindex;
            }
            set
            {
                _objectypeindex = value;
                OnPropertyChanged();
                if (_objectypeindex != -1)
                {
                    FetchHueObject();
                    RemoveError(GlobalStrings.Hotkey_SelectObjectType);
                }
                else
                {
                    SetError(GlobalStrings.Hotkey_SelectObjectType);
                }
            }
        }

        public bool IsGeneric
        {
            get { return _isgeneric; }
            set
            {
                _isgeneric = value;
                OnPropertyChanged("CanSelectObjectAndType");
                if (_isgeneric)
                {
                    PropertyGridObject = new CommonProperties();
                    RemoveError(GlobalStrings.Hotkey_SelectObjectType, "ObjectTypeIndex");
                    RemoveError(GlobalStrings.Hotkey_SelectObject, "SelectedHueObject");
                }
                else
                {
                    SetError(GlobalStrings.Hotkey_SelectObjectType, "ObjectTypeIndex");
                    SetError(GlobalStrings.Hotkey_SelectObject, "SelectedHueObject");
                    PropertyGridObject = null;
                }
                OnPropertyChanged();
            }
        }

        public bool CanSelectObjectAndType => !_isgeneric;

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
                    hr = new HelperResult() {Success = false};
                    break;
            }

            if (hr.Success)
            {
                ListHueObject = (List<HueObject>) hr.Hrobject;
            }
        }

        public List<HueObject> ListHueObject
        {
            get
            {
                return _listHueObject;
            }
            set
            {
                _listHueObject = value;
                OnPropertyChanged();
            }
        }

        public HueObject SelectedHueObject
        {
            get
            {
                return _selectedHueObject;
                
            }
            set
            {
                _selectedHueObject = value;

                if (_selectedHueObject == null)
                {

                    SetError(GlobalStrings.Hotkey_SelectObject);
                }
                else
                {
                    switch (_objectypeindex)
                    {
                        case 2:
                            PropertyGridObject = null;
                            break;
                        default:
                            PropertyGridObject = new CommonProperties();
                            break;

                    }
                    RemoveError(GlobalStrings.Hotkey_SelectObject);
                }
                OnPropertyChanged();
            }

        }

        public CommonProperties PropertyGridObject
        {
            get
            {
                return _propertyObject;
            }
            set
            {
                _propertyObject = value;
                OnPropertyChanged();
            }
        }

        public HotKey SelectedHotkey
        {
            get
            {
                return _selectedHotKey;
            }
            set
            {
                _selectedHotKey = value;
                if (_selectedHotKey != null)
                {
                    _modifierKeys = _selectedHotKey.Modifier;
                    Key = _selectedHotKey.Key;
                    OnPropertyChanged("CurrentHotkey");

                    HotKeyName = _selectedHotKey.Name;
                    HotKeyDescription = _selectedHotKey.Description;
                    
                    if (_selectedHotKey.objecType != null)
                    {
                        IsGeneric =false;
                        OnPropertyChanged("CanSelectObjectAndType");

                        if (_selectedHotKey.objecType == typeof(Light))
                        {
                            ObjectTypeIndex = 0;
                        }
                        else if (_selectedHotKey.objecType == typeof(Group))
                        {
                            ObjectTypeIndex = 1;
                        }
                        else
                        {
                            ObjectTypeIndex = 2;
                        }

                        try
                        {
                            SelectedHueObject = _listHueObject.First(x => x.Id == _selectedHotKey.id);
                        }
                        catch (InvalidOperationException)
                        {
                            log.Warn("Hotkey target object does not exists");
                            SelectedHueObject = null;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            SelectedHueObject = null;
                        }

                    }
                    else
                    {
                        ObjectTypeIndex = -1;
                        SelectedHueObject = null;
                        IsGeneric = true;

                    }
                    
                    PropertyGridObject = _selectedHotKey.properties;
                    RemoveError(GlobalStrings.Hotkey_RecordHotkey, "CurrentHotkey");
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HotKey> ListHotkeys
        {
            get { return _listHotKeys; }
            set
            {
                _listHotKeys = value;
                OnPropertyChanged();
            }
        }

        public Brush RecordButtonColor
        {
            get
            {
                return _recordbuttoncolor;
            }
            set
            {
                _recordbuttoncolor = value;
                OnPropertyChanged();
            }
        }

        public Key Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }

        public bool canRecordKeyUp
        {
            get { return _canrecordkeyup; }
            internal set
            {
                _canrecordkeyup = value;
                OnPropertyChanged();
            }
        }

        public string CurrentHotkey
        {
            get
            {
                if (_key == Key.None) return "None";
                return (_modifierKeys == ModifierKeys.None) ? _key.ToString() : _modifierKeys  + " + " + _key;
            } 
        }

        public string HotKeyName
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }

        }

        public string HotKeyDescription
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        //**************************** METHODS ********************************************

        private void AddHotkey()
        {
            if (ValidateHotKeyProperties())
            {
                HotKey hotkey = new HotKey()
                {
                    Modifier = _modifierKeys,
                    Key = _key,
                    properties = _propertyObject,
                    Name = _name,
                    Description = _description,
                };

                if (_selectedHueObject != null)
                {
                    hotkey.id = _selectedHueObject.Id;

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
                        _listHotKeys.Add(hotkey);
                        log.Info($"Adding new hotkey : {hotkey}");
                        Clearfields();
                    }
                    else
                    {
                        MessageBox.Show(GlobalStrings.Error_Hotkey_Already_Assigned, GlobalStrings.Error,MessageBoxButton.OK, MessageBoxImage.Error);
                        log.Error("Hotkey already assigned by another process. Please select another key combo.");                        
                    }
                }
                else
                {
                    if (
                        MessageBox.Show(GlobalStrings.HotKey_Already_Exists, GlobalStrings.Warning,
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        _listHotKeys.Remove(existingKey);
                        _listHotKeys.Add(hotkey);
                        log.Info($"Replacing hotkey with : {hotkey}");
                        Clearfields();
                    }
                }

                OnPropertyChanged("ListHotkeys");
            }
            else
            {
                MessageBox.Show(GlobalStrings.Hotkey_FillOneProperty, GlobalStrings.Error, MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Clearfields()
        {
            SelectedHueObject = null;
            SelectedHotkey = null;
            ObjectTypeIndex = -1;
            PropertyGridObject = null;
            HotKeyName = string.Empty;
            HotKeyDescription = string.Empty;
            ListHueObject = null;
            ObjectTypeIndex = -1;
            _key = Key.None;
            _isgeneric = false;
            OnPropertyChanged("IsGeneric");
            OnPropertyChanged("CurrentHotkey");
            SetError(GlobalStrings.Hotkey_RecordHotkey, "CurrentHotkey");
        }

        private bool HotkeyAlreadyExists(HotKey h,out HotKey existingKey)
        {
            bool exists = false;
            existingKey = null;
            foreach (HotKey hk in _listHotKeys)
            {
                if (hk.Modifier != h.Modifier || hk.Key != h.Key) continue;
                exists = true;
                existingKey = hk;
            }
            return exists;

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

        public void RecordHotKey()
        {
            _hotkeyrecordTimer.Start();
            _canrecordkeyup = true;
            RecordButtonColor = new SolidColorBrush() {Color = Color.FromRgb(255,0,0)};
        }

        private void _hotkeyrecordTimer_Tick(object sender, EventArgs e)
        {
            StopRecording();
           
        }

        public void SaveHotKeys()
        {
            WinHueSettings.settings.listHotKeys = _listHotKeys.ToList();
            WinHueSettings.Save();
        }

        public void CaptureHotkey(KeyEventArgs e)
        {
            StopRecording();
            _modifierKeys = e.KeyboardDevice.Modifiers;
            _key = e.Key;
            OnPropertyChanged("CurrentHotKey");
            RemoveError(GlobalStrings.Hotkey_RecordHotkey, "CurrentHotkey");
        }

        private void StopRecording()
        {
            _hotkeyrecordTimer.Stop();
            _canrecordkeyup = false;
            RecordButtonColor = new SolidColorBrush() { Color = Color.FromRgb(240, 240, 240) };
        }

        private void DeleteHotkey()
        {
            _listHotKeys.Remove(_selectedHotKey);
            OnPropertyChanged("ListHotKeys");
        }

        //********************************* COMMANDS ******************************************

        public ICommand AddHotkeyCommand => new RelayCommand(param => AddHotkey());
        public ICommand RecordHotKeyCommand => new RelayCommand(param => RecordHotKey());
        public ICommand DeleteHotKeyCommand => new RelayCommand(param => DeleteHotkey());
        public ICommand ClearFieldsCommand => new RelayCommand(param => Clearfields());
    }
}
