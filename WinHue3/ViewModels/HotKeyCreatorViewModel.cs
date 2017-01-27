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
using WinHue3.Models;

namespace WinHue3.ViewModels
{
    
    public class HotKeyCreatorViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<HueObject> _listHueObject;
        private HueObject _selectedHueObject;
        private CommonProperties _propertyObject;
        private ObservableCollection<HotKey> _listHotKeys;
        private DispatcherTimer _hotkeyrecordTimer;
        private Bridge _bridge;
        private HotKeyCreatorModel _hotKeyModel;
        private HotKey _selectedHotKey;
        private int _objectypeindex;
        private bool _canRecordKeyUp;

        public HotKeyCreatorViewModel()
        {
            _hotkeyrecordTimer = new DispatcherTimer();
        }

        public void Initialize(Bridge bridge)
        {
            _bridge = bridge;
            _listHotKeys = new ObservableCollection<HotKey>();
            _hotkeyrecordTimer.Interval = new TimeSpan(0, 0, 0, 10);
            _hotkeyrecordTimer.Tick += _hotkeyrecordTimer_Tick;
            ListHotKeys = new ObservableCollection<HotKey>(WinHueSettings.settings.listHotKeys);
        }

        public CommonProperties PropertyGridObject
        {
            get { return _propertyObject; }
            set { SetProperty(ref _propertyObject,value); }
        }

        public int ObjectTypeIndex
        {
            get { return _objectypeindex; }
            set
            {
                SetProperty(ref _objectypeindex,value);
                OnPropertyChanged();
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
            HotKeyModel.RecordButtonColor = new SolidColorBrush() { Color = Color.FromRgb(255, 0, 0) };
        }

        public List<HueObject> ListHueObject
        {
            get { return _listHueObject; }
            set { SetProperty(ref _listHueObject, value); }
        }

        public HueObject SelectedHueObject
        {
            get { return _selectedHueObject; }
            set { SetProperty(ref _selectedHueObject, value); }
        }

        public HotKey SelectedHotKey
        {
            get { return _selectedHotKey; }
            set { SetProperty(ref _selectedHotKey, value); }
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
                ListHueObject = (List<HueObject>)hr.Hrobject;
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

                if (SelectedHueObject != null)
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
                        _listHotKeys.Add(hotkey);
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

        private bool HotkeyAlreadyExists(HotKey h, out HotKey existingKey)
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

        public bool IsObjectSelected()
        {
            return SelectedHueObject != null;
        }

        public ICommand AddHotkeyCommand => new RelayCommand(param => AddHotkey());
        public ICommand RecordHotKeyCommand => new RelayCommand(param => RecordHotKey());
        public ICommand DeleteHotKeyCommand => new RelayCommand(param => DeleteHotkey(), (param)=> IsObjectSelected());
        public ICommand ClearFieldsCommand => new RelayCommand(param => Clearfields());


    }
}
