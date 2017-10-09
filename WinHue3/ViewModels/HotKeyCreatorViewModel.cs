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
using WinHue3.ExtensionMethods;
using WinHue3.Hotkeys;
using WinHue3.Models;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Settings;
using WinHue3.Utils;
using WinHue3.Validation;


namespace WinHue3.ViewModels
{
    
    public class HotKeyCreatorViewModel : ValidatableBindableBase
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ObservableCollection<IHueObject> _listHueObject;
        private IHueObject _selectedHueObject;
        private IBaseProperties _propertyObject;
        private ObservableCollection<HotKey> _listHotKeys;
        private readonly DispatcherTimer _hotkeyrecordTimer;
        private Bridge _bridge;
        private HotKeyCreatorModel _hotKeyModel;
        private HotKey _selectedHotKey;
        private Type _objectype;
        private bool _canRecordKeyUp;
        private bool _isGeneric;

        public HotKeyCreatorViewModel()
        {
            HotKeyModel = new HotKeyCreatorModel();
            _hotkeyrecordTimer = new DispatcherTimer();
            ListHotKeys = new ObservableCollection<HotKey>();
            _hotkeyrecordTimer.Interval = new TimeSpan(0, 0, 0, 10);
            _hotkeyrecordTimer.Tick += _hotkeyrecordTimer_Tick;
            ListHotKeys = new ObservableCollection<HotKey>(WinHueSettings.hotkeys.listHotKeys);
            ObjectType = typeof(Light);
        }

        public bool NotGeneric => !_isGeneric;

        public async Task Initialize(Bridge bridge)
        {
            CanRecordKeyUp = false;
            _bridge = bridge;
            await FetchHueObject();
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
                PropertyGridObject = _selectedHueObject != null ? BasePropertiesCreator.CreateBaseProperties(_objectype) : null;
            }
        }

        public HotKey SelectedHotKey
        {
            get => _selectedHotKey;
            set
            {
                SetProperty(ref _selectedHotKey, value);
                if (SelectedHotKey == null) return;
                HotKeyModel.Name = value.Name;
                HotKeyModel.Description = value.Description;
                HotKeyModel.Key = value.Key;
                HotKeyModel.ModifierKeys = value.Modifier;
                HotKeyModel.Id = value.id;

                if (value.objecType != null)
                {
                    ObjectType = value.objecType;
                    SelectedHueObject = ListHueObject.FirstOrDefault(x => x.Id == value.id);                   

                }
                else
                {
                    IsGeneric = true;
                }
                PropertyGridObject = value.properties;
                RaisePropertyChanged("CurrentHotKey");
            }
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
            HotKeyModel.Key = e.Key;
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

        private async Task FetchHueObject()
        {
            if (ObjectType == null || _bridge == null)
            {
                ListHueObject = null;
                return;
            }

           
            if (ObjectType == typeof(Light))
            {
                List<Light> hr = await HueObjectHelper.GetBridgeLightsAsyncTask(_bridge);
                if (hr != null)
                {
                    ListHueObject = new ObservableCollection<IHueObject>(hr);
                }   

            }
            else if (ObjectType == typeof(Group))
            {
                List<Group> hr = await HueObjectHelper.GetBridgeGroupsAsyncTask(_bridge);
                if (hr != null)
                {
                    ListHueObject = new ObservableCollection<IHueObject>(hr);
                }
            }
            else if( ObjectType == typeof(Scene))
            {
                List<Scene> hr = await HueObjectHelper.GetBridgeScenesAsyncTask(_bridge);
                if (hr != null)
                {
                    ListHueObject = new ObservableCollection<IHueObject>(hr);
                }
            }
        }

        private bool ValidateHotKeyProperties()
        {
            bool valid = false;
            if (_propertyObject != null)
            {
                PropertyInfo[] prop = _propertyObject.GetType().GetHueProperties();
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
                    Description = HotKeyModel.Description                  
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
            ListHueObject = null;
            IsGeneric = false;
            RaisePropertyChanged("CurrentHotKey");
        }

        private bool CanAddHotKey()
        {
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
        public ICommand ChangeObjectTypeCommand => new AsyncRelayCommand(param => ChangeObject());

        private async Task ChangeObject()
        {
            if (_objectype != null)
            {
                await FetchHueObject();
            }
        }
    }
}
