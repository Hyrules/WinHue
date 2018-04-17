using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Advanced_Creator;
using WinHue3.Functions.Application_Settings;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.BridgePairing;
using WinHue3.Functions.BridgeSettings;
using WinHue3.Functions.Groups.Creator;
using WinHue3.Functions.Groups.View;
using WinHue3.Functions.HotKeys;
using WinHue3.Functions.HotKeys.Creator;
using WinHue3.Functions.Lights.Finder;
using WinHue3.Functions.Lights.View;
using WinHue3.Functions.Renaming;
using WinHue3.Functions.ResourceLinks;
using WinHue3.Functions.Rules.Creator;
using WinHue3.Functions.Scenes.Creator;
using WinHue3.Functions.Scenes.View;
using WinHue3.Functions.Schedules.OldCreator;
using WinHue3.Functions.Sensors.Creator;
using WinHue3.Functions.Sensors.Daylight;
using WinHue3.Functions.Sensors.HueTap;
using WinHue3.Functions.User_Management;
using WinHue3.Interface;
using WinHue3.MainForm.Wait;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueMotion;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Utils;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using State = WinHue3.Philips_Hue.HueObjects.LightObject.State;
using WinHue3.Functions.Schedules.NewCreator;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using WinHue3.Functions.Entertainment;

