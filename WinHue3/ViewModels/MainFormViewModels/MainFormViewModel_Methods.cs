using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using HueLib2;
using Newtonsoft.Json;
using WinHue3.Models;
using Action = HueLib2.Action;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using System.Diagnostics;
using WinHue3.Settings;

namespace WinHue3.ViewModels
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        private T ExecuteGenericMethod<T>(object objectmethod, string methodname, object[] paramsarray) where T : new()
        {
            MethodInfo mi = objectmethod.GetType().GetMethod(methodname);
            MethodInfo generic = mi.MakeGenericMethod(_selectedObject.GetType());
            object result = generic.Invoke(SelectedBridge, paramsarray);
            return (T)result;
        }

        #region BRIDGE_SETTINGS
        private void ChangeBridgeSettings()
        {
            Form_BridgeSettings frs = new Form_BridgeSettings(SelectedBridge) { Owner = Application.Current.MainWindow };
            if (frs.ShowDialog() == true)
            {
                CommandResult bresult = SelectedBridge.GetBridgeSettings();
                if (bresult.Success)
                {
                    string newname = ((BridgeSettings)(bresult.resultobject)).name;
                    if (SelectedBridge.Name != newname)
                    {
                        SelectedBridge.Name = newname;
                        ListBridges[ListBridges.IndexOf(SelectedBridge)].Name = newname;
                        Bridge selbr = SelectedBridge;
                        SelectedBridge = null;
                        SelectedBridge = selbr;
                    }
                    RefreshView();
                    OnPropertyChanged("UpdateAvailable");
                }
            }
        }

        private void ManageUsers()
        {
            Form_ManageUsers fmu = new Form_ManageUsers(SelectedBridge) { Owner = Application.Current.MainWindow };
            fmu.ShowDialog();
        }

        public bool SaveSettings()
        {
            foreach (Bridge br in ListBridges)
            {
                if (WinHueSettings.settings.BridgeInfo.ContainsKey(br.Mac))
                    WinHueSettings.settings.BridgeInfo[br.Mac] = new BridgeSaveSettings()
                    {
                        ip = br.IpAddress.ToString(),
                        apikey = br.ApiKey,
                        apiversion = br.ApiVersion,
                        swversion = br.SwVersion,
                        name = br.Name
                    };
                else
                    WinHueSettings.settings.BridgeInfo.Add(br.Mac,
                        new BridgeSaveSettings() { ip = br.IpAddress.ToString(), apikey = br.ApiKey, apiversion = br.ApiVersion, swversion = br.SwVersion, name = br.Name });

                if (br.IsDefault) WinHueSettings.settings.DefaultBridge = br.Mac;
            }

            return WinHueSettings.Save();
        }
        #endregion

        #region HOME_TAB_METHODS
        private void RefreshView()
        {
            if (SelectedBridge == null) return;
            Cursor_Tools.ShowWaitCursor();
            SelectedObject = null;
            log.Info($"Getting list of objects from bridge at {SelectedBridge.IpAddress}.");
            HelperResult hr = HueObjectHelper.GetBridgeDataStore(SelectedBridge);
            if (hr.Success)
            {
                List<HueObject> listobj = (List<HueObject>)hr.Hrobject;
                ObservableCollection<HueObject> newlist = new ObservableCollection<HueObject>();

                switch (MainFormModel.Sort)
                {
                    case MainFormModel.WinHueSortOrder.Default:
                        ListBridgeObjects = new ObservableCollection<HueObject>((List<HueObject>)hr.Hrobject);
                        break;
                    case MainFormModel.WinHueSortOrder.Ascending:
                        newlist.AddRange(from item in listobj where item is Light orderby item.GetName() ascending select item);
                        newlist.AddRange(from item in listobj where item is Group orderby item.GetName() ascending select item);
                        newlist.AddRange(from item in listobj where item is Schedule orderby item.GetName() ascending select item);
                        newlist.AddRange(from item in listobj where item is Scene orderby item.GetName() ascending select item);
                        newlist.AddRange(from item in listobj where item is Sensor orderby item.GetName() ascending select item);
                        newlist.AddRange(from item in listobj where item is Rule orderby item.GetName() ascending select item);
                        newlist.AddRange(from item in listobj where item is Resourcelink orderby item.GetName() ascending select item);
                        ListBridgeObjects = new ObservableCollection<HueObject>(newlist);
                        break;
                    case MainFormModel.WinHueSortOrder.Descending:
                        newlist.AddRange(from item in listobj where item is Light orderby item.GetName() descending select item);
                        newlist.AddRange(from item in listobj where item is Group orderby item.GetName() descending select item);
                        newlist.AddRange(from item in listobj where item is Schedule orderby item.GetName() descending select item);
                        newlist.AddRange(from item in listobj where item is Scene orderby item.GetName() descending select item);
                        newlist.AddRange(from item in listobj where item is Sensor orderby item.GetName() descending select item);
                        newlist.AddRange(from item in listobj where item is Rule orderby item.GetName() descending select item);
                        newlist.AddRange(from item in listobj where item is Resourcelink orderby item.GetName() descending select item);
                        ListBridgeObjects = new ObservableCollection<HueObject>(newlist);
                        break;
                    default:
                        goto case MainFormModel.WinHueSortOrder.Default;
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
                log.Error(hr.Hrobject);
            }
            Cursor_Tools.ShowNormalCursor();
        }

        private void DoubleClickObject()
        {
            log.Debug("Double click on : " + SelectedObject);
            if ((SelectedObject is Light) || (SelectedObject is Group))
            {
                HelperResult hr = HueObjectHelper.ToggleObjectOnOffState(SelectedBridge, SelectedObject);
                if (hr.Success)
                {
                    ImageSource newimg = (ImageSource)hr.Hrobject;
                    if (newimg == null) return;
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
                SelectedBridge.ActivateScene(_selectedObject.Id);

            }

        }

        private void RefreshObject(HueObject obj, bool logging = false)
        {
            int index = ListBridgeObjects.FindIndex(x => x.Id == obj.Id && x.GetType() == obj.GetType());
            if (index == -1) return;

            MethodInfo mi = typeof(HueObjectHelper).GetMethod("GetObject");
            MethodInfo generic = mi.MakeGenericMethod(obj.GetType());
            HelperResult hr = (HelperResult)generic.Invoke(SelectedBridge, new object[] { SelectedBridge, obj.Id });

            if (!hr.Success) return;
            HueObject newobj = (HueObject)hr.Hrobject;
            ListBridgeObjects[index].Image = newobj.Image;
            PropertyInfo[] pi = newobj.GetType().GetProperties();
            foreach (PropertyInfo p in pi)
            {
                if (ListBridgeObjects[index].HasProperty(p.Name))
                    p.SetValue(ListBridgeObjects[index], ListBridgeObjects[index].GetType().GetProperty(p.Name).GetValue(newobj));
            }
        }
        private void ShowEventLog()
        {
            if (_eventlogform.IsVisible) return;
            log.Debug("Opening event log.");
            _eventlogform.Show();

        }

        private void AllOn()
        {
            log.Info("Sending all on command to bridge" + SelectedBridge.IpAddress);
            HueLib2.Action act = new HueLib2.Action() { on = true };
            if (WinHueSettings.settings.AllOnTT != null) act.transitiontime = WinHueSettings.settings.AllOnTT;
            CommandResult bresult = SelectedBridge.SetState<Group>(act, "0");
            if (!bresult.Success) return;
            log.Debug("Refreshing the main view.");
            RefreshView();
        }

        private void AllOff()
        {
            log.Info("Sending all off command to bridge" + SelectedBridge.IpAddress);
            HueLib2.Action act = new Action() { on = false };
            if (WinHueSettings.settings.AllOnTT != null) act.transitiontime = WinHueSettings.settings.AllOnTT;
            CommandResult bresult = SelectedBridge.SetState<Group>(act, "0");
            if (!bresult.Success) return;
            log.Debug("Refreshing the main view.");
            RefreshView();
        }

        private void CheckForNewBulb()
        {
            CommandResult bresult = SelectedBridge.FindNewObjects<Light>();
            if (bresult.Success)
            {
                log.Info("Seaching for new lights...");
                _findlighttimer.Start();
            }
            else
            {
                MessageBox.Show(GlobalStrings.Error_Getting_NewLights, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error("There was an error while looking for new lights.");
            }
        }

        private void CreateGroup()
        {
            Form_GroupCreator fgc = new Form_GroupCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the Group creator window for bridge {SelectedBridge.IpAddress} ");
            if (fgc.ShowDialog() != true) return;
            if (fgc.GetCreatedOrModifiedID() == null) return;

            HelperResult hr = HueObjectHelper.GetObject<Group>(SelectedBridge, fgc.GetCreatedOrModifiedID());
            if (hr.Success)
            {              
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private void CreateScene()
        {
            Form_SceneCreator fsc = new Form_SceneCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the scene creator window for bridge {SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created scene ID {fsc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Scene>(SelectedBridge, fsc.GetCreatedOrModifiedID());
            if (hr.Success)
            {
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private void CreateSchedule()
        {
            Form_ScheduleCreator fscc = new Form_ScheduleCreator(_selectedObject,SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the schedule creator window passing bridge {SelectedBridge.IpAddress} ");
            if (fscc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created schedule ID {fscc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Schedule>(SelectedBridge, fscc.GetCreatedOrModifiedID());
            if (hr.Success)
            {
                _listBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }

        }

        private void CreateRule()
        {
            Form_RulesCreator2 frc = new Form_RulesCreator2(SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the rule creator window passing bridge {SelectedBridge.IpAddress} ");
            if (frc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created rule ID {frc.GetCreatedOrModifiedId()} from bridge {SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Rule>(SelectedBridge, frc.GetCreatedOrModifiedId());
            if (hr.Success)
            {

                ListBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }

        }

        private void CreateSensor()
        {
            Form_SensorCreator fsc = new Form_SensorCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the sensor creator window passing bridge {SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created sensor ID {fsc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Sensor>(SelectedBridge, fsc.GetCreatedOrModifiedID());
            if (hr.Success)
            {

                ListBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private void SearchNewSensors()
        {
            CommandResult bresult = SelectedBridge.FindNewObjects<Sensor>();
            if (bresult.Success)
            {
                log.Info("Looking for new sensors for 1 minute.");
                _findsensortimer.Start();
            }
            else
            {
                log.Error("Unable to look for new sensors. Please check the log for more details.");
            }
        }

        public void CreateHotKey()
        {
            Form_HotKeyCreator fhkc = new Form_HotKeyCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            fhkc.ShowDialog();
            _listHotKeys = fhkc.GetHotKeys();
            LoadHotkeys();
        }


        private void CreateResourceLink()
        {
            Form_ResourceLinksCreator frc = new Form_ResourceLinksCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            log.Debug($@"Opening the sensor ResourceLink window passing bridge {SelectedBridge.IpAddress} ");
            if (!(bool)frc.ShowDialog()) return;
            log.Debug($@"Getting the newly created ResourceLink ID {frc.GetCreatedModifiedId()} from bridge {SelectedBridge.IpAddress}");
            HelperResult hr = HueObjectHelper.GetObject<Resourcelink>(SelectedBridge, frc.GetCreatedModifiedId());
            if(hr.Success)
            {
                ListBridgeObjects.Add((HueObject)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private void DoBridgeUpdate()
        {
            if (MessageBox.Show(GlobalStrings.Update_Confirmation, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            log.Info("Updating bridge to the latest firmware.");
            CommandResult bresult = SelectedBridge.DoSwUpdate();
            if (!bresult.Success)
            {
                log.Error("An error occured while trying to start a bridge update. Please try again later.");
                return;
            }
            else
            {
                Form_Wait fw = new Form_Wait();
                fw.ShowWait(GlobalStrings.ApplyingUpdate, new TimeSpan(0, 0, 0, 180), Application.Current.MainWindow);
                CommandResult bsettings = SelectedBridge.GetBridgeSettings();
                if (bsettings.Success)
                {
                    BridgeSettings brs = (BridgeSettings)bsettings.resultobject;

                    switch (brs.swupdate.updatestate)
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
            }

            OnPropertyChanged("UpdateAvailable");
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
                    Type objtype = h.objecType == null ? SelectedObject.GetType() : h.objecType;

                    if (objtype == typeof(Scene))
                    {
                        SelectedBridge.ActivateScene(h.id);
                    }
                    else if (objtype == typeof(Group) || objtype == typeof(Light))
                    {

                        List<object> listparams = new List<object>();
                        listparams.Add(Convert.ChangeType(h.properties, typeof(CommonProperties)));
                        listparams.Add(h.objecType != null ? h.id : SelectedObject.Id);
                        ExecuteGenericMethod<CommandResult>(SelectedBridge, "SetState", listparams.ToArray());
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

        #endregion

        #region SLIDERS_METHODS
        private void SliderChangeHue()
        {
            CommandResult cr = ExecuteGenericMethod<CommandResult>(SelectedBridge, "SetState", new object[] { new CommonProperties() { hue = MainFormModel.SliderHue, transitiontime = SliderTt }, _selectedObject.Id });
            if(!cr.Success)
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

        private void SliderChangeBri()
        {
            CommandResult cr = ExecuteGenericMethod<CommandResult>(SelectedBridge, "SetState", new object[] { new CommonProperties() { bri = MainFormModel.SliderBri, transitiontime = SliderTt }, _selectedObject.Id });
            if(!cr.Success)
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

        private void SliderChangeCt()
        {
            CommandResult cr = ExecuteGenericMethod<CommandResult>(SelectedBridge, "SetState", new object[] { new CommonProperties() { ct = MainFormModel.SliderCt, transitiontime = SliderTt }, _selectedObject.Id });
            if(!cr.Success)
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

        private void SliderChangeSat()
        {
            CommandResult cr = ExecuteGenericMethod<CommandResult>(SelectedBridge, "SetState", new object[] { new CommonProperties() { sat = MainFormModel.SliderSat, transitiontime = SliderTt }, _selectedObject.Id });
            if(!cr.Success)
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

        private void SliderChangeXy()
        {
            CommandResult cr = ExecuteGenericMethod<CommandResult>(SelectedBridge, "SetState", new object[] { new CommonProperties() { xy = new XY() { x = MainFormModel.SliderX, y = MainFormModel.SliderY}, transitiontime = SliderTt }, _selectedObject.Id });
            if (!cr.Success)
            {
                MainFormModel.SliderX = MainFormModel.OldSliderX;
                MainFormModel.SliderY = MainFormModel.OldSliderY;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                if (SelectedObject is Light)
                {
                    ((Light)SelectedObject).state.xy.x = MainFormModel.SliderX;
                    ((Light)SelectedObject).state.xy.y = MainFormModel.SliderY;
                }

                if (SelectedObject is Group)
                {
                    ((Group)SelectedObject).action.xy.x = MainFormModel.SliderX;
                    ((Group)SelectedObject).action.xy.y = MainFormModel.SliderY;
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
                MainFormModel.SliderX = light.state.xy?.x ?? 0;
                MainFormModel.SliderY = light.state.xy?.y ?? 0;
             
            }
            else if (_selectedObject is Group)
            {
                Group light = (Group)_selectedObject;
                
                MainFormModel.SliderBri = light.action.bri ?? 0;
                MainFormModel.SliderHue = light.action.hue ?? 0;
                MainFormModel.SliderSat = light.action.sat ?? 0;
                MainFormModel.SliderCt = light.action.ct ?? 153;
                MainFormModel.SliderX = light.action.xy?.x ?? 0;
                MainFormModel.SliderY = light.action.xy?.y ?? 0;
               
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
        private void ViewSceneMapping()
        {
            Form_SceneMapping _fsm = new Form_SceneMapping(SelectedBridge) { Owner = Application.Current.MainWindow };
            _fsm.Show();
        }

        private void ViewBulbs()
        {
            Form_BulbsView _fbv = new Form_BulbsView(SelectedBridge) { Owner = Application.Current.MainWindow };
            _fbv.Show();
        }

        private void ViewGroups()
        {
            Form_GroupView _fgv = new Form_GroupView(SelectedBridge) { Owner = Application.Current.MainWindow };
            _fgv.Show();
        }
        #endregion

        #region CONTEXT_MENU_METHODS
        private void ReplaceCurrentState()
        {
            if (MessageBox.Show(GlobalStrings.Scene_Replace_Current_States, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            log.Info($@"Replacing scene {((Scene)_selectedObject).name} lights state with current one.");
            SelectedBridge.StoreCurrentLightState(_selectedObject.Id);

        }

        private void EditObject()
        {
            log.Debug("Editing object : " + _selectedObject);

            if (_selectedObject is Group)
            {
                Form_GroupCreator fgc = new Form_GroupCreator(SelectedObject, SelectedBridge) { Owner = Application.Current.MainWindow };
                if (fgc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Schedule)
            {
                Form_ScheduleCreator fsc = new Form_ScheduleCreator(SelectedObject, SelectedBridge) { Owner = Application.Current.MainWindow };
                if (fsc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Sensor)
            {
                Sensor obj = (Sensor)_selectedObject;
                switch (obj.modelid)
                {
                    case "PHDL00":
                        CommandResult cr = SelectedBridge.GetObject<Sensor>(obj.Id);
                        if (cr.Success)
                        {
                            Sensor daylight = (Sensor)cr.resultobject;
                            daylight.Id = obj.Id;
                            Form_Daylight dl = new Form_Daylight(daylight, SelectedBridge) { Owner = Application.Current.MainWindow };
                            if (dl.ShowDialog() == true)
                            {
                                RefreshObject(_selectedObject);
                            }

                        }

                        break;
                    case "ZGPSWITCH":
                        Form_HueTapConfig htc = new Form_HueTapConfig(obj.Id, SelectedBridge)
                        {
                            Owner = Application.Current.MainWindow
                        };
                        if (htc.ShowDialog() == true)
                        {
                            RefreshObject(_selectedObject);
                        }
                        break;
                    default:
                        CommandResult crs = SelectedBridge.GetObject<Sensor>(obj.Id);
                        if (crs.Success)
                        {
                            Form_SensorCreator fsc = new Form_SensorCreator(SelectedBridge, (Sensor)crs.resultobject)
                            {
                                Owner = Application.Current.MainWindow
                            };
                            if (fsc.ShowDialog() == true)
                            {
                                RefreshObject(_selectedObject);
                            }

                        }
                        break;
                }
            }
            else if (_selectedObject is Rule)
            {
                Form_RulesCreator2 frc = new Form_RulesCreator2(SelectedBridge, _selectedObject) { Owner = Application.Current.MainWindow };
                if (frc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Scene)
            {
                Form_SceneCreator fscc = new Form_SceneCreator(SelectedBridge, _selectedObject.Id) { Owner = Application.Current.MainWindow };
                if (fscc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Resourcelink)
            {
                Form_ResourceLinksCreator frlc = new Form_ResourceLinksCreator(SelectedBridge, (Resourcelink)_selectedObject) { Owner = Application.Current.MainWindow };

                if (frlc.ShowDialog() == true)
                {
                    RefreshObject(_selectedObject);
                }

            }
        }

        private void Identify(string type)
        {
            MethodInfo mi = typeof(Bridge).GetMethod("SetState");
            MethodInfo generic = mi.MakeGenericMethod(_selectedObject.GetType());
            log.Info($@"Sending the {type} Identify command to object ID : {_selectedObject.Id}");
            CommandResult hr = (CommandResult)generic.Invoke(SelectedBridge, new object[] { new CommonProperties() { alert = type }, _selectedObject.Id });
        }

        private void Clone(bool quick)
        {
            log.Info($"Cloning {SelectedObject}...");

            if (quick)
            {
                log.Info($"Quick cloning beginning...");
                Duplicate();              
            }
            else
            {
                log.Info($"Cloning beginning...");
                bool result = Duplicate();
                if (result)
                {                 
                    try
                    {
                        HueObject oldobj = (HueObject)SelectedObject.Clone();
                        SelectedObject = ListBridgeObjects.Single(x => x.GetType() == oldobj.GetType() && x.Id == oldobj.Id);
                        EditObject();
                    }
                    catch(Exception)
                    {
                        MessageBox.Show(GlobalStrings.Error_While_Cloning, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                }
            }
        }

        private bool Duplicate()
        {
            bool result = false;
            CommandResult cr = ExecuteGenericMethod<CommandResult>(SelectedBridge, "CreateObject", new[] { SelectedObject.Clone() });
            if (cr.Success)
            {
                log.Info("Object cloned succesfully !");
                MethodInfo mi = typeof(HueObjectHelper).GetMethod("GetObject");
                MethodInfo gm = mi.MakeGenericMethod(SelectedObject.GetType());
                HelperResult hr = (HelperResult)gm.Invoke(null, new object[] { SelectedBridge, cr.resultobject.ToString() });
                if (hr.Success)
                {
                    ListBridgeObjects.Add((HueObject)hr.Hrobject);
                    result = true;
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

        private void Sensitivity(int sensitivity)
        {
            Sensor sensor = (Sensor)SelectedObject;
            ((HueMotionSensorConfig)sensor.config).sensitivity = sensitivity;
            // sensor.Id = _selectedObject.Id;
            sensor.type = ((Sensor)SelectedObject).type;
            SelectedBridge.ModifyObject<Sensor>(sensor, sensor.Id);
        }

        private void DeleteObject()
        {
            if (
                MessageBox.Show(string.Format(GlobalStrings.Confirm_Delete_Object, SelectedObject.GetName()),
                    GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            MethodInfo method = typeof(Bridge).GetMethod("RemoveObject");
            MethodInfo generic = method.MakeGenericMethod(_selectedObject.GetType());
            CommandResult bresult = (CommandResult)generic.Invoke(SelectedBridge, new object[] { SelectedObject.Id });

            log.Debug("Result : " + bresult.resultobject);
            if (bresult.Success)
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
            SelectedObject.SetName(fro.GetNewName());
            int index = ListBridgeObjects.FindIndex(x => x.Id == SelectedObject.Id && x.GetType() == SelectedObject.GetType());
            if (index == -1) return;
            ListBridgeObjects[index].SetName(fro.GetNewName());
            log.Info($"Renamed object ID : {index} renamed.");

        }

        private void CopyToJson(bool raw)
        {
            Lastmessage = "Object copied to clipboard.";
            Clipboard.SetText(JsonConvert.SerializeObject(SelectedObject, raw ? Formatting.None : Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }

        #endregion

        #region VIEW_TAB_METHODS
        private void SortListView()
        {
            RefreshView();
            WinHueSettings.settings.Sort = MainFormModel.Sort;
            WinHueSettings.Save();
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
        private void OpenSettingsWindow()
        {
            Form_AppSettings settings = new Form_AppSettings() { Owner = Application.Current.MainWindow };
            if (settings.ShowDialog() != true) return;
            Communication.Timeout = WinHueSettings.settings.Timeout;
        }

        private void QuitApplication()
        {
            Application.Current.Shutdown();
        }


        private bool DoBridgePairing(ObservableCollection<Bridge> listBridges = null)
        {
            Form_BridgeDetectionPairing dp = new Form_BridgeDetectionPairing(listBridges) { Owner = Application.Current.MainWindow };

            bool result = (bool)dp.ShowDialog();
            if (result)
            {
                ListBridges = dp.ViewModel.ListBridges;
                SaveSettings();
            }
            return result;
        }
        #endregion

        private void CpuTempMonSettings()
        {
            _ctm.ShowSettingsForm();
        }

        private void ShowID()
        {
            WinHueSettings.settings.ShowID = MainFormModel.ShowId;
            WinHueSettings.Save(); 
        }

        private void WrapText()
        {
            WinHueSettings.settings.WrapText = MainFormModel.WrapText;
            WinHueSettings.Save();
        }

        private void StartProcDump()
        {
            ProcessStartInfo psi = new ProcessStartInfo($"{AppDomain.CurrentDomain.BaseDirectory}procdump.exe")
            {
                Arguments = $@"-e -w -ma WinHue3.exe {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\WinHue3",
                UseShellExecute = false
            };
            Process.Start(psi);
        }
    }
}
