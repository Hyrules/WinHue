using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.ExtensionMethods;
using WinHue3.Hotkeys;
using WinHue3.Interface;
using WinHue3.Models;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Philips_Hue.HueObjects.SensorObject;
using WinHue3.Philips_Hue.HueObjects.SensorObject.Daylight;
using WinHue3.Philips_Hue.HueObjects.SensorObject.HueMotion;
using WinHue3.Settings;
using WinHue3.Utils;
using WinHue3.Views;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.ViewModels.MainFormViewModels
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        #region BRIDGE_SETTINGS
        private async Task ChangeBridgeSettings()
        {
            Form_BridgeSettings frs = new Form_BridgeSettings { Owner = Application.Current.MainWindow };
            await frs.Initialize(SelectedBridge);
            if (frs.ShowDialog() == true)
            {
                BridgeSettings bresult = await SelectedBridge.GetBridgeSettingsAsyncTask();
                if (bresult != null)
                {
                    string newname = bresult.name;
                    if (SelectedBridge.name != newname)
                    {
                        SelectedBridge.name = newname;
                        ListBridges[ListBridges.IndexOf(SelectedBridge)].name = newname;
                        Bridge selbr = SelectedBridge;
                        SelectedBridge = null;
                        SelectedBridge = selbr;
                    }
                    await RefreshView();
                    RaisePropertyChanged("UpdateAvailable");
                }
            }
        }

        private async Task ChangeBridge()
        {
            await RefreshView();
            RaisePropertyChanged("UpdateAvailable");
        }

        private async Task ManageUsers()
        {
            Form_ManageUsers fmu = new Form_ManageUsers() { Owner = Application.Current.MainWindow };
            await fmu.Initialize(SelectedBridge);
            fmu.ShowDialog();
        }

        private bool SaveSettings()
        {
            foreach (Bridge br in ListBridges)
            {
                if (br.Mac == string.Empty) continue;
                if (WinHueSettings.settings.BridgeInfo.ContainsKey(br.Mac))
                    WinHueSettings.settings.BridgeInfo[br.Mac] = new BridgeSaveSettings
                    {
                        ip = br.IpAddress.ToString(),
                        apikey = br.ApiKey,
                        apiversion = br.ApiVersion,
                        swversion = br.SwVersion,
                        name = br.name
                    };
                else
                    WinHueSettings.settings.BridgeInfo.Add(br.Mac,
                        new BridgeSaveSettings { ip = br.IpAddress.ToString(), apikey = br.ApiKey, apiversion = br.ApiVersion, swversion = br.SwVersion, name = br.name });

                if (br.IsDefault) WinHueSettings.settings.DefaultBridge = br.Mac;
            }

            return WinHueSettings.Save();
        }
        #endregion

        #region HOME_TAB_METHODS

        private void CreateAdvanced()
        {
            Form_AdvancedCreator fac = new Form_AdvancedCreator(SelectedBridge);
            fac.Owner = Application.Current.MainWindow;
            fac.OnObjectCreated += Fac_OnObjectCreated;
            fac.Show();
        }

        private async void Fac_OnObjectCreated(object sender, EventArgs e)
        {
            await RefreshView();
        }

        private async Task RefreshView()
        {
            if (SelectedBridge == null) return;
            Cursor_Tools.ShowWaitCursor();
            SelectedObject = null;
            log.Info($"Getting list of objects from bridge at {SelectedBridge.IpAddress}.");
            List<IHueObject> hr = await HueObjectHelper.GetBridgeDataStoreAsyncTask(SelectedBridge);
            if (hr != null)
            {
                List<IHueObject> listobj = hr;
                ObservableCollection<IHueObject> newlist = new ObservableCollection<IHueObject>();

                switch (MainFormModel.Sort)
                {
                    case WinHueSortOrder.Default:
                        ListBridgeObjects = new ObservableCollection<IHueObject>(hr);
                        break;
                    case WinHueSortOrder.Ascending:
                        newlist.AddRange(from item in listobj where item is Light orderby item.name select item);
                        newlist.AddRange(from item in listobj where item is Group orderby item.name select item);
                        newlist.AddRange(from item in listobj where item is Schedule orderby item.name select item);
                        newlist.AddRange(from item in listobj where item is Scene orderby item.name select item);
                        newlist.AddRange(from item in listobj where item is ISensor orderby item.name select item);
                        newlist.AddRange(from item in listobj where item is Rule orderby item.name select item);
                        newlist.AddRange(from item in listobj where item is Resourcelink orderby item.name select item);
                        ListBridgeObjects = new ObservableCollection<IHueObject>(newlist);
                        break;
                    case WinHueSortOrder.Descending:
                        newlist.AddRange(from item in listobj where item is Light orderby item.name descending select item);
                        newlist.AddRange(from item in listobj where item is Group orderby item.name descending select item);
                        newlist.AddRange(from item in listobj where item is Schedule orderby item.name descending select item);
                        newlist.AddRange(from item in listobj where item is Scene orderby item.name descending select item);
                        newlist.AddRange(from item in listobj where item is ISensor orderby item.name descending select item);
                        newlist.AddRange(from item in listobj where item is Rule orderby item.name descending select item);
                        newlist.AddRange(from item in listobj where item is Resourcelink orderby item.name descending select item);
                        ListBridgeObjects = new ObservableCollection<IHueObject>(newlist);
                        break;
                    default:
                        goto case WinHueSortOrder.Default;
                }

                log.Info($"Found {ListBridgeObjects.Count} objects in the bridge.");
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBridgeObjects);
                view.GroupDescriptions?.Clear();
                PropertyGroupDescription groupDesc = new TypeGroupDescription();
                view.GroupDescriptions?.Add(groupDesc);
                log.Info($"Finished refreshing view.");
            }
            else
            {
                ListBridgeObjects = null;
                log.Error(hr);
            }
            Cursor_Tools.ShowNormalCursor();
        }

        private async Task DoubleClickObject()
        {
            log.Debug("Double click on : " + SelectedObject);
            if ((SelectedObject is Light) || (SelectedObject is Group))
            {
                ImageSource hr = await HueObjectHelper.ToggleObjectOnOffStateAsyncTask(SelectedBridge, SelectedObject, SliderTt);
                if (hr != null)
                {
                    ImageSource newimg = hr;
                    SelectedObject.Image = newimg;
                    int index = _listBridgeObjects.FindIndex(x => x.Id == SelectedObject.Id && x.GetType() == SelectedObject.GetType());
                    if (index == -1) return;
                    if (SelectedObject is Light)
                    {
                        ((Light)SelectedObject).state.on = !((Light)SelectedObject).state.on;
                        ((Light)ListBridgeObjects[index]).state.on = !((Light)ListBridgeObjects[index]).state.on;
                    }
                    else
                    {
                        ((Group)_selectedObject).action.on = !((Group)_selectedObject).action.on;
                        ((Group)ListBridgeObjects[index]).action.on = !((Group)ListBridgeObjects[index]).action.on;
                    }

                    ListBridgeObjects[index].Image = newimg;

                }
            }
            else
            {
                log.Info($"Activating scene : {_selectedObject.Id}");
                await SelectedBridge.ActivateSceneAsyncTask(_selectedObject.Id);

            }

        }

        private void ResetTransitionTime()
        {
            SliderTt = WinHueSettings.settings.DefaultTT;
        }

        private async Task RefreshObject(IHueObject obj, bool logging = false)
        {
            int index = ListBridgeObjects.FindIndex(x => x.Id == obj.Id && x.GetType() == obj.GetType());
            if (index == -1) return;

            IHueObject hr = await HueObjectHelper.GetObjectAsyncTask(SelectedBridge, obj.Id, obj.GetType());

            if (hr == null) return;
            IHueObject newobj = hr;
            ListBridgeObjects[index].Image = newobj.Image;
            List<PropertyInfo> pi = newobj.GetType().GetListHueProperties();

            foreach (PropertyInfo p in pi)
            {

                if (ListBridgeObjects[index].HasProperty(p.Name))
                {
                    PropertyInfo prop = ListBridgeObjects[index].GetType().GetProperty(p.Name);
                    if (prop != null) p.SetValue(ListBridgeObjects[index], prop.GetValue(newobj));
                }
            }
        }
        private void ShowEventLog()
        {
            if (_eventlogform.IsVisible) return;
            log.Debug("Opening event log.");
            _eventlogform.Show();

        }

        private async Task AllOn()
        {
            log.Info("Sending all on command to bridge" + SelectedBridge.IpAddress);
            Action act = new Action { on = true };
            if (WinHueSettings.settings.AllOnTT != null) act.transitiontime = WinHueSettings.settings.AllOnTT;
            bool bresult = await SelectedBridge.SetStateAsyncTask(act, "0");
            if (!bresult) return;
            log.Debug("Refreshing the main view.");
            await RefreshView();
        }

        private async Task AllOff()
        {
            log.Info("Sending all off command to bridge" + SelectedBridge.IpAddress);
            Action act = new Action { on = false };
            if (WinHueSettings.settings.AllOnTT != null) act.transitiontime = WinHueSettings.settings.AllOnTT;
            bool bresult = await SelectedBridge.SetStateAsyncTask(act, "0");
            if (!bresult) return;
            log.Debug("Refreshing the main view.");
            await RefreshView();
        }

        private async Task CheckForNewBulb()
        {
            bool success = await SelectedBridge.StartNewObjectsSearchAsyncTask(typeof(Light));
            if (success)
            {
                log.Info("Seaching for new lights...");
                _findlighttimer.Start();
                RaisePropertyChanged("SearchingLights");
            }
            else
            {
                MessageBox.Show(GlobalStrings.Error_Getting_NewLights, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error("There was an error while looking for new lights.");
            }
        }

        private async Task CreateGroup()
        {
            Form_GroupCreator fgc = new Form_GroupCreator() { Owner = Application.Current.MainWindow };
            await fgc.Initialize(SelectedBridge);
            log.Debug($@"Opening the Group creator window for bridge {SelectedBridge.IpAddress} ");
            if (fgc.ShowDialog() != true) return;
            if (fgc.GetCreatedOrModifiedID() == null) return;

            Group hr = HueObjectHelper.GetObject<Group>(SelectedBridge, fgc.GetCreatedOrModifiedID());
            if (hr != null)
            {              
                _listBridgeObjects.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private  async Task CreateScene()
        {
            Form_SceneCreator fsc = new Form_SceneCreator() { Owner = Application.Current.MainWindow };
            await fsc.Inititalize(SelectedBridge);
            log.Debug($@"Opening the scene creator window for bridge {SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created scene ID {fsc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            Scene hr = (Scene) await HueObjectHelper.GetObjectAsyncTask(SelectedBridge, fsc.GetCreatedOrModifiedID(), typeof(Scene));
            if (hr != null)
            {
                _listBridgeObjects.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private async Task CreateSchedule()
        {
            Form_ScheduleCreator fscc = new Form_ScheduleCreator() { Owner = Application.Current.MainWindow };
            await fscc.Initialize(SelectedBridge, _selectedObject );
            log.Debug($@"Opening the schedule creator window passing bridge {SelectedBridge.IpAddress} ");
            if (fscc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created schedule ID {fscc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            Schedule hr = (Schedule) await HueObjectHelper.GetObjectAsyncTask(SelectedBridge, fscc.GetCreatedOrModifiedID(), typeof(Schedule));
            if (hr != null)
            {
                _listBridgeObjects.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }

        }

        private async Task CreateRule()
        {

            Form_RuleCreator frc = new Form_RuleCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            if (frc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly sensor schedule ID {frc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            Rule rule = (Rule) await HueObjectHelper.GetObjectAsyncTask(SelectedBridge, frc.GetCreatedOrModifiedID(), typeof(Rule));
            if (rule != null)
            {
                _listBridgeObjects.Add(rule);
            }
            else
            {
                SelectedBridge.ShowErrorMessages();
            }

        }

        private void CreateSensor()
        {
            Form_SensorCreator fsc = new Form_SensorCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the sensor creator window passing bridge {SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created sensor ID {fsc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            ISensor hr = HueObjectHelper.GetObject<ISensor>(SelectedBridge, fsc.GetCreatedOrModifiedID());
            if (hr != null)
            {

                ListBridgeObjects.Add(hr);
            }
            else
            {
                SelectedBridge.ShowErrorMessages();
            }
        }

        private async Task SearchNewSensors()
        {
            bool bresult = await SelectedBridge.StartNewObjectsSearchAsyncTask(typeof(ISensor));
            if (bresult)
            {
                log.Info("Looking for new sensors for 1 minute.");
                _findsensortimer.Start();
                RaisePropertyChanged("SearchingLights");
            }
            else
            {
                log.Error("Unable to look for new sensors. Please check the log for more details.");
            }
        }

        private async Task CreateHotKey()
        {
            UnloadHotkeys();
            Form_HotKeyCreator fhkc = new Form_HotKeyCreator() { Owner = Application.Current.MainWindow };
            await fhkc.Initialize(SelectedBridge);
            fhkc.ShowDialog();
            _listHotKeys = fhkc.GetHotKeys();
            LoadHotkeys();
        }


        private async Task CreateResourceLink()
        {
            Form_ResourceLinksCreator frc = new Form_ResourceLinksCreator() { Owner = Application.Current.MainWindow };
            await frc.Initialize(SelectedBridge);
            log.Debug($@"Opening the sensor ResourceLink window passing bridge {SelectedBridge.IpAddress} ");
            if (!(bool)frc.ShowDialog()) return;
            log.Debug($@"Getting the newly created ResourceLink ID {frc.GetCreatedModifiedId()} from bridge {SelectedBridge.IpAddress}");
            Resourcelink hr = HueObjectHelper.GetObject<Resourcelink>(SelectedBridge, frc.GetCreatedModifiedId());
            if(hr != null)
            {
                ListBridgeObjects.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private async Task DoBridgeUpdate()
        {
            if (MessageBox.Show(GlobalStrings.Update_Confirmation, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            log.Info("Updating bridge to the latest firmware.");
            bool bresult = await SelectedBridge.UpdateBridgeAsyncTask();
            if (!bresult)
            {
                log.Error("An error occured while trying to start a bridge update. Please try again later.");
                return;
            }
            Form_Wait fw = new Form_Wait();
            fw.ShowWait(GlobalStrings.ApplyingUpdate, new TimeSpan(0, 0, 0, 180), Application.Current.MainWindow);
            BridgeSettings bsettings = await SelectedBridge.GetBridgeSettingsAsyncTask();
            if (bsettings != null)
            {

                switch (bsettings.swupdate.updatestate)
                {
                    case 0:
                        log.Info("Bridge updated succesfully");
                        SelectedBridge.SetNotify();
                        break;
                    case 1:
                        log.Info("Bridge update incoming. Please wait for it to be ready.");
                        break;
                    case 2:
                        log.Info("Bridge update still available or a new update is available.");
                        break;
                    case null:
                        log.Error("Bride update state was null.");
                        break;
                    default:
                        log.Error("An error occured while updating the bridge. Please try again later.");
                        break;

                }

            }

            RaisePropertyChanged("UpdateAvailable");
        }

        public void HandleHotkey(HotKeyHandle e)
        {

            ModifierKeys m = e.KeyModifiers;
            Key k = e.Key;
            try
            {

                HotKey h = _listHotKeys.First(x => x.Modifier == m && x.Key == k);
                if (!(h.objecType == null && SelectedObject == null))
                {
                    HotkeyDetected = true;
                    _ledTimer.Start();
                    Type objtype = h?.objecType == null ? SelectedObject.GetType() : h.objecType;

                    if (objtype == typeof(Scene) )
                    {
                        SelectedBridge.ActivateScene(h.id);
                    }
                    else if (objtype == typeof(Group) || objtype == typeof(Light))
                    {
                        SelectedBridge.SetState(h.properties,h.id);
                    }
                    else
                    {
                        log.Warn($"Type of object {objtype} not supported");
                    }
                }
                else
                {
                    log.Warn("You must select an object in WinHue in order to apply this hotkey.");
                }
            }
            catch (InvalidOperationException)
            {
                log.Warn("No Hotkey was found.");
                _ledTimer.Start();
                
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void LoadHotkeys()
        {

            if (_lhk.Count > 0)
            {
                while (_lhk.Count != 0)
                {
                    _lhk[0].Unregister();
                    _lhk.Remove(_lhk[0]);
                }
            }

            foreach (HotKey h in _listHotKeys)
            {
                HotKeyHandle hkh = new HotKeyHandle(h, HandleHotkey);
                if (hkh.Register())
                    _lhk.Add(hkh);
                else
                    log.Error($"Cannot register hotkey {h.Name} key seems to be already taken by another process.");
            }

        }

        private void UnloadHotkeys()
        {
            if (_lhk.Count > 0)
            {
                while (_lhk.Count != 0)
                {
                    _lhk[0].Unregister();
                    _lhk.Remove(_lhk[0]);
                }
            }

        }

        #endregion

        #region SLIDERS_METHODS
        private async Task SliderChangeHue()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.hue = MainFormModel.SliderHue;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if(!result)
            {
                MainFormModel.SliderHue = MainFormModel.OldSliderHue;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                
                if(SelectedObject is Light)
                    ((Light)SelectedObject).state.hue = MainFormModel.SliderHue;

                if(SelectedObject is Group)
                    ((Group)SelectedObject).action.hue = MainFormModel.SliderHue;

            }
        }

        private async Task SliderChangeBri()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.bri = MainFormModel.SliderBri;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);
                
            if(!result)
            {
                MainFormModel.SliderBri = MainFormModel.OldSliderBri;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                if (SelectedObject is Light)
                    ((Light)SelectedObject).state.bri = MainFormModel.SliderBri;

                if (SelectedObject is Group)
                    ((Group)SelectedObject).action.bri = MainFormModel.SliderBri;
            }
        }

        private async Task SliderChangeCt()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.ct = MainFormModel.SliderCt;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);
            
            if(!result)
            {
                MainFormModel.SliderCt = MainFormModel.OldSliderCt;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                if (SelectedObject is Light)
                    ((Light)SelectedObject).state.ct = MainFormModel.SliderCt;

                if (SelectedObject is Group)
                    ((Group)SelectedObject).action.ct = MainFormModel.SliderCt;
            }
        }

        private async Task SliderChangeSat()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.sat = MainFormModel.SliderSat;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if(!result)
            {
                MainFormModel.SliderSat = MainFormModel.OldSliderSat;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                if (SelectedObject is Light)
                    ((Light)SelectedObject).state.sat = MainFormModel.SliderSat;

                if (SelectedObject is Group)
                    ((Group)SelectedObject).action.sat = MainFormModel.SliderSat;
            }
        }

        private async Task SliderChangeXy()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.xy = new decimal[2]
            {
                MainFormModel.SliderX,
                MainFormModel.SliderY
            };
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);            

            if (!result)
            {
                MainFormModel.SliderX = MainFormModel.OldSliderX;
                MainFormModel.SliderY = MainFormModel.OldSliderY;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                if (SelectedObject is Light)
                {
                    ((Light)SelectedObject).state.xy[0] = MainFormModel.SliderX;
                    ((Light)SelectedObject).state.xy[1] = MainFormModel.SliderY;
                }

                if (SelectedObject is Group)
                {
                    ((Group)SelectedObject).action.xy[0] = MainFormModel.SliderX;
                    ((Group)SelectedObject).action.xy[1] = MainFormModel.SliderY;
                }
            }
        }

        private void SetMainFormModel()
        {
            
            if (_selectedObject is Light)
            {
                Light light = (Light) _selectedObject;
                
                MainFormModel.SliderBri = light.state.bri ?? 0;
                MainFormModel.SliderHue = light.state.hue ?? 0;
                MainFormModel.SliderSat = light.state.sat ?? 0;
                MainFormModel.SliderCt = light.state.ct ?? 153;
                MainFormModel.SliderX = light.state.xy?[0] ?? 0;
                MainFormModel.SliderY = light.state.xy?[1] ?? 0;
             
            }
            else if (_selectedObject is Group)
            {
                Group light = (Group)_selectedObject;
                
                MainFormModel.SliderBri = light.action.bri ?? 0;
                MainFormModel.SliderHue = light.action.hue ?? 0;
                MainFormModel.SliderSat = light.action.sat ?? 0;
                MainFormModel.SliderCt = light.action.ct ?? 153;
                MainFormModel.SliderX = light.action.xy?[0] ?? 0;
                MainFormModel.SliderY = light.action.xy?[1] ?? 0;
               
            }
            else
            {            
                MainFormModel.SliderBri = 0;
                MainFormModel.SliderHue = 0;
                MainFormModel.SliderSat = 0;
                MainFormModel.SliderCt = 153;
                MainFormModel.SliderX = 0;
                MainFormModel.SliderY = 0;           
            }
        }

        #endregion

        #region TOOLS_VIEW_METHODS
        private async Task ViewSceneMapping()
        {
            Form_SceneMapping fsm = new Form_SceneMapping() { Owner = Application.Current.MainWindow };
            await fsm.Initialize(SelectedBridge);
            fsm.Show();
        }

        private async Task ViewBulbs()
        {
            Form_BulbsView fbv = new Form_BulbsView() { Owner = Application.Current.MainWindow };
            await fbv.Initialize(SelectedBridge);
            fbv.Show();
        }

        private async Task ViewGroups()
        {
            Form_GroupView fgv = new Form_GroupView() { Owner = Application.Current.MainWindow };
            await fgv.Initialize(SelectedBridge);
            fgv.Show();
        }
        #endregion

        #region CONTEXT_MENU_METHODS
        private async Task ReplaceCurrentState()
        {
            if (MessageBox.Show(GlobalStrings.Scene_Replace_Current_States, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            log.Info($@"Replacing scene {((Scene)_selectedObject).name} lights state with current one.");
            await SelectedBridge.StoreCurrentLightStateAsyncTask(_selectedObject.Id);

        }

        private async Task EditObject()
        {
            log.Debug("Editing object : " + _selectedObject);

            if (_selectedObject is Group)
            {
                Form_GroupCreator fgc = new Form_GroupCreator() { Owner = Application.Current.MainWindow };
                await fgc.Initialize(SelectedBridge, (Group)SelectedObject);
                if (fgc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Schedule)
            {
                Form_ScheduleCreator fsc = new Form_ScheduleCreator() { Owner = Application.Current.MainWindow };
                await fsc.Initialize(SelectedBridge, SelectedObject);
                if (fsc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Sensor)
            {
                Sensor obj = (Sensor)_selectedObject;
                switch (obj.modelid)
                {
                    case "PHDL00":
                        Sensor cr = (Sensor) await HueObjectHelper.GetObjectAsyncTask(SelectedBridge, obj.Id, typeof(Sensor));
                        if (cr != null)
                        { 
                            cr.Id = obj.Id;
                            Form_Daylight dl = new Form_Daylight(cr, SelectedBridge) { Owner = Application.Current.MainWindow };
                            if (dl.ShowDialog() == true)
                            {
                                await RefreshObject(_selectedObject);
                            }

                        }

                        break;
                    case "ZGPSWITCH":
                        Form_HueTapConfig htc = new Form_HueTapConfig()
                        {
                            Owner = Application.Current.MainWindow
                        };
                        await htc.Initialize(obj.Id, SelectedBridge);
                        if (htc.ShowDialog() == true)
                        {
                            await RefreshObject(_selectedObject);
                        }
                        break;
                    default:
                        Sensor crs = (Sensor) await HueObjectHelper.GetObjectAsyncTask(SelectedBridge,obj.Id, typeof(Sensor));
                        if (crs != null)
                        {
                            Form_SensorCreator fsc = new Form_SensorCreator(SelectedBridge, crs)
                            {
                                Owner = Application.Current.MainWindow
                            };
                            if (fsc.ShowDialog() == true)
                            {
                                await RefreshObject(_selectedObject);
                            }

                        }
                        break;
                }
            }
            else if (_selectedObject is Rule)
            {
                Form_RuleCreator frc = new Form_RuleCreator(SelectedBridge, (Rule)_selectedObject) { Owner = Application.Current.MainWindow };
                if (frc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Scene)
            {
                Form_SceneCreator fscc = new Form_SceneCreator() { Owner = Application.Current.MainWindow };
                await fscc.Inititalize(SelectedBridge, _selectedObject.Id);
                if (fscc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Resourcelink)
            {
                Form_ResourceLinksCreator frlc = new Form_ResourceLinksCreator() { Owner = Application.Current.MainWindow };
                await frlc.Initialize(SelectedBridge, (Resourcelink) _selectedObject);
                if (frlc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }

            }
        }

        private async Task Identify(string type)
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.alert = type;

            await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);
        }

        private async Task Clone(bool quick)
        {
            log.Info($"Cloning {SelectedObject}...");

            if (quick)
            {
                log.Info($"Quick cloning beginning...");
                await Duplicate();              
            }
            else
            {
                log.Info($"Cloning beginning...");
                bool result = await Duplicate();
                if (!result) return;
                try
                {
                    IHueObject oldobj = (IHueObject)SelectedObject.Clone();
                    SelectedObject = ListBridgeObjects.Single(x => x.GetType() == oldobj.GetType() && x.Id == oldobj.Id);
                    await EditObject();
                }
                catch(Exception)
                {
                    MessageBox.Show(GlobalStrings.Error_While_Cloning, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task<bool> Duplicate()
        {
            
            bool result = await SelectedBridge.CreateObjectAsyncTask(SelectedObject);
            
            if (result)
            {
                log.Info("Object cloned succesfully !");
                IHueObject hr = await SelectedBridge.GetObjectAsyncTask(SelectedObject.Id, SelectedObject.GetType());

                if (hr != null)
                {
                    ListBridgeObjects.Add(hr);
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(SelectedBridge);
                }
            }
            else
            {
                log.Error("Error while cloning object.");
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            return result;
        }

        private async Task Sensitivity(int sensitivity)
        {
            await SelectedBridge.ChangeSensorConfigAsyncTask(SelectedObject.Id,new HueMotionSensorConfig(){sensitivity = sensitivity});
        }

        private async Task DeleteObject()
        {
            if (
                MessageBox.Show(string.Format(GlobalStrings.Confirm_Delete_Object, SelectedObject.name),
                    GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            bool result = await SelectedBridge.RemoveObjectAsyncTask(SelectedObject);

            log.Debug($"Object ID {SelectedObject.Id} removal " + result);
            if (result)
            {
                int index =
                    ListBridgeObjects.FindIndex(
                        x => x.Id == SelectedObject.Id && x.GetType() == SelectedObject.GetType());
                if (index == -1) return;
                ListBridgeObjects.RemoveAt(index);
                SelectedObject = null;
                log.Info($"Object ID : {index} removed.");
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private void RenameObject()
        {

            
            Form_RenameObject fro = new Form_RenameObject(SelectedBridge, SelectedObject) { Owner = Application.Current.MainWindow };
            if (fro.ShowDialog() != true) return;
            SelectedObject.name = fro.GetNewName();
            int index = ListBridgeObjects.FindIndex(x => x.Id == SelectedObject.Id && x.GetType() == SelectedObject.GetType());
            if (index == -1) return;
            ListBridgeObjects[index].name = fro.GetNewName();
            log.Info($"Renamed object ID : {index} renamed.");
            
        }

        private void CopyToJson(bool raw)
        {
            Lastmessage = "Object copied to clipboard.";
            Clipboard.SetText(JsonConvert.SerializeObject(SelectedObject, raw ? Formatting.None : Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        #endregion

        #region VIEW_TAB_METHODS
        private async Task SortListView()
        {
            await RefreshView();
            WinHueSettings.settings.Sort = MainFormModel.Sort;
        }

        #endregion

        #region HELP_TAB_METHODS
        private void OpenWinHueWebsite()
        {
            Process.Start("https://hyrules.github.io/WinHue3/");
        }

        private void OpenWinHueSupport()
        {
            Process.Start("https://github.com/Hyrules/WinHue3/issues");
        }

        private void OpenAboutWindow()
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        #endregion

        #region APPLICATION_MENU_METHODS
        private async Task OpenSettingsWindow()
        {
            Form_AppSettings settings = new Form_AppSettings { Owner = Application.Current.MainWindow };
            if (settings.ShowDialog() != true) return;
            Comm.Timeout = WinHueSettings.settings.Timeout;
            if (MainFormModel.ShowId != WinHueSettings.settings.ShowID)
            {
                MainFormModel.ShowId = WinHueSettings.settings.ShowID;
            }

            if (MainFormModel.WrapText != WinHueSettings.settings.WrapText)
            {
                MainFormModel.WrapText = WinHueSettings.settings.WrapText;
                await RefreshView();
            }

            if (SliderTt != WinHueSettings.settings.DefaultTT)
            {
                SliderTt = WinHueSettings.settings.DefaultTT;
            }


        }

        private void QuitApplication()
        {
            Application.Current.Shutdown();
        }


        private bool DoBridgePairing(ObservableCollection<Bridge> listBridges = null)
        {
            Form_BridgeDetectionPairing dp = new Form_BridgeDetectionPairing(listBridges) { Owner = Application.Current.MainWindow };

            bool result = (bool)dp.ShowDialog();
            if (!result) return result;
            ListBridges = dp.ViewModel.ListBridges;
            SaveSettings();
            return result;
        }
        #endregion

        private void CpuTempMonSettings()
        {
            _ctm.ShowSettingsForm();
        }

        private void RssFeedMon()
        {
          
        }

        private void RssFeedMonSettings()
        {

        }

        private async Task Colorloop()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.effect = "colorloop";
            await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

        }

        private async Task NoEffect()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.effect = "none";
            await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

        }

        private async Task ClickObject()
        {
            if (SelectedObject != null)
            {
                IHueObject hr = await HueObjectHelper.GetObjectAsyncTask(SelectedBridge, SelectedObject.Id, SelectedObject.GetType());
                if (hr == null) return;
                _selectedObject = hr;
            }
            SetMainFormModel();
        }

        private void DoAppUpdate()
        {
            if(MessageBox.Show(GlobalStrings.UpdateAvailableDownload, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                UpdateManager.DownloadUpdate();
        }
    }
}