namespace WinHue3.MainForm
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
                    if (SelectedBridge.Name != newname)
                    {
                        SelectedBridge.Name = newname;
                        ListBridges[ListBridges.IndexOf(SelectedBridge)].Name = newname;
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
            RaisePropertyChanged("EnableListView");
        }

        private async Task CheckForBridgeUpdate()
        {
            log.Info("Checking for a bridge update.");
            if (! await _selectedBridge.CheckUpdateAvailableAsyncTask())
            {
                log.Info("No update found on the bridge. Forcing the bridge to check online.");
                await _selectedBridge.CheckOnlineForUpdateAsyncTask();
            }

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
                if (WinHueSettings.bridges.BridgeInfo.ContainsKey(br.Mac))
                    WinHueSettings.bridges.BridgeInfo[br.Mac] = new BridgeSaveSettings
                    {
                        ip = br.IpAddress.ToString(),
                        apikey = br.ApiKey,
                        apiversion = br.ApiVersion,
                        swversion = br.SwVersion,
                        name = br.Name,
                    };
                else
                    WinHueSettings.bridges.BridgeInfo.Add(br.Mac,
                        new BridgeSaveSettings { ip = br.IpAddress.ToString(), apikey = br.ApiKey, apiversion = br.ApiVersion, swversion = br.SwVersion, name = br.Name });

                if (br.IsDefault) WinHueSettings.bridges.DefaultBridge = br.Mac;
            }

            return WinHueSettings.SaveBridges();
        }
        #endregion

        #region HOME_TAB_METHODS

        private void CreateEntertainment()
        {
            Form_EntertainmentCreator fec = new Form_EntertainmentCreator(){ Owner = Application.Current.MainWindow};
            fec.ShowDialog();

        }

        private void CreateAdvanced()
        {
            Form_AdvancedCreator fac = new Form_AdvancedCreator(SelectedBridge)
            {
                Owner = Application.Current.MainWindow,
            };
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
            SelectedHueObject = null;
            if (!_selectedBridge.Virtual)
            {
                if (EnableListView.GetValueOrDefault(false))
                {
                    log.Info($"Getting list of objects from bridge at {SelectedBridge.IpAddress}.");
                    List<IHueObject> hr = await HueObjectHelper.GetBridgeDataStoreAsyncTask(SelectedBridge);
                    if (hr != null)
                    {
                        List<IHueObject> listobj = hr;

                        if (!WinHueSettings.settings.ShowHiddenScenes)
                        {
                            hr.RemoveAll(x => x.GetType() == typeof(Scene) && x.name.StartsWith("HIDDEN"));
                        }
                        ObservableCollection<IHueObject> newlist = new ObservableCollection<IHueObject>();

                        switch (MainFormModel.Sort)
                        {
                            case WinHueSortOrder.Default:
                                ListBridgeObjects = new ObservableCollection<IHueObject>(hr);
                                break;
                            case WinHueSortOrder.Ascending:
                                newlist.AddRange(from item in listobj
                                    where item is Light
                                    orderby item.name
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Group
                                    orderby item.name
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Schedule 
                                    orderby item.name
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Scene
                                    orderby item.name
                                    select item);
                                newlist.AddRange(
                                    from item in listobj where item is Sensor orderby item.name select item);
                                newlist.AddRange(from item in listobj where item is Rule orderby item.name select item);
                                newlist.AddRange(from item in listobj
                                    where item is Resourcelink
                                    orderby item.name
                                    select item);
                                ListBridgeObjects = new ObservableCollection<IHueObject>(newlist);
                                break;
                            case WinHueSortOrder.Descending:
                                newlist.AddRange(from item in listobj
                                    where item is Light
                                    orderby item.name descending
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Group
                                    orderby item.name descending
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Schedule
                                    orderby item.name descending
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Scene
                                    orderby item.name descending
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Sensor
                                    orderby item.name descending
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Rule
                                    orderby item.name descending
                                    select item);
                                newlist.AddRange(from item in listobj
                                    where item is Resourcelink
                                    orderby item.name descending
                                    select item);
                                ListBridgeObjects = new ObservableCollection<IHueObject>(newlist);
                                break;
                            default:
                                goto case WinHueSortOrder.Default;
                        }
                        CreateExpanders();

                    }
                    else
                    {
                        ListBridgeObjects = null;
                        log.Error(hr);
                    }
                }
                else
                {
                    ListBridgeObjects = new ObservableCollection<IHueObject>();
                    log.Info("Bridge update required. Please update the bridge to use WinHue with this bridge.");
                }
            }
            else
            {
                log.Info("Virtual Bridge detected. Will skip refresh.");
            }

            Cursor_Tools.ShowNormalCursor();
        }

        private void CreateExpanders()
        {
            log.Info($"Found {ListBridgeObjects.Count} objects in the bridge.");
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBridgeObjects);
            view.GroupDescriptions?.Clear();
            PropertyGroupDescription groupDesc = new TypeGroupDescription();
            view.GroupDescriptions?.Add(groupDesc);
            log.Info($"Finished refreshing view.");
        }


        private async Task DoubleClickObject()
        {
            log.Debug("Double click on : " + SelectedHueObject);
            if ((SelectedHueObject is Light) || (SelectedHueObject is Group))
            {
                ImageSource hr = await HueObjectHelper.ToggleObjectOnOffStateAsyncTask(SelectedBridge, SelectedHueObject, SliderTt,null, _newstate);
                if (hr != null)
                {
                    ImageSource newimg = hr;
                    SelectedHueObject.Image = newimg;
                    int index = _listBridgeObjects.FindIndex(x => x.Id == SelectedHueObject.Id && x.GetType() == SelectedHueObject.GetType());
                    if (index == -1) return;
                    if (SelectedHueObject is Light light)
                    {
                        light.state.on = !light.state.on;
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
                Scene scene = (Scene) _selectedObject;
                if (!scene.On)
                {
                    await SelectedBridge.ActivateSceneAsyncTask(_selectedObject.Id);
                }
                else
                {
                    foreach (string l in scene.lights)
                    {
                        await SelectedBridge.SetStateAsyncTask(new State() {on = false}, l);
                    }
                }
                ((Scene)_selectedObject).On = !((Scene)_selectedObject).On;
            }

        }

        private async Task Strobe()
        {
            for (int i = 0; i <= 20; i++)
            {
                await _selectedBridge.SetStateAsyncTask(new Action() {@on = true, transitiontime = 0}, "2");
                Thread.Sleep(100);
                await _selectedBridge.SetStateAsyncTask(new Action() {@on = false, transitiontime = 0}, "2");
                Thread.Sleep(100);
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
            Action act = new Action { @on = true };
            if (WinHueSettings.settings.AllOnTT != null) act.transitiontime = WinHueSettings.settings.AllOnTT;
            bool bresult = await SelectedBridge.SetStateAsyncTask(act, "0");
            if (!bresult) return;
            log.Debug("Refreshing the main view.");
            await RefreshView();
        }

        private async Task AllOff()
        {
            log.Info("Sending all off command to bridge" + SelectedBridge.IpAddress);
            Action act = new Action { @on = false };
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

        private async Task DoTouchLink()
        {
            await _selectedBridge.TouchLink();
            Thread.Sleep(3000);
            await CheckForNewBulb();
        }

        private void FindLightSerial()
        {
            Form_AddLightSerial fas = new Form_AddLightSerial(_selectedBridge) { Owner = Application.Current.MainWindow };
            if (fas.ShowDialog() == true)
            {
                Thread.Sleep(60000);
                _findlighttimer.Start();
                RaisePropertyChanged("SearchingLights");
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
            /*Form_ScheduleCreator fscc = new Form_ScheduleCreator() { Owner = Application.Current.MainWindow };
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
            }*/
            
            Form_ScheduleCreator2 fscc = new Form_ScheduleCreator2() { Owner = Application.Current.MainWindow};
            await fscc.Initialize(SelectedBridge);
            if (fscc.ShowDialog() != true) return;
            Schedule sc = (Schedule) await HueObjectHelper.GetObjectAsyncTask(SelectedBridge,fscc.GetCreatedOrModifiedId(), typeof(Schedule));
            if (sc != null)
            {
                _listBridgeObjects.Add(sc);
            }
            else
            {
                SelectedBridge.ShowErrorMessages();
            }
        }

        private async Task CreateRule()
        {

            Form_RuleCreator frc = new Form_RuleCreator(SelectedBridge) { Owner = Application.Current.MainWindow };
            await frc.Initialize();
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
            Sensor hr = HueObjectHelper.GetObject<Sensor>(SelectedBridge, fsc.GetCreatedOrModifiedID());
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
            bool bresult = await SelectedBridge.StartNewObjectsSearchAsyncTask(typeof(Sensor));
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
                if (!(h.objecType == null && SelectedHueObject == null))
                {
                    HotkeyDetected = true;
                    _ledTimer.Start();
                    Type objtype = h?.objecType == null ? SelectedHueObject.GetType() : h.objecType;

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
            bool on = false;
            switch (SelectedHueObject)
            {
                case Light l:
                    @on = l.state.@on.GetValueOrDefault();
                    break;
                case Group g:
                    @on = g.action.@on.GetValueOrDefault();
                    break;
            }

            if (@on)
            {
                await SetHueSliderNow();
            }
            else
            {
                
                switch (SelectedHueObject)
                {
                    case Light _:
                        if (_newstate == null) _newstate = new State();
                        _newstate.hue = MainFormModel.SliderHue;
                        break;
                    case Group _:
                        if (_newstate == null) _newstate = new Action();
                        _newstate.hue = MainFormModel.SliderHue;
                        break;
                }
            }



        }

        private async Task SetHueSliderNow()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.hue = MainFormModel.SliderHue;
            bp.transitiontime = SliderTt;
            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderHue = MainFormModel.OldSliderHue;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedHueObject)
                {
                    case Light light:
                        light.state.hue = MainFormModel.SliderHue;
                        break;
                    case Group @group:
                        @group.action.hue = MainFormModel.SliderHue;
                        break;
                }
            }
        }

        private async Task SliderChangeBri()
        {
            switch (WinHueSettings.settings.SlidersBehavior)
            {
                case 0:
                    await SetBriSliderNow();
                    break;
                case 1:
                    switch (SelectedHueObject)
                    {
                        case Light _:
                            if (_newstate == null) _newstate = new State();
                            _newstate.bri = MainFormModel.SliderBri;
                            break;
                        case Group _:
                            if (_newstate == null) _newstate = new Action();
                            _newstate.bri = MainFormModel.SliderBri;
                            break;
                    }
                    break;
                default:
                    break;
            }


        }

        private async Task SetBriSliderNow()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.bri = MainFormModel.SliderBri;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderBri = MainFormModel.OldSliderBri;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedHueObject)
                {
                    case Light light:
                        light.state.bri = MainFormModel.SliderBri;
                        break;
                    case Group @group:
                        @group.action.bri = MainFormModel.SliderBri;
                        break;
                }
            }
        }


        private async Task SliderChangeCt()
        {
            switch (WinHueSettings.settings.SlidersBehavior)
            {
                case 0:
                    await SetCtSliderNow();
                    break;
                case 1:
                    switch (SelectedHueObject)
                    {
                        case Light _:
                            if (_newstate == null) _newstate = new State();
                            _newstate.ct = MainFormModel.SliderCt;
                            break;
                        case Group _:
                            if (_newstate == null) _newstate = new Action();
                            _newstate.ct = MainFormModel.SliderCt;
                            break;
                    }

                    break;
                default:
                    break;
            }

        }

        private async Task SetCtSliderNow()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.ct = MainFormModel.SliderCt;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderCt = MainFormModel.OldSliderCt;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedHueObject)
                {
                    case Light light:
                        light.state.ct = MainFormModel.SliderCt;
                        break;
                    case Group @group:
                        @group.action.ct = MainFormModel.SliderCt;
                        break;
                }
            }
        }


        private async Task SliderChangeSat()
        {
            switch (WinHueSettings.settings.SlidersBehavior)
            {
                case 0:
                    await SetSatSliderNow();
                    break;
                case 1:
                    switch (SelectedHueObject)
                    {
                        case Light _:
                            if (_newstate == null) _newstate = new State();
                            _newstate.sat = MainFormModel.SliderSat;
                            break;
                        case Group _:
                            if (_newstate == null) _newstate = new Action();
                            _newstate.sat = MainFormModel.SliderSat;
                            break;
                    }

                    break;
                default:
                    break;
            }

        }

        private async Task SetSatSliderNow()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.sat = MainFormModel.SliderSat;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderSat = MainFormModel.OldSliderSat;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedHueObject)
                {
                    case Light light:
                        light.state.sat = MainFormModel.SliderSat;
                        break;
                    case Group @group:
                        @group.action.sat = MainFormModel.SliderSat;
                        break;
                }
            }
        }

        private async Task SliderChangeXy()
        {
            switch (WinHueSettings.settings.SlidersBehavior)
            {
                case 0:
                    await SetXYSlidersNow();
                    break;
                case 1:
                    switch (SelectedHueObject)
                    {
                        case Light _:
                            if (_newstate == null) _newstate = new State();
                            if(_newstate.xy == null) _newstate.xy = new decimal[2];
                            _newstate.xy[0] = MainFormModel.SliderX;
                            _newstate.xy[1] = MainFormModel.SliderY;
                            break;
                        case Group _:
                            if (_newstate == null) _newstate = new Action();
                            if (_newstate.xy == null) _newstate.xy = new decimal[2];
                            _newstate.xy[0] = MainFormModel.SliderX;
                            _newstate.xy[1] = MainFormModel.SliderY;
                            break;
                    }
                    break;
                default:
                    break;
            }


        }

        private async Task SetXYSlidersNow()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
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
                switch (SelectedHueObject)
                {
                    case Light light:
                        light.state.xy[0] = MainFormModel.SliderX;
                        light.state.xy[1] = MainFormModel.SliderY;
                        break;
                    case Group @group:
                        @group.action.xy[0] = MainFormModel.SliderX;
                        @group.action.xy[1] = MainFormModel.SliderY;
                        break;
                }
            }
        }

        private void SetMainFormModel()
        {
            switch (_selectedObject)
            {
                case Light light:
                    light = (Light) _selectedObject;
                
                    MainFormModel.SliderBri = light.state.bri ?? 0;
                    MainFormModel.SliderHue = light.state.hue ?? 0;
                    MainFormModel.SliderSat = light.state.sat ?? 0;
                    MainFormModel.SliderCt = light.state.ct ?? 153;
                    MainFormModel.SliderX = light.state.xy?[0] ?? 0;
                    MainFormModel.SliderY = light.state.xy?[1] ?? 0;
                    MainFormModel.On = light.state.@on ?? false;
                    break;
                case Group group:
                    @group = (Group)_selectedObject;
                
                    MainFormModel.SliderBri = @group.action.bri ?? 0;
                    MainFormModel.SliderHue = @group.action.hue ?? 0;
                    MainFormModel.SliderSat = @group.action.sat ?? 0;
                    MainFormModel.SliderCt = @group.action.ct ?? 153;
                    MainFormModel.SliderX = @group.action.xy?[0] ?? 0;
                    MainFormModel.SliderY = @group.action.xy?[1] ?? 0;
                    MainFormModel.On = @group.action.@on ?? false;
                    break;
                case Scene scene:
                    scene = (Scene) _selectedObject;
                    MainFormModel.On = scene.On;
                    break;
                default:
                    MainFormModel.SliderBri = 0;
                    MainFormModel.SliderHue = 0;
                    MainFormModel.SliderSat = 0;
                    MainFormModel.SliderCt = 153;
                    MainFormModel.SliderX = 0;
                    MainFormModel.SliderY = 0;
                    MainFormModel.On = false;
                    break;
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
                await fgc.Initialize(SelectedBridge, (Group)SelectedHueObject);
                if (fgc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Schedule)
            {
                Form_ScheduleCreator2 fsc = new Form_ScheduleCreator2() { Owner = Application.Current.MainWindow };
                await fsc.Initialize(SelectedBridge);
                fsc.EditSchedule(SelectedHueObject as Schedule);
                if (fsc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Sensor obj)
            {
                obj = (Sensor)_selectedObject;
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
                await frc.Initialize();
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
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.alert = type;
            await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);
        }

        private async Task Clone(bool quick)
        {
            log.Info($"Cloning {SelectedHueObject}...");

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
                    IHueObject oldobj = (IHueObject)SelectedHueObject.Clone();
                    SelectedHueObject = ListBridgeObjects.Single(x => x.GetType() == oldobj.GetType() && x.Id == oldobj.Id);
                    await EditObject();
                }
                catch(Exception)
                {
                    MessageBox.Show(GlobalStrings.Error_While_Cloning, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SetSensorStatus()
        {
            int currentstatus = ((ClipGenericStatusSensorState)((Sensor)SelectedHueObject).state).status;
            log.Info($"Trying to set the sensor {SelectedHueObject.Id} status to {SensorStatus}");           
            bool result = await SelectedBridge.ChangeSensorStateAsyncTask(SelectedHueObject.Id, new ClipGenericStatusSensorState()
            {
                status = SensorStatus
            });
            if (result)
            {
                ((ClipGenericStatusSensorState) ((Sensor) SelectedHueObject).state).status = SensorStatus;                
            }
            else
            {
                SensorStatus = currentstatus;
            }

        }

        private async Task SetSensorFlag()
        {
            bool currentflag = ((ClipGenericFlagSensorState) ((Sensor) SelectedHueObject).state).flag;
            log.Info($"Trying to set the sensor {SelectedHueObject.Id} flag to {SensorFlag}");
            bool result = await SelectedBridge.ChangeSensorStateAsyncTask(SelectedHueObject.Id, new ClipGenericFlagSensorState()
            {
                flag = SensorFlag
            });
            if (result)
            {
                ((ClipGenericFlagSensorState) ((Sensor) SelectedHueObject).state).flag = SensorFlag;                
            }
            else
            {
                SensorFlag = currentflag;
            }
        }

        private async Task<bool> Duplicate()
        {
            
            bool result = await SelectedBridge.CreateObjectAsyncTask(SelectedHueObject);
            
            if (result)
            {
                log.Info("Object cloned succesfully !");
                IHueObject hr = await SelectedBridge.GetObjectAsyncTask(SelectedHueObject.Id, SelectedHueObject.GetType());

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
            await SelectedBridge.ChangeSensorConfigAsyncTask(SelectedHueObject.Id,new HueMotionSensorConfig(){sensitivity = sensitivity});
        }

        private async Task DeleteObject()
        {
            if (
                MessageBox.Show(string.Format(GlobalStrings.Confirm_Delete_Object, SelectedHueObject.name),
                    GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            bool result = await SelectedBridge.RemoveObjectAsyncTask(SelectedHueObject);

            log.Debug($"Object ID {SelectedHueObject.Id} removal " + result);
            if (result)
            {
                int index =
                    ListBridgeObjects.FindIndex(
                        x => x.Id == SelectedHueObject.Id && x.GetType() == SelectedHueObject.GetType());
                if (index == -1) return;
                ListBridgeObjects.RemoveAt(index);
                SelectedHueObject = null;
                log.Info($"Object ID : {index} removed.");
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private void RenameObject()
        {

            
            Form_RenameObject fro = new Form_RenameObject(SelectedBridge, SelectedHueObject) { Owner = Application.Current.MainWindow };
            if (fro.ShowDialog() != true) return;
            SelectedHueObject.name = fro.GetNewName();
            int index = ListBridgeObjects.FindIndex(x => x.Id == SelectedHueObject.Id && x.GetType() == SelectedHueObject.GetType());
            if (index == -1) return;
            ListBridgeObjects[index].name = fro.GetNewName();
            log.Info($"Renamed object ID : {index} renamed.");
            
        }

        private void CopyToJson(bool raw)
        {
            Lastmessage = "Object copied to clipboard.";
            Clipboard.SetText(JsonConvert.SerializeObject(SelectedHueObject, raw ? Formatting.None : Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        private async Task OnDim(byte dimval)
        {
            if (SelectedHueObject is Light)
                await SelectedBridge.SetStateAsyncTask(new State() {bri = dimval, on = true}, SelectedHueObject.Id);
            else
                await SelectedBridge.SetStateAsyncTask(new Action() {bri = dimval, on = true}, SelectedHueObject.Id);
        }

        private async Task Colorloop()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.effect = "colorloop";
            await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

        }

        private async Task NoEffect()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.effect = "none";
            await SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

        }

        #endregion

        #region VIEW_TAB_METHODS
        private async Task SortListView()
        {
            await RefreshView();
            WinHueSettings.settings.Sort = MainFormModel.Sort;
        }

        private void ShowPropertyGrid()
        {
            _propertyGrid.Show();
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

        private async Task ClickObject()
        {
            if (SelectedHueObject != null)
            {
                if (!SelectedBridge.Virtual)
                {
                    IHueObject hr = await HueObjectHelper.GetObjectAsyncTask(SelectedBridge, SelectedHueObject.Id,
                        SelectedHueObject.GetType());
                    if (hr == null) return;
                    _selectedObject = hr;
                    _propertyGrid.SelectedObject = hr;
                }

                if (SelectedHueObject is Sensor sensor)
                {
                    switch (sensor.state)
                    {
                        case ClipGenericFlagSensorState state:
                            SensorFlag = state.flag;
                            break;
                        case ClipGenericStatusSensorState state:
                            SensorStatus = state.status;
                            break;
                        default:
                            break;
                    }
                }
            }

            SetMainFormModel();
        }

        private void DoAppUpdate()
        {
            if(MessageBox.Show(GlobalStrings.UpdateAvailableDownload, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                UpdateManager.DownloadUpdate();
        }

        private void MinimizeToTray()
        {
            Application.Current.MainWindow.Visibility = Visibility.Hidden;
        }

        private void LoadVirtualBridge()
        {
            OpenFileDialog fd = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };
            if (fd.ShowDialog() == DialogResult.OK)
            {
                string file = File.ReadAllText(fd.FileName);
                DataStore ds = JsonConvert.DeserializeObject<DataStore>(file);
                List<IHueObject> hueobjects = HueObjectHelper.ProcessDataStore(ds);
                Bridge vbridge = new Bridge() {Virtual = true, Name = "Virtual Bridge", RequiredUpdate = false};
                ListBridges.Add(vbridge);
                SelectedBridge = vbridge;
                ListBridgeObjects = new ObservableCollection<IHueObject>(hueobjects);
                CreateExpanders();
            }
                
        }

    }
}
