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
using WinHue3.Functions.Animations;
using WinHue3.Functions.Entertainment;
using WinHue3.Functions.EventViewer;
using WinHue3.Functions.PowerSettings;
using WinHue3.Functions.PropertyGrid;
using WinHue3.Functions.RoomMap;
using Binding = System.Windows.Data.Binding;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        #region BRIDGE_SETTINGS

        private async Task ChangeBridgeSettings()
        {
            Form_BridgeSettings frs = new Form_BridgeSettings {Owner = Application.Current.MainWindow};
            await frs.Initialize(BridgeManager.Instance.SelectedBridge);
            if (frs.ShowDialog() == true)
            {
                BridgeSettings bresult = await BridgeManager.Instance.SelectedBridge.GetBridgeSettingsAsyncTask();
                if (bresult != null)
                {
                    string newname = bresult.name;
                    if (BridgeManager.Instance.SelectedBridge.Name != newname)
                    {
                        BridgeManager.Instance.SelectedBridge.Name = newname;
                        BridgeManager.Instance.ListBridges[BridgeManager.Instance.ListBridges.IndexOf(BridgeManager.Instance.SelectedBridge)].Name = newname;
                        Bridge selbr = BridgeManager.Instance.SelectedBridge;
                        BridgeManager.Instance.SelectedBridge = null;
                        BridgeManager.Instance.SelectedBridge = selbr;
                    }

                    await RefreshView();
                    RaisePropertyChanged("UpdateAvailable");
                }
            }
        }

        private async Task ChangeBridge()
        {
            await RefreshView();
            _refreshTimer.Start();
            RaisePropertyChanged("UpdateAvailable");
            RaisePropertyChanged("EnableListView");
        }

        private async Task CheckForBridgeUpdate()
        {
            log.Info("Checking for a bridge update.");
            if (!await BridgeManager.Instance.SelectedBridge.CheckUpdateAvailableAsyncTask())
            {
                log.Info("No update found on the bridge. Forcing the bridge to check online.");
                await BridgeManager.Instance.SelectedBridge.CheckOnlineForUpdateAsyncTask();
            }
        }

        private async Task ManageUsers()
        {
            Form_ManageUsers fmu = new Form_ManageUsers() {Owner = Application.Current.MainWindow};
            await fmu.Initialize(BridgeManager.Instance.SelectedBridge);
            fmu.ShowDialog();
        }

        [Obsolete]
        private bool SaveSettings()
        {
            foreach (Bridge br in BridgeManager.Instance.ListBridges)
            {
                if (br.Mac == string.Empty) continue;
                if (WinHueSettings.bridges.BridgeInfo.ContainsKey(br.Mac))
                    WinHueSettings.bridges.BridgeInfo[br.Mac] = new BridgeSaveSettings
                    {
                        ip = br.IpAddress.ToString(),
                        apikey = br.ApiKey,
                        name = br.Name,
                    };
                else
                    WinHueSettings.bridges.BridgeInfo.Add(br.Mac,
                        new BridgeSaveSettings {ip = br.IpAddress.ToString(), apikey = br.ApiKey, name = br.Name});

                if (br.IsDefault) WinHueSettings.bridges.DefaultBridge = br.Mac;
            }

            return WinHueSettings.SaveBridges();
        }

        #endregion

        #region HOME_TAB_METHODS

        private void CreateAnimation()
        {
            List<IHueObject> listGroupLights = new List<IHueObject>();
            listGroupLights.AddRange(ListBridgeObjects.Where(x => x.GetType() == typeof(Light)));
            listGroupLights.AddRange(ListBridgeObjects.Where(x => x.GetType() == typeof(Group)));
            Form_Animations fa = new Form_Animations(listGroupLights)
            {
                Owner = Application.Current.MainWindow
            };
            fa.ShowDialog();
        }

        private void CreateEntertainment()
        {
            Form_EntertainmentCreator fec = new Form_EntertainmentCreator() {Owner = Application.Current.MainWindow};
            fec.ShowDialog();
        }

        private void CreateAdvanced()
        {
            Form_AdvancedCreator fac = new Form_AdvancedCreator(BridgeManager.Instance.SelectedBridge)
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
            if (BridgeManager.Instance.SelectedBridge == null) return;
            Cursor_Tools.ShowWaitCursor();
            SelectedHueObject = null;
            if (!BridgeManager.Instance.SelectedBridge.Virtual)
            {
                if (EnableListView)
                {
                    log.Info($"Getting list of objects from bridge at {BridgeManager.Instance.SelectedBridge.IpAddress}.");
                    List<IHueObject> hr = await BridgeManager.Instance.SelectedBridge.GetAllObjectsAsync(false,true);
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
            CollectionView view = (CollectionView) CollectionViewSource.GetDefaultView(ListBridgeObjects);
            view.GroupDescriptions?.Clear();
            PropertyGroupDescription groupDesc = new TypeGroupDescription();
            view.GroupDescriptions?.Add(groupDesc);
            log.Info($"Finished refreshing view.");
        }

        private void UpdateFloorPlanIcons(ImageSource image, string id, Type objecttype)
        {
            if (SelectedFloorPlan == null) return;
            HueElement he = SelectedFloorPlan.Elements.FirstOrDefault(x => x.Id == id && x.HueType == GetHueElementEnumFromType(objecttype));
            if (he == null) return;
            he.Image = image;
        }

        private async Task DoubleClickObject()
        {
            log.Debug("Double click on : " + SelectedHueObject);
            if ((SelectedHueObject is Light) || (SelectedHueObject is Group ))
            {
                ImageSource hr = await BridgeManager.Instance.SelectedBridge.ToggleObjectOnOffStateAsyncTask(SelectedHueObject, SliderTt, null, _newstate);
                if (hr != null)
                {
                    
                    SelectedHueObject.Image = hr;
                    UpdateFloorPlanIcons(hr, SelectedHueObject.Id, SelectedHueObject.GetType());

                    int index = _listBridgeObjects.FindIndex(x => x.Id == SelectedHueObject.Id && x.GetType() == SelectedHueObject.GetType());
                    if (index == -1) return;
                    if (SelectedHueObject is Light light)
                    {
                        light.state.on = !light.state.on;
                        ((Light) ListBridgeObjects[index]).state.on = !((Light) ListBridgeObjects[index]).state.on;
                    }
                    else
                    {
                        ((Group) _selectedObject).action.on = !((Group) _selectedObject).action.on;
                        ((Group) ListBridgeObjects[index]).action.on = !((Group) ListBridgeObjects[index]).action.on;
                    }

                    ListBridgeObjects[index].Image = hr;
                }
            }
            else
            {
                log.Info($"Activating scene : {_selectedObject.Id}");
                Scene scene = (Scene) _selectedObject;
                if (!scene.On)
                {
                    await BridgeManager.Instance.SelectedBridge.ActivateSceneAsyncTask(_selectedObject.Id);
                }
                else
                {
                    foreach (string l in scene.lights)
                    {
                        await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(new State() {on = false}, l);
                    }
                }

                ((Scene) _selectedObject).On = !((Scene) _selectedObject).On;
            }
        }

        private async Task Strobe()
        {
            for (int i = 0; i <= 20; i++)
            {
                await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(new Action() {@on = true, transitiontime = 0}, "2");
                Thread.Sleep(100);
                await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(new Action() {@on = false, transitiontime = 0}, "2");
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

            IHueObject hr = await BridgeManager.Instance.SelectedBridge.GetObjectAsync(obj.Id, obj.GetType());

            if (hr == null) return;
            IHueObject newobj = hr;
            ListBridgeObjects[index].Image = newobj.Image;
            List<PropertyInfo> pi = newobj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList();

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
            Form_EventLog fel = new Form_EventLog(); 
            log.Debug("Opening event log.");
            fel.Show();
        }

        private async Task AllOnOff(bool onoff)
        {
            string onoffs = onoff ? "on" : "off";
            log.Info($"Sending all {onoffs} command to bridge" + BridgeManager.Instance.SelectedBridge.IpAddress);
            Action act = new Action {@on = onoff};
            if (WinHueSettings.settings.AllOnTT != null) act.transitiontime = WinHueSettings.settings.AllOnTT;
            bool bresult = await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(act, "0");
            if (!bresult) return;
            log.Debug("Refreshing the main view.");
            await RefreshView();
        }

        private async Task CheckForNewBulb()
        {
            bool success = await BridgeManager.Instance.SelectedBridge.StartNewObjectsSearchAsyncTask(typeof(Light));
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
            await BridgeManager.Instance.SelectedBridge.TouchLink();
            Thread.Sleep(3000);
            await CheckForNewBulb();
        }

        private void FindLightSerial()
        {
            Form_AddLightSerial fas = new Form_AddLightSerial() {Owner = Application.Current.MainWindow};
            if (fas.ShowDialog() == true)
            {
                Thread.Sleep(60000);
                _findlighttimer.Start();
                RaisePropertyChanged("SearchingLights");
            }
        }

        private async Task CreateGroup()
        {
            Form_GroupCreator fgc = new Form_GroupCreator() {Owner = Application.Current.MainWindow};
            await fgc.Initialize();
            log.Debug($@"Opening the Group creator window for bridge {BridgeManager.Instance.SelectedBridge.IpAddress} ");
            if (fgc.ShowDialog() != true) return;
            if (fgc.GetCreatedOrModifiedID() == null) return;

            Group hr = BridgeManager.Instance.SelectedBridge.GetObject<Group>(fgc.GetCreatedOrModifiedID());
            if (hr != null)
            {
                _listBridgeObjects.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
            }
        }

        private async Task CreateScene()
        {
            Form_SceneCreator fsc = new Form_SceneCreator() {Owner = Application.Current.MainWindow};
            await fsc.Inititalize();
            log.Debug($@"Opening the scene creator window for bridge {BridgeManager.Instance.SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created scene ID {fsc.GetCreatedOrModifiedID()} from bridge {BridgeManager.Instance.SelectedBridge.IpAddress}");
            Scene hr = await BridgeManager.Instance.SelectedBridge.GetObjectAsync<Scene>(fsc.GetCreatedOrModifiedID());
            if (hr != null)
            {
                _listBridgeObjects.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
            }
        }

        private async Task CreateSchedule()
        {

            Form_ScheduleCreator2 fscc = new Form_ScheduleCreator2() {Owner = Application.Current.MainWindow};
            await fscc.Initialize();
            if (fscc.ShowDialog() != true) return;
            Schedule sc = await BridgeManager.Instance.SelectedBridge.GetObjectAsync<Schedule>(fscc.GetCreatedOrModifiedId());
            if (sc != null)
            {
                _listBridgeObjects.Add(sc);
            }
            else
            {
                BridgeManager.Instance.SelectedBridge.ShowErrorMessages();
            }
        }

        private async Task CreateRule()
        {
            Form_RuleCreator frc = new Form_RuleCreator() {Owner = Application.Current.MainWindow};
            await frc.Initialize();
            if (frc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly sensor schedule ID {frc.GetCreatedOrModifiedID()} from bridge {BridgeManager.Instance.SelectedBridge.IpAddress}");
            Rule rule = await BridgeManager.Instance.SelectedBridge.GetObjectAsync<Rule>(frc.GetCreatedOrModifiedID());
            if (rule != null)
            {
                _listBridgeObjects.Add(rule);
            }
            else
            {
                BridgeManager.Instance.SelectedBridge.ShowErrorMessages();
            }
        }

        private void CreateSensor()
        {
            Form_SensorCreator fsc = new Form_SensorCreator() {Owner = Application.Current.MainWindow};
            log.Debug($@"Opening the sensor creator window passing bridge {BridgeManager.Instance.SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created sensor ID {fsc.GetCreatedOrModifiedID()} from bridge {BridgeManager.Instance.SelectedBridge.IpAddress}");
            Sensor hr = BridgeManager.Instance.SelectedBridge.GetObject<Sensor>(fsc.GetCreatedOrModifiedID());
            if (hr != null)
            {
                ListBridgeObjects.Add(hr);
            }
            else
            {
                BridgeManager.Instance.SelectedBridge.ShowErrorMessages();
            }
        }

        private async Task SearchNewSensors()
        {
            bool bresult = await BridgeManager.Instance.SelectedBridge.StartNewObjectsSearchAsyncTask(typeof(Sensor));
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
            Form_HotKeyCreator fhkc = new Form_HotKeyCreator() {Owner = Application.Current.MainWindow};
            await fhkc.Initialize();
            fhkc.ShowDialog();
            _listHotKeys = fhkc.GetHotKeys();
            LoadHotkeys();
        }


        private async Task CreateResourceLink()
        {
            Form_ResourceLinksCreator frc = new Form_ResourceLinksCreator() {Owner = Application.Current.MainWindow};
            await frc.Initialize();
            log.Debug($@"Opening the sensor ResourceLink window passing bridge {BridgeManager.Instance.SelectedBridge.IpAddress} ");
            if (!(bool) frc.ShowDialog()) return;
            log.Debug($@"Getting the newly created ResourceLink ID {frc.GetCreatedModifiedId()} from bridge {BridgeManager.Instance.SelectedBridge.IpAddress}");
            Resourcelink hr = BridgeManager.Instance.SelectedBridge.GetObject<Resourcelink>(frc.GetCreatedModifiedId());
            if (hr != null)
            {
                ListBridgeObjects.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
            }
        }

        private async Task DoBridgeUpdate()
        {
            if (MessageBox.Show(GlobalStrings.Update_Confirmation, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            log.Info("Updating bridge to the latest firmware.");
            bool bresult = await BridgeManager.Instance.SelectedBridge.UpdateBridgeAsyncTask();
            if (!bresult)
            {
                log.Error("An error occured while trying to start a bridge update. Please try again later.");
                return;
            }

            Form_Wait fw = new Form_Wait();
            fw.ShowWait(GlobalStrings.ApplyingUpdate, new TimeSpan(0, 0, 0, 180), Application.Current.MainWindow);
            BridgeSettings bsettings = await BridgeManager.Instance.SelectedBridge.GetBridgeSettingsAsyncTask();
            if (bsettings != null)
            {
                switch (bsettings.swupdate.updatestate)
                {
                    case 0:
                        log.Info("Bridge updated succesfully");
                        BridgeManager.Instance.SelectedBridge.SetNotify();
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

        private void HandleHotkey(HotKeyHandle e)
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

                    if (objtype == typeof(Scene))
                    {
                        BridgeManager.Instance.SelectedBridge.ActivateScene(h.id);
                    }
                    else if (objtype == typeof(Group) || objtype == typeof(Light))
                    {
                        BridgeManager.Instance.SelectedBridge.SetState(h.properties, h.id);
                    }
                    else
                    {
                        log.Warn($"Type of object {objtype} not supported");
                    }

                    if (h.ProgramPath == null) return;
                    if (File.Exists(h.ProgramPath))
                    {
                        log.Info($"Starting application at {h.ProgramPath}");
                        Process.Start(h.ProgramPath);
                    }
                    else
                    {
                        log.Error($"Application at {h.ProgramPath} does not exist anymore. Ignoring it...");
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
            bool result = await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderHue = MainFormModel.OldSliderHue;
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
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

            bool result = await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderBri = MainFormModel.OldSliderBri;
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
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

            bool result = await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderCt = MainFormModel.OldSliderCt;
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
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

            bool result = await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderSat = MainFormModel.OldSliderSat;
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
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
                            if (_newstate.xy == null) _newstate.xy = new decimal[2];
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

            bool result = await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderX = MainFormModel.OldSliderX;
                MainFormModel.SliderY = MainFormModel.OldSliderY;
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
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
                    @group = (Group) _selectedObject;

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
            Form_SceneMapping fsm = new Form_SceneMapping() {Owner = Application.Current.MainWindow};
            await fsm.Initialize();
            fsm.Show();
        }

        private async Task ViewBulbs()
        {
            Form_BulbsView fbv = new Form_BulbsView() {Owner = Application.Current.MainWindow};
            await fbv.Initialize();
            fbv.Show();
        }

        private async Task ViewGroups()
        {
            Form_GroupView fgv = new Form_GroupView() {Owner = Application.Current.MainWindow};
            await fgv.Initialize();
            fgv.Show();
        }

        #endregion

        #region CONTEXT_MENU_METHODS

        private void SetPowerMode()
        {
            Form_PowerFailureSettings fps = new Form_PowerFailureSettings()
            {
                Owner = Application.Current.MainWindow
            };
            fps.ShowDialog();
        }

        private async Task ReplaceCurrentState()
        {
            if (MessageBox.Show(GlobalStrings.Scene_Replace_Current_States, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            log.Info($@"Replacing scene {((Scene) _selectedObject).name} lights state with current one.");
            await BridgeManager.Instance.SelectedBridge.StoreCurrentLightStateAsyncTask(_selectedObject.Id);
        }

        private async Task EditObject()
        {
            log.Debug("Editing object : " + _selectedObject);

            if (_selectedObject is Group)
            {
                Form_GroupCreator fgc = new Form_GroupCreator() {Owner = Application.Current.MainWindow};
                await fgc.Initialize((Group) SelectedHueObject);
                if (fgc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Schedule)
            {
                Form_ScheduleCreator2 fsc = new Form_ScheduleCreator2() {Owner = Application.Current.MainWindow};
                await fsc.Initialize();
                fsc.EditSchedule(SelectedHueObject as Schedule);
                if (fsc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Sensor obj)
            {
                obj = (Sensor) _selectedObject;
                switch (obj.modelid)
                {
                    case "PHDL00":
                        Sensor cr = await BridgeManager.Instance.SelectedBridge.GetObjectAsync<Sensor>(obj.Id);
                        if (cr != null)
                        {
                            cr.Id = obj.Id;
                            Form_Daylight dl = new Form_Daylight(cr) {Owner = Application.Current.MainWindow};
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
                        await htc.Initialize(obj.Id);
                        if (htc.ShowDialog() == true)
                        {
                            await RefreshObject(_selectedObject);
                        }

                        break;
                    default:
                        Sensor crs = await BridgeManager.Instance.SelectedBridge.GetObjectAsync<Sensor>(obj.Id);
                        if (crs != null)
                        {
                            Form_SensorCreator fsc = new Form_SensorCreator(crs)
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
                Form_RuleCreator frc = new Form_RuleCreator((Rule) _selectedObject) {Owner = Application.Current.MainWindow};
                await frc.Initialize();
                if (frc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Scene)
            {
                Form_SceneCreator fscc = new Form_SceneCreator() {Owner = Application.Current.MainWindow};
                await fscc.Inititalize(_selectedObject.Id);
                if (fscc.ShowDialog() == true)
                {
                    await RefreshObject(_selectedObject);
                }
            }
            else if (_selectedObject is Resourcelink)
            {
                Form_ResourceLinksCreator frlc = new Form_ResourceLinksCreator() {Owner = Application.Current.MainWindow};
                await frlc.Initialize((Resourcelink) _selectedObject);
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
            await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);
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
                    IHueObject oldobj = (IHueObject) SelectedHueObject.Clone();
                    SelectedHueObject = ListBridgeObjects.Single(x => x.GetType() == oldobj.GetType() && x.Id == oldobj.Id);
                    await EditObject();
                }
                catch (Exception)
                {
                    MessageBox.Show(GlobalStrings.Error_While_Cloning, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SetSensorStatus()
        {
            int currentstatus = ((ClipGenericStatusSensorState) ((Sensor) SelectedHueObject).state).status;
            log.Info($"Trying to set the sensor {SelectedHueObject.Id} status to {SensorStatus}");
            bool result = await BridgeManager.Instance.SelectedBridge.ChangeSensorStateAsyncTask(SelectedHueObject.Id, new ClipGenericStatusSensorState()
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
            bool result = await BridgeManager.Instance.SelectedBridge.ChangeSensorStateAsyncTask(SelectedHueObject.Id, new ClipGenericFlagSensorState()
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
            bool result = await BridgeManager.Instance.SelectedBridge.CreateObjectAsyncTask(SelectedHueObject);

            if (result)
            {
                log.Info("Object cloned succesfully !");
                IHueObject hr = await BridgeManager.Instance.SelectedBridge.GetObjectAsync(SelectedHueObject.Id, SelectedHueObject.GetType());

                if (hr != null)
                {
                    ListBridgeObjects.Add(hr);
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
                }
            }
            else
            {
                log.Error("Error while cloning object.");
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
            }

            return result;
        }

        private async Task Sensitivity(int sensitivity)
        {
            await BridgeManager.Instance.SelectedBridge.ChangeSensorConfigAsyncTask(SelectedHueObject.Id, new HueMotionSensorConfig() {sensitivity = sensitivity});
        }

        private async Task DeleteObject()
        {
            if (
                MessageBox.Show(string.Format(GlobalStrings.Confirm_Delete_Object, SelectedHueObject.name),
                    GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            bool result = await BridgeManager.Instance.SelectedBridge.RemoveObjectAsyncTask(SelectedHueObject);

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
                MessageBoxError.ShowLastErrorMessages(BridgeManager.Instance.SelectedBridge);
            }
        }

        private void RenameObject()
        {
            Form_RenameObject fro = new Form_RenameObject(BridgeManager.Instance.SelectedBridge, SelectedHueObject) {Owner = Application.Current.MainWindow};
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
            Clipboard.SetText(JsonConvert.SerializeObject(SelectedHueObject, raw ? Formatting.None : Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }

        private async Task OnDim(byte dimval)
        {
            if (SelectedHueObject is Light)
                await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(new State() {bri = dimval, on = true}, SelectedHueObject.Id);
            else
                await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(new Action() {bri = dimval, on = true}, SelectedHueObject.Id);
        }

        private async Task Colorloop()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.effect = "colorloop";
            await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);
        }

        private async Task NoEffect()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedHueObject.GetType());
            bp.effect = "none";
            await BridgeManager.Instance.SelectedBridge.SetStateAsyncTask(bp, _selectedObject.Id);
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
            Form_PropertyGrid fpg = new Form_PropertyGrid();
            fpg.Owner = Application.Current.MainWindow;
            Binding selectedBinding = new Binding("SelectedHueObject");
            selectedBinding.Source = this;
            selectedBinding.Mode = BindingMode.TwoWay;          
            BindingOperations.SetBinding(fpg, Form_PropertyGrid.SelectedObjectProperty, selectedBinding);
            fpg.Show();
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
            Form_AppSettings settings = new Form_AppSettings {Owner = Application.Current.MainWindow};
            if (settings.ShowDialog() != true) return;
            Comm.Timeout = WinHueSettings.settings.Timeout;
            if (MainFormModel.ShowId != WinHueSettings.settings.ShowID)
            {
                MainFormModel.ShowId = WinHueSettings.settings.ShowID;
            }

            if (MainFormModel.WrapText != WinHueSettings.settings.WrapText || 
                MainFormModel.Sort != WinHueSettings.settings.Sort || 
                MainFormModel.Showhiddenscenes != WinHueSettings.settings.ShowHiddenScenes)
            {
                MainFormModel.Sort = WinHueSettings.settings.Sort;
                MainFormModel.WrapText = WinHueSettings.settings.WrapText;
                await RefreshView();
            }

            MainFormModel.ShowFloorPlanTab = WinHueSettings.settings.ShowFloorPlanTab;

            if (SliderTt != WinHueSettings.settings.DefaultTT)
            {
                SliderTt = WinHueSettings.settings.DefaultTT;
            }
        }

        private void QuitApplication()
        {
            Application.Current.Shutdown();
        }

        private void RefreshHueObject(ref IHueObject obj, IHueObject newobject)
        {
            if (obj == null || newobject == null) return;
            if (obj.GetType() != newobject.GetType()) return;
            if (obj.Id != newobject.Id) return;

            PropertyInfo[] pi = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo p in pi)
            {
                object value = p.GetValue(newobject);
                p.SetValue(obj, value);
            }
        }

        private async Task ExportDataStore(object param)
        {
            if (param == null) return;
            string p = param.ToString();
            string data = string.Empty;
            JsonSerializerSettings jss = new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore};

            SaveFileDialog sfd = new SaveFileDialog
            {
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = @"Text File (*.txt) | *.txt"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                List<IHueObject> listobject = await BridgeManager.Instance.SelectedBridge.GetAllObjectsAsync();

                data = JsonConvert.SerializeObject(listobject.ToDictionary(x => x.Id, x => x), Formatting.Indented, jss);

                if (data != string.Empty)
                {
                    try
                    {
                        log.Info($"Saving {p} to file {sfd.FileName}...");
                        File.WriteAllText(sfd.FileName, data);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"An exception occured while saving the config. {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show(GlobalStrings.UnableToExport, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    log.Error($"Error while saving to file.");
                }
            }
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
                if (!BridgeManager.Instance.SelectedBridge.Virtual)
                {
                    IHueObject hr = await BridgeManager.Instance.SelectedBridge.GetObjectAsync(SelectedHueObject.Id,SelectedHueObject.GetType());
                    if (hr == null) return;
                    _selectedObject = hr;
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
            if (MessageBox.Show(GlobalStrings.UpdateAvailableDownload, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                UpdateManager.DownloadUpdate();
        }

        private void MinimizeToTray()
        {
            Application.Current.MainWindow.Visibility = Visibility.Hidden;
        }

        private void LoadVirtualBridge()
        {           
            ListBridgeObjects = BridgeManager.Instance.LoadVirtualBridge();
            if(ListBridgeObjects != null)
                CreateExpanders();
        }

        private void CreateFloorPlan()
        {
            Form_RoomMapCreator frmp = new Form_RoomMapCreator(_listBridgeObjects.ToList())
            {
                Owner = Application.Current.MainWindow
            };
            frmp.ShowDialog();
            LoadFloorPlans();
        }

        private async Task SelectHueElement()
        {
            if (SelectedHueElement == null) return;
            SelectedHueObject = ListBridgeObjects.FirstOrDefault(x => x.Id == SelectedHueElement.Id && x.GetType() == GetTypeFromHueElementEnum(SelectedHueElement.HueType));
            await ClickObject();
            SelectedHueElement.Image = SelectedHueObject.Image;
        }

        private Type GetTypeFromHueElementEnum(HueElementType type)
        {
            switch (type)
            {
                case HueElementType.Light:
                    return typeof(Light);
                case HueElementType.Group:
                    return typeof(Group);
                case HueElementType.Scene:
                    return typeof(Scene);
            }

            return null;
        }

        private HueElementType GetHueElementTypeFromString(string type)
        {
            switch (type)
            {
                case "lights":
                case "light":
                    return HueElementType.Light;
                case "groups":
                case "group":
                    return HueElementType.Group;
                default:
                    return HueElementType.Other;

                    
            }
        }

        private HueElementType GetHueElementEnumFromType(Type type)
        {
            return type == typeof(Group) ? HueElementType.Group : HueElementType.Light;
        }

        private void SelectedFloorPlanChanged()
        {
            if (SelectedFloorPlan == null) return;
            foreach (Tuple<string, string> t in WinHueSettings.bridges.BridgeInfo[BridgeManager.Instance.SelectedBridge.Mac].hiddenobjects)
            {
                if (SelectedFloorPlan.Elements.Any(x => x.Id == t.Item1 && x.HueType == GetHueElementTypeFromString(t.Item2)))
                {
                    SelectedFloorPlan.Elements.Remove(SelectedFloorPlan.Elements.FirstOrDefault(x => x.Id == t.Item1 && x.HueType == GetHueElementTypeFromString(t.Item2)));
                }
            }

            foreach (HueElement h in SelectedFloorPlan.Elements)
            {
                IHueObject obj = ListBridgeObjects.FirstOrDefault(x => x.Id == h.Id && x.GetType() == GetTypeFromHueElementEnum(h.HueType));
                if (obj == null) continue;
                h.Image = obj.Image;
            }
        }

        private async Task CallCommandOnKeyPress(Key pressed, Func<Task> callback)
        {
            if (pressed == Key.Up || pressed == Key.Right || pressed == Key.Down || pressed == Key.Left)
            {
                if (callback != null)
                    await callback();
            }
        }

        private async Task SliderChangeHueKeypress(object e)
        {
            KeyEventArgs kp = e as KeyEventArgs;
            await CallCommandOnKeyPress(kp.Key, SliderChangeHue);
        }

        private async Task SliderChangeBriKeypress(object e)
        {
            KeyEventArgs kp = e as KeyEventArgs;
            await CallCommandOnKeyPress(kp.Key, SliderChangeBri);
        }

        private async Task SliderChangeSatKeypress(object e)
        {
            KeyEventArgs kp = e as KeyEventArgs;
            await CallCommandOnKeyPress(kp.Key, SliderChangeSat);
        }

        private async Task SliderChangeCtKeypress(object e)
        {
            KeyEventArgs kp = e as KeyEventArgs;
            await CallCommandOnKeyPress(kp.Key, SliderChangeCt);
        }
    }
}