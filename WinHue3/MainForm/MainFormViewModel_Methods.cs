using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using WinHue3.Functions.Advanced_Creator;
using WinHue3.Functions.Application_Settings;
using WinHue3.Functions.Application_Settings.Settings;
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
using WinHue3.MainForm.Wait;
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
using WinHue3.Functions.Animations;
using WinHue3.Functions.Entertainment;
using WinHue3.Functions.EventViewer;
using WinHue3.Functions.Mqtt.Client;
using WinHue3.Functions.PowerSettings;
using WinHue3.Functions.PropertyGrid;
using WinHue3.Functions.RoomMap;
using WinHue3.Philips_Hue.Communication2;
using Binding = System.Windows.Data.Binding;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WinHue3.MainForm
{
    public partial class MainFormViewModel : ValidatableBindableBase
    {
        #region BRIDGE_SETTINGS

        private async Task CheckForBridgeUpdate()
        {
            log.Info("Checking for a bridge update.");
            if (!await SelectedBridge.CheckUpdateAvailableAsyncTask())
            {
                log.Info("No update found on the bridge. Forcing the bridge to check online.");
                await SelectedBridge.CheckOnlineForUpdateAsyncTask();
            }
        }

        private async Task ManageUsers()
        {
            Form_ManageUsers fmu = new Form_ManageUsers() {Owner = Application.Current.MainWindow};
            await fmu.Initialize(SelectedBridge);
            fmu.ShowDialog();
        }

        #endregion

        #region HOME_TAB_METHODS

        private void CreateAnimation()
        {
            List<IHueObject> listGroupLights = new List<IHueObject>();
            Form_Animations fa = new Form_Animations()
            {
                Owner = Application.Current.MainWindow
            };
            fa.ShowDialog();
        }

        private async Task CreateEntertainment()
        {
            Form_EntertainmentCreator fec = new Form_EntertainmentCreator() {Owner = Application.Current.MainWindow};
            await fec.Initialize(SelectedBridge);
            if (fec.ShowDialog().GetValueOrDefault())
            {
                

            }

        }

        private void CreateAdvanced()
        {
            Form_AdvancedCreator fac = new Form_AdvancedCreator()
            {
                Owner = Application.Current.MainWindow,
            };
            fac.Initialize(SelectedBridge);
            fac.OnObjectCreated += Fac_OnObjectCreated;
            fac.Show();
        }

        private async void Fac_OnObjectCreated(object sender, EventArgs e)
        {
            await RefreshCurrentListHueObject();
        }

        private void UpdateFloorPlanIcons(ImageSource image ,string id, Type objecttype)
        {
            HueElement he = SelectedFloorPlan?.Elements.FirstOrDefault(x => x.Id == id && x.HueType == GetHueElementEnumFromType(objecttype));
            if (he == null) return;
            he.Image = image;
        }

        private async Task DoubleClickObject()
        {
            log.Debug("Double click on : " + SelectedObject);

            switch (SelectedObject)
            {
                case Light light:
                    if (!await SelectedBridge.ToggleObjectOnOffStateAsyncTask(SelectedObject, SliderTt, null, _newstate)) break;
                    light.state.on = !light.state.on;
                    light.RefreshImage();
                    UpdateFloorPlanIcons(light.Image, SelectedObject.Id, SelectedObject.GetType());
                    break;
                case Group group:
                    if (!await SelectedBridge.ToggleObjectOnOffStateAsyncTask(SelectedObject, SliderTt, null, _newstate)) break;
                    group.state.all_on = !group.state.all_on;
                    group.RefreshImage();
                    UpdateFloorPlanIcons(group.Image, SelectedObject.Id, SelectedObject.GetType());
                    break;
                case Scene scene:
                    log.Info($"Activating scene : {SelectedObject.Id}");
                    if (!scene.On)
                    {
                        await SelectedBridge.ActivateSceneAsyncTask(SelectedObject.Id);
                    }
                    else
                    {
                        foreach (string l in scene.lights)
                        {
                            await SelectedBridge.SetStateAsyncTask(new State() { on = false }, l);
                        }
                    }

                    scene.On = !scene.On;
                    break;
            }

        }

        private async Task Strobe()
        {
            for (int i = 0; i <= 20; i++)
            {
                await SelectedBridge.SetStateAsyncTask(new Action() {on = true, transitiontime = 0}, "2");
                Thread.Sleep(100);
                await SelectedBridge.SetStateAsyncTask(new Action() {on = false, transitiontime = 0}, "2");
                Thread.Sleep(100);
            }
        }

        private void ResetTransitionTime()
        {
            SliderTt = WinHueSettings.settings.DefaultTT;
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
            log.Info($"Sending all {onoffs} command to bridge" + SelectedBridge.IpAddress);
            Action act = new Action {on = onoff};
            if (WinHueSettings.settings.AllOnTT != null) act.transitiontime = WinHueSettings.settings.AllOnTT;
            if (!await SelectedBridge.SetStateAsyncTask(act, "0")) return;
            log.Debug("Refreshing the main view.");
            await RefreshCurrentListHueObject();
        }

        private async Task CheckForNewBulb()
        {
            if (await SelectedBridge.StartNewObjectsSearchAsyncTask(typeof(Light)))
            {
                log.Info("Seaching for new lights...");
                FindLight();
            }
            else
            {
                MessageBox.Show(GlobalStrings.Error_Getting_NewLights, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error("There was an error while looking for new lights.");
            }
        }

        private async Task DoTouchLink()
        {
            await SelectedBridge.TouchLink();
            Thread.Sleep(3000);
            await CheckForNewBulb();
        }

        private void FindLightSerial()
        {
            Form_AddLightSerial fas = new Form_AddLightSerial(SelectedBridge) {Owner = Application.Current.MainWindow};
            if (fas.ShowDialog() == true)
            {
                Thread.Sleep(60000);
                FindLight();
            }
        }

        private async Task CreateGroup()
        {
            Form_GroupCreator fgc = new Form_GroupCreator() {Owner = Application.Current.MainWindow};
            await fgc.Initialize(SelectedBridge);
            log.Debug($@"Opening the Group creator window for bridge {SelectedBridge.IpAddress} ");
            if (fgc.ShowDialog() != true) return;
            if (fgc.GetCreatedOrModifiedID() == null) return;

            Group hr = SelectedBridge.GetObject<Group>(fgc.GetCreatedOrModifiedID());
            if (hr != null)
            {
                CurrentBridgeHueObjectsList.Add(hr);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private async Task CreateEditGroup(Group g)
        {
            Form_GroupCreator fgc = new Form_GroupCreator() {Owner = Application.Current.MainWindow};
            await fgc.Initialize(SelectedBridge);


        }

        private async Task CreateScene()
        {
            Form_SceneCreator fsc = new Form_SceneCreator() {Owner = Application.Current.MainWindow};
            await fsc.Inititalize(SelectedBridge);
            log.Debug($@"Opening the scene creator window for bridge {SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created scene ID {fsc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            Scene hr = await SelectedBridge.GetObjectAsync<Scene>(fsc.GetCreatedOrModifiedID());
            if (hr != null)
            {
                CurrentBridgeHueObjectsList.Add(hr);
            }
            else
            {
                SelectedBridge.ShowErrorMessages();
            }
        }

        private async Task CreateSchedule()
        {

            Form_ScheduleCreator2 fscc = new Form_ScheduleCreator2() {Owner = Application.Current.MainWindow};
            await fscc.Initialize(SelectedBridge);
            if (fscc.ShowDialog() != true) return;
            Schedule sc = await SelectedBridge.GetObjectAsync<Schedule>(fscc.GetCreatedOrModifiedId());
            if (sc != null)
            {
                CurrentBridgeHueObjectsList.Add(sc);
            }
            else
            {
                SelectedBridge.ShowErrorMessages();
            }
        }

        private async Task CreateRule()
        {
            Form_RuleCreator frc = new Form_RuleCreator() {Owner = Application.Current.MainWindow};
            await frc.Initialize(SelectedBridge);
            if (frc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly sensor schedule ID {frc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            Rule rule = await SelectedBridge.GetObjectAsync<Rule>(frc.GetCreatedOrModifiedID());
            if (rule != null)
            {
                CurrentBridgeHueObjectsList.Add(rule);
            }
            else
            {
                SelectedBridge.ShowErrorMessages();
            }
        }

        private void CreateSensor()
        {
            Form_SensorCreator fsc = new Form_SensorCreator(SelectedBridge) {Owner = Application.Current.MainWindow};
            log.Debug($@"Opening the sensor creator window passing bridge {SelectedBridge.IpAddress} ");
            if (fsc.ShowDialog() != true) return;
            log.Debug($@"Getting the newly created sensor ID {fsc.GetCreatedOrModifiedID()} from bridge {SelectedBridge.IpAddress}");
            Sensor hr = SelectedBridge.GetObject<Sensor>(fsc.GetCreatedOrModifiedID());
            if (hr != null)
            {
                CurrentBridgeHueObjectsList.Add(hr);
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
                FindSensor();
            }
            else
            {
                log.Error("Unable to look for new sensors. Please check the log for more details.");
            }
        }

        private async Task CreateHotKey()
        {
            _hkm.StopHotKeyCapture();
            Form_HotKeyCreator fhkc = new Form_HotKeyCreator() {Owner = Application.Current.MainWindow};
            await fhkc.Initialize(SelectedBridge,_hkm.ListHotKeys);
            fhkc.ShowDialog();
            _hkm.StartHotKeyCapture();
        }


        private async Task CreateResourceLink()
        {
            Form_ResourceLinksCreator frc = new Form_ResourceLinksCreator() {Owner = Application.Current.MainWindow};
            await frc.Initialize(SelectedBridge);
            log.Debug($@"Opening the sensor ResourceLink window passing bridge {SelectedBridge.IpAddress} ");
            if (!(bool) frc.ShowDialog()) return;
            log.Debug($@"Getting the newly created ResourceLink ID {frc.GetCreatedModifiedId()} from bridge {SelectedBridge.IpAddress}");
            Resourcelink hr = SelectedBridge.GetObject<Resourcelink>(frc.GetCreatedModifiedId());
            if (hr != null)
            {
                CurrentBridgeHueObjectsList.Add(hr);
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



    

        private void UnloadHotkeys()
        {

        }

        #endregion

        #region SLIDERS_METHODS

        private async Task SliderChangeHue()
        {
            bool on = false;
            switch (SelectedObject)
            {
                case Light l:
                    on = l.state.on.GetValueOrDefault();
                    break;
                case Group g:
                    on = g.action.on.GetValueOrDefault();
                    break;
            }

            if (on)
            {
                await SetHueSliderNow();
            }
            else
            {
                switch (SelectedObject)
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
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.hue = MainFormModel.SliderHue;
            bp.transitiontime = SliderTt;
            bool result = await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderHue = MainFormModel.OldSliderHue;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedObject)
                {
                    case Light light:
                        light.state.hue = MainFormModel.SliderHue;
                        break;
                    case Group group:
                        group.action.hue = MainFormModel.SliderHue;
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
                    switch (SelectedObject)
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
            }
        }

        private async Task SetBriSliderNow()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.bri = MainFormModel.SliderBri;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderBri = MainFormModel.OldSliderBri;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedObject)
                {
                    case Light light:
                        light.state.bri = MainFormModel.SliderBri;
                        break;
                    case Group group:
                        group.action.bri = MainFormModel.SliderBri;
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
                    switch (SelectedObject)
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
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.ct = MainFormModel.SliderCt;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderCt = MainFormModel.OldSliderCt;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedObject)
                {
                    case Light light:
                        light.state.ct = MainFormModel.SliderCt;
                        break;
                    case Group group:
                        group.action.ct = MainFormModel.SliderCt;
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
                    switch (SelectedObject)
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
            }
        }

        private async Task SetSatSliderNow()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.sat = MainFormModel.SliderSat;
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderSat = MainFormModel.OldSliderSat;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedObject)
                {
                    case Light light:
                        light.state.sat = MainFormModel.SliderSat;
                        break;
                    case Group group:
                        group.action.sat = MainFormModel.SliderSat;
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
                    switch (SelectedObject)
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
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.xy = new decimal[]
            {
                MainFormModel.SliderX,
                MainFormModel.SliderY
            };
            bp.transitiontime = SliderTt;

            bool result = await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);

            if (!result)
            {
                MainFormModel.SliderX = MainFormModel.OldSliderX;
                MainFormModel.SliderY = MainFormModel.OldSliderY;
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
            else
            {
                switch (SelectedObject)
                {
                    case Light light:
                        light.state.xy[0] = MainFormModel.SliderX;
                        light.state.xy[1] = MainFormModel.SliderY;
                        break;
                    case Group group:
                        group.action.xy[0] = MainFormModel.SliderX;
                        group.action.xy[1] = MainFormModel.SliderY;
                        break;
                }
            }
        }

        private void SetMainFormModel()
        {
            switch (SelectedObject)
            {
                case Light light:
                    MainFormModel.SliderBri = light.state.bri ?? 0;
                    MainFormModel.SliderHue = light.state.hue ?? 0;
                    MainFormModel.SliderSat = light.state.sat ?? 0;
                    MainFormModel.SliderCt = light.state.ct ?? 153;
                    MainFormModel.SliderX = light.state.xy?[0] ?? 0;
                    MainFormModel.SliderY = light.state.xy?[1] ?? 0;
                    MainFormModel.On = light.state.on ?? false;
                    break;
                case Group group:
                    
                    MainFormModel.SliderBri = group.action.bri ?? 0;
                    MainFormModel.SliderHue = group.action.hue ?? 0;
                    MainFormModel.SliderSat = group.action.sat ?? 0;
                    MainFormModel.SliderCt = group.action.ct ?? 153;
                    MainFormModel.SliderX = group.action.xy?[0] ?? 0;
                    MainFormModel.SliderY = group.action.xy?[1] ?? 0;
                    MainFormModel.On = group.action.on ?? false;
                    break;
                case Scene scene:
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
            await fsm.Initialize(SelectedBridge);
            fsm.Show();
        }

        private async Task ViewBulbs()
        {
            Form_BulbsView fbv = new Form_BulbsView() {Owner = Application.Current.MainWindow};
            await fbv.Initialize(SelectedBridge);
            fbv.Show();
        }

        private async Task ViewGroups()
        {
            Form_GroupView fgv = new Form_GroupView() {Owner = Application.Current.MainWindow};
            await fgv.Initialize(SelectedBridge);
            fgv.Show();
        }

        #endregion

        #region CONTEXT_MENU_METHODS

        private async Task EnableStreaming()
        {
            await SelectedBridge.SetEntertrainementGroupStreamStatus(SelectedObject.Id, true);
        }

        private async Task SetPowerMode()
        {
            Form_PowerFailureSettings fps = new Form_PowerFailureSettings()
            {
                Owner = Application.Current.MainWindow
            };
            await fps.Initialize(SelectedBridge);
            fps.ShowDialog();
        }

        private async Task ReplaceCurrentState()
        {
            if (MessageBox.Show(GlobalStrings.Scene_Replace_Current_States, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            log.Info($@"Replacing scene {SelectedObject.name} lights state with current one.");
            await SelectedBridge.StoreCurrentLightStateAsyncTask(SelectedObject.Id);
        }

        private async Task EditObject()
        {
            log.Debug("Editing object : " + SelectedObject);

            switch (SelectedObject)
            {
                case Group group:
                {
                    Form_GroupCreator fgc = new Form_GroupCreator() {Owner = Application.Current.MainWindow};
                    await fgc.Initialize(SelectedBridge,group);
                    if (fgc.ShowDialog() == true)
                    {
                        await RefreshCurrentObject();
                    }

                    break;
                }
                case Schedule schedule:
                {
                    Form_ScheduleCreator2 fsc = new Form_ScheduleCreator2() {Owner = Application.Current.MainWindow};
                    await fsc.Initialize(SelectedBridge);
                    fsc.EditSchedule(schedule);
                    if (fsc.ShowDialog() == true)
                    {
                        await RefreshCurrentObject();
                    }

                    break;
                }
                case Sensor obj:
                    switch (obj.modelid)
                    {
                        case "PHDL00":
                            Sensor cr = await SelectedBridge.GetObjectAsync<Sensor>(obj.Id);
                            if (cr != null)
                            {
                                cr.Id = obj.Id;
                                Form_Daylight dl = new Form_Daylight(SelectedBridge,cr) {Owner = Application.Current.MainWindow};
                                if (dl.ShowDialog() == true)
                                {
                                    await RefreshCurrentObject();
                                }
                            }

                            break;
                        case "ZGPSWITCH":
                            Form_HueTapConfig htc = new Form_HueTapConfig()
                            {
                                Owner = Application.Current.MainWindow
                            };
                            await htc.Initialize(SelectedBridge,obj.Id);
                            if (htc.ShowDialog() == true)
                            {
                                await RefreshCurrentObject();
                            }

                            break;
                        default:
                            Sensor crs = await SelectedBridge.GetObjectAsync<Sensor>(obj.Id);
                            if (crs != null)
                            {
                                Form_SensorCreator fsc = new Form_SensorCreator(SelectedBridge,crs)
                                {
                                    Owner = Application.Current.MainWindow
                                };
                                if (fsc.ShowDialog() == true)
                                {
                                    await RefreshCurrentObject();
                                }
                            }

                            break;
                    }

                    break;
                case Rule _:
                {
                    Form_RuleCreator frc = new Form_RuleCreator((Rule) SelectedObject) {Owner = Application.Current.MainWindow};
                    await frc.Initialize(SelectedBridge);
                    if (frc.ShowDialog() == true)
                    {
                        await RefreshCurrentObject();
                    }

                    break;
                }
                case Scene _:
                {
                    Form_SceneCreator fscc = new Form_SceneCreator() {Owner = Application.Current.MainWindow};
                    await fscc.Inititalize(SelectedBridge,SelectedObject.Id);
                    if (fscc.ShowDialog() == true)
                    {
                        await RefreshCurrentObject();
                    }

                    break;
                }
                case Resourcelink _:
                {
                    Form_ResourceLinksCreator frlc = new Form_ResourceLinksCreator() {Owner = Application.Current.MainWindow};
                    await frlc.Initialize(SelectedBridge,(Resourcelink) SelectedObject);
                    if (frlc.ShowDialog() == true)
                    {
                        await RefreshCurrentObject();
                    }

                    break;
                }
            }
        }

        private async Task Identify(string type)
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.alert = type;
            await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);
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
                    IHueObject oldobj = (IHueObject) SelectedObject.Clone();

                    SelectedObject = CurrentBridgeHueObjectsList.Single(x => x.Equals(oldobj));
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
            int currentstatus = ((ClipGenericStatusSensorState) ((Sensor) SelectedObject).state).status.GetValueOrDefault();
            log.Info($"Trying to set the sensor {SelectedObject.Id} status to {SensorStatus}");
            bool result = await SelectedBridge.ChangeSensorStateAsyncTask(SelectedObject.Id, new ClipGenericStatusSensorState()
            {
                status = SensorStatus
            });
            if (result)
            {
                ((ClipGenericStatusSensorState) ((Sensor) SelectedObject).state).status = SensorStatus;
            }
            else
            {
                SensorStatus = currentstatus;
            }
        }

        private async Task SetSensorFlag()
        {
            bool currentflag = ((ClipGenericFlagSensorState) ((Sensor) SelectedObject).state).flag;
            log.Info($"Trying to set the sensor {SelectedObject.Id} flag to {SensorFlag}");
            bool result = await SelectedBridge.ChangeSensorStateAsyncTask(SelectedObject.Id, new ClipGenericFlagSensorState()
            {
                flag = SensorFlag
            });
            if (result)
            {
                ((ClipGenericFlagSensorState) ((Sensor) SelectedObject).state).flag = SensorFlag;
            }
            else
            {
                SensorFlag = currentflag;
            }
        }

        private async Task<bool> Duplicate()
        {
            bool result = await SelectedBridge.CreateObjectAsyncTask(SelectedObject);

            if (result)
            {
                string id = SelectedBridge.LastCommandMessages.LastSuccess.value;

                log.Info("Object cloned succesfully !");
                IHueObject hr = await SelectedBridge.GetObjectAsync(id, SelectedObject.GetType());

                if (hr != null)
                {
                    CurrentBridgeHueObjectsList.Add(hr);
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
            await SelectedBridge.ChangeSensorConfigAsyncTask(SelectedObject.Id, new HueMotionSensorConfig() {sensitivity = sensitivity});
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
                CurrentBridgeHueObjectsList.Remove(SelectedObject);
                SelectedObject = null;
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(SelectedBridge);
            }
        }

        private void RenameObject()
        {
            Form_RenameObject fro = new Form_RenameObject(SelectedBridge, SelectedObject) {Owner = Application.Current.MainWindow};
            if (fro.ShowDialog() != true) return;
            SelectedObject.name = fro.GetNewName();            
            log.Info($"Renamed object ID : {SelectedObject.Id} renamed.");
        }

        private void CopyToJson(bool raw)
        {
            Lastmessage = "Object copied to clipboard.";
            Clipboard.SetText(JsonConvert.SerializeObject(SelectedObject, raw ? Formatting.None : Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }

        private async Task OnDim(byte dimval)
        {
            if (SelectedObject is Light)
                await SelectedBridge.SetStateAsyncTask(new State() {bri = dimval, on = true}, SelectedObject.Id);
            else
                await SelectedBridge.SetStateAsyncTask(new Action() {bri = dimval, on = true}, SelectedObject.Id);
        }

        private async Task Colorloop()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.effect = "colorloop";
            await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);
        }

        private async Task NoEffect()
        {
            IBaseProperties bp = BasePropertiesCreator.CreateBaseProperties(SelectedObject.GetType());
            bp.effect = "none";
            await SelectedBridge.SetStateAsyncTask(bp, SelectedObject.Id);
        }

        #endregion

        #region VIEW_TAB_METHODS

        private void SwitchView(int id)
        {
            CurrentView = id;
        }

        private async Task SortListView()
        {
            await RefreshCurrentListHueObject();
            WinHueSettings.settings.Sort = MainFormModel.Sort;
        }

        private void ShowPropertyGrid()
        {
            Form_PropertyGrid fpg = new Form_PropertyGrid {Owner = Application.Current.MainWindow};
            Binding selectedBinding = new Binding("SelectedObject") {Source = this, Mode = BindingMode.TwoWay};
            BindingOperations.SetBinding(fpg, Form_PropertyGrid.SelectedObjectProperty, selectedBinding);
            fpg.Show();
        }

        #endregion

        #region HELP_TAB_METHODS

        private void OpenWinHueWebsite()
        {
            Process.Start("https://hyrules.github.io/WinHue/");
        }

        private void OpenWinHueSupport()
        {
            Process.Start("https://github.com/Hyrules/WinHue/issues");
        }

        #endregion

        #region APPLICATION_MENU_METHODS

        private async Task OpenSettingsWindow()
        {
            Form_AppSettings settings = new Form_AppSettings {Owner = Application.Current.MainWindow};
            if (settings.ShowDialog() != true) return;
            HueHttpClient.Timeout = WinHueSettings.settings.Timeout;
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
                await RefreshCurrentListHueObject();
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


        private async Task ExportDataStore(object param)
        {
            if (param == null) return;
            string p = param.ToString();

            JsonSerializerSettings jss = new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore};

            SaveFileDialog sfd = new SaveFileDialog
            {
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = @"Text File (*.txt) | *.txt"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string data = string.Empty;
                List <IHueObject> listobject = await SelectedBridge.GetAllObjectsAsync(true);

                Dictionary<string, Dictionary<string, IHueObject>> datastore = new Dictionary<string, Dictionary<string, IHueObject>>();

                switch (p)
                {
                    default:
                    case "Full":                        
                        datastore.Add("lights", listobject.OfType<Light>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        datastore.Add("groups", listobject.OfType<Group>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        datastore.Add("scenes", listobject.OfType<Scene>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        datastore.Add("schedules", listobject.OfType<Schedule>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        datastore.Add("rules", listobject.OfType<Rule>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        datastore.Add("resourcelinks", listobject.OfType<Resourcelink>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        datastore.Add("sensors", listobject.OfType<Sensor>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                    case "Groups":
                        datastore.Add("groups", listobject.OfType<Group>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                    case "Lights":
                        datastore.Add("lights", listobject.OfType<Light>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                    case "Scenes":
                        datastore.Add("scenes", listobject.OfType<Scene>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                    case "Schedules":
                        datastore.Add("schedules", listobject.OfType<Schedule>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                    case "Rules":
                        datastore.Add("rules", listobject.OfType<Rule>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                    case "ResourceLinks":
                        datastore.Add("resourcelinks", listobject.OfType<Resourcelink>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                    case "Sensors":
                        datastore.Add("sensors", listobject.OfType<Sensor>().ToList<IHueObject>().ToDictionary(x => x.Id, x => x));
                        break;
                }

                data = JsonConvert.SerializeObject(datastore, Formatting.Indented, jss);
                
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

        private async Task ClickObject()
        {
            if (SelectedObject != null)
            {
                if (!SelectedBridge.Virtual)
                {
                    IHueObject hr = await SelectedBridge.GetObjectAsync(SelectedObject.Id,SelectedObject.GetType());
                    if (hr == null) return;
                    SelectedObject = hr;
                }

                if (SelectedObject is Sensor sensor)
                {
                    switch (sensor.state)
                    {
                        case ClipGenericFlagSensorState state:
                            SensorFlag = state.flag;
                            break;
                        case ClipGenericStatusSensorState state:
                            SensorStatus = state.status.GetValueOrDefault();
                            break;
                    }
                }
            }

            SetMainFormModel();
        }

        private void DoAppUpdate()
        {
            if (MessageBox.Show(GlobalStrings.UpdateAvailableDownload, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                UpdateManager.Instance.DownloadUpdate();
        }

        private void MinimizeToTray()
        {
            Application.Current.MainWindow.Visibility = Visibility.Hidden;
        }

        private void CreateFloorPlan()
        {
            Form_RoomMapCreator frmp = new Form_RoomMapCreator(CurrentBridgeHueObjectsList.ToList())
            {
                Owner = Application.Current.MainWindow
            };
            frmp.ShowDialog();
            LoadFloorPlans();
        }

        private async Task SelectHueElement()
        {
            if (SelectedHueElement == null) return;
            SelectedObject = CurrentBridgeHueObjectsList.FirstOrDefault(x => x.Id == SelectedHueElement.Id && x.GetType() == GetTypeFromHueElementEnum(SelectedHueElement.HueType));
            await ClickObject();
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
            foreach (Tuple<string, string> t in WinHueSettings.bridges.BridgeInfo[SelectedBridge.Mac].hiddenobjects)
            {
                if (SelectedFloorPlan.Elements.Any(x => x.Id == t.Item1 && x.HueType == GetHueElementTypeFromString(t.Item2)))
                {
                    SelectedFloorPlan.Elements.Remove(SelectedFloorPlan.Elements.FirstOrDefault(x => x.Id == t.Item1 && x.HueType == GetHueElementTypeFromString(t.Item2)));
                }
            }

            foreach (HueElement h in SelectedFloorPlan.Elements)
            {
                IHueObject obj = CurrentBridgeHueObjectsList.FirstOrDefault(x => x.Id == h.Id && x.GetType() == GetTypeFromHueElementEnum(h.HueType));
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

        private void CreateMqttConditions()
        {
            Form_MqttClient fmqtt = new Form_MqttClient
            {
                Owner = Application.Current.MainWindow,
            };
            fmqtt.ShowDialog();
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