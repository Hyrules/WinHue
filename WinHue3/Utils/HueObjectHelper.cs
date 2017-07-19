using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using HueLib2;
using HueLib2.BridgeMessages;
using HueLib2.Objects.HueObject;
using log4net;
using WinHue3.SupportedLights;
using WinHue3.Utils;
using Action = HueLib2.Action;

namespace WinHue3
{
    public static class HueObjectHelper
    {
        /// <summary>
        /// Logging 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// CTOR
        /// </summary>
        static HueObjectHelper()
        {
            LightImageLibrary.LoadLightsImages();
        }

        /// <summary>
        /// Get a list of light with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the lights from.</param>
        /// <returns>A List fo lights.</returns>
        public static List<Light> GetBridgeLights(Bridge bridge)
        {
            log.Debug($@"Getting all lights from bridge : {bridge.IpAddress}");
            CommandResult<Dictionary<string,Light>> bresult = bridge?.GetListObjects<Light>();
            if (bresult == null) return null;
            log.Debug("List lights : " + Serializer.SerializeToJson(bresult.Data));
            return ProcessLights(bresult.Data);
        }

        /// <summary>
        /// GEt the list of newly discovered lights
        /// </summary>
        /// <param name="bridge">Bridge to get the new lights from.</param>
        /// <returns>A list of lights.</returns>
        public static List<IHueObject> GetBridgeNewLights(Bridge bridge)
        {
            log.Debug($@"Getting new lights from bridge {bridge.IpAddress}");
            CommandResult<SearchResult> bresult = bridge?.GetNewObjects<Light>();
            if (!bresult.Success) return null;
            log.Debug("Search Result : " + Serializer.SerializeToJson(bresult.Data));
            return ProcessSearchResult(bridge, bresult.Data, true);
        }

        /// <summary>
        /// Process the list of lights
        /// </summary>
        /// <param name="listlights">List of lights to process.</param>
        /// <returns>A list of processed lights.</returns>
        private static List<Light> ProcessLights(Dictionary<string, Light> listlights)
        {
            List<Light> newlist = new List<Light>();

            foreach (KeyValuePair<string, Light> kvp in listlights)
            {
                kvp.Value.Id = kvp.Key;

                kvp.Value.Image = GetImageForLight(kvp.Value.state.reachable.GetValueOrDefault() ? kvp.Value.state.on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off : LightImageState.Unr, kvp.Value.modelid);

                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Process the search results.
        /// </summary>
        /// <param name="bridge">Bridge to process the search results from</param>
        /// <param name="results">Search result to process</param>
        /// <param name="type">Type of result to process. Lights or Sensors</param>
        /// <returns>A list of objects.</returns>
        private static List<IHueObject> ProcessSearchResult(Bridge bridge, SearchResult results,bool type)
        {
            List<IHueObject> newlist = new List<IHueObject>();
            if (type) // lights
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    CommandResult<Light> bresult = bridge.GetObject<Light>(kvp.Key);
                    if (!bresult.Success) continue;
                    Light newlight = bresult.Data;
                    newlight.Id = kvp.Key;
                    newlight.Image = GetImageForLight(newlight.state.reachable.GetValueOrDefault() ? newlight.state.on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off : LightImageState.Unr, newlight.modelid);
                    newlist.Add(newlight);
                }
            }
            else // sensors
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    CommandResult<Sensor> bresult = bridge.GetObject<Sensor>(kvp.Key);
                    if (!bresult.Success) continue;
                    Sensor newSensor = bresult.Data;
                    if (newSensor == null) continue;
                    newSensor.Id = kvp.Key;
                    switch (newSensor.type)
                    {
                        case "ZGPSwitch":
                            log.Debug("New sensor is Hue Tap.");
                            newSensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.huetap);
                            break;
                        case "ZLLSwitch":
                            log.Debug("New sensor is dimmer.");
                            newSensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.dimmer);
                            break;
                        default:
                            log.Debug("New sensor is generic.");
                            newSensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.sensor);
                            break;

                    }
                    newlist.Add(newSensor);
                }
            }

            return newlist;
        }

        /// <summary>
        /// Get a list of group with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the groups from.</param>
        /// <returns>A List of Group.</returns>
        public static List<Group> GetBridgeGroups(Bridge bridge)
        {
            log.Debug($@"Getting all groups from bridge {bridge.IpAddress}");
            CommandResult<Dictionary<string,Group>> bresult = bridge.GetListObjects<Group>();
            if (!bresult.Success) return null;
            Dictionary<string, Group> gs = bresult.Data;
            List<Group> hr = ProcessGroups(gs);
            Group zero = GetGroupZero(bridge);
            if (zero != null)
            {
                gs.Add("0",zero);
            }
            log.Debug("List groups : " + Serializer.SerializeToJson(hr));   
            return hr;
        }

        /// <summary>
        /// Process groups
        /// </summary>
        /// <param name="listgroups">List of group t</param>
        /// <returns>A list of processed group with image and id.</returns>
        private static List<Group> ProcessGroups(Dictionary<string, Group> listgroups)
        {
            List<Group> newlist = new List<Group>();
            foreach (KeyValuePair<string, Group> kvp in listgroups)
            {
                log.Debug("Processing group : " + kvp.Value);
                kvp.Value.Id = kvp.Key;
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(kvp.Value.action.on.GetValueOrDefault() ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a list of scenes with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the scenes from.</param>
        /// <returns>A List of scenes.</returns>
        public static List<Scene> GetBridgeScenes(Bridge bridge)
        {
            log.Debug($@"Getting all scenes from bridge {bridge.IpAddress}");
            CommandResult<Dictionary<string,Scene>> bresult = bridge.GetListObjects<Scene>();
            if (!bresult.Success) return null;
            List<Scene> hr = ProcessScenes(bresult.Data);
            log.Debug("List Scenes : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Process a list of scenes.
        /// </summary>
        /// <param name="listscenes">List of scenes to process.</param>
        /// <returns>A list of processed scenes.</returns>
        private static List<Scene> ProcessScenes(Dictionary<string, Scene> listscenes)
        {
            List<Scene> newlist = new List<Scene>();

            foreach (KeyValuePair<string, Scene> kvp in listscenes)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing scene : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.SceneLarge);
                if (kvp.Value.Name.Contains("HIDDEN") && !WinHueSettings.settings.ShowHiddenScenes) continue;
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a list of schedules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the schedules from.</param>
        /// <returns>A List of schedules.</returns>
        public static List<Schedule> GetBridgeSchedules(Bridge bridge)
        {
            log.Debug($@"Getting all schedules from bridge {bridge.IpAddress}");
            CommandResult<Dictionary<string,Schedule>> bresult = bridge.GetListObjects<Schedule>();
            if (!bresult.Success) return null;
            List<Schedule> hr = ProcessSchedules(bresult.Data);
            log.Debug("List Schedules : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Process a list of schedules
        /// </summary>
        /// <param name="listschedules">List of schedules to process.</param>
        /// <returns>A list of processed schedules.</returns>
        public static List<Schedule> ProcessSchedules(Dictionary<string, Schedule> listschedules)
        {
            List<Schedule> newlist = new List<Schedule>();

            foreach (KeyValuePair<string, Schedule> kvp in listschedules)
            {
                log.Debug("Assigning id to schedule ");
                kvp.Value.Id = kvp.Key;
                ImageSource imgsource;
                log.Debug("Processing schedule : " + kvp.Value);
                string Time = string.Empty;
                if (kvp.Value.localtime == null)
                {
                    log.Debug("LocalTime does not exists try to use Time instead.");
                    if (kvp.Value.time == null) continue;
                    Time = kvp.Value.time;
                }
                else
                {
                    log.Debug("Using LocalTime as schedule time.");
                    Time = kvp.Value.time;
                }

                if (Time.Contains("PT"))
                {
                    log.Debug("Schedule is type Timer.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.timer_clock);
                }
                else if (Time.Contains('W'))
                {
                    log.Debug("Schedule is type Alarm.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.stock_alarm);
                }
                else if (Time.Contains('T'))
                {
                    log.Debug("Schedule is type Schedule.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.SchedulesLarge);
                }
                else
                {
                    log.Debug("Schedule is unknown type.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.schedules);
                }

                kvp.Value.Image = imgsource;
                newlist.Add(kvp.Value);
            }

            return newlist;
        }


        /// <summary>
        /// Get a list of rules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the rules from.</param>
        /// <returns>A List of rules.</returns>
        public static List<Rule> GetBridgeRules(Bridge bridge)
        {
            log.Debug($@"Getting all rules from bridge {bridge.IpAddress}");
            CommandResult<Dictionary<string,Rule>> bresult = bridge.GetListObjects<Rule>();
            if (!bresult.Success) return null;
            List<Rule> hr = ProcessRules(bresult.Data);
            log.Debug("List Rules : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Process a list of rules.
        /// </summary>
        /// <param name="listrules">List of rules to process.</param>
        /// <returns>A processed list of rules.</returns>
        private static List<Rule> ProcessRules(Dictionary<string, Rule> listrules)
        {
            List<Rule> newlist = new List<Rule>();

            foreach (KeyValuePair<string, Rule> kvp in listrules)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing rule : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a list of sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static List<Sensor> GetBridgeSensors(Bridge bridge)
        {
            log.Debug($@"Getting all sensors from bridge {bridge.IpAddress}");
            CommandResult<Dictionary<string,Sensor>> bresult = bridge.GetListObjects<Sensor>();
            if (!bresult.Success) return null;
            List<Sensor> hr = ProcessSensors(bresult.Data);
            log.Debug("List Sensors : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of new sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static List<IHueObject> GetBridgeNewSensors(Bridge bridge)
        {
            log.Debug($@"Getting new sensors from bridge : {bridge.IpAddress}");
            CommandResult<SearchResult> bresult = bridge.GetNewObjects<Sensor>();
            List<IHueObject> hr = ProcessSearchResult(bridge, bresult.Data, false);
            log.Debug("Search Result : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Process a list of sensors
        /// </summary>
        /// <param name="listsensors">List of sensors to process.</param>
        /// <returns>A list of processed sensors.</returns>
        private static List<Sensor> ProcessSensors(Dictionary<string, Sensor> listsensors)
        {
            List<Sensor> newlist = new List<Sensor>();

            foreach (KeyValuePair<string, Sensor> kvp in listsensors)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing Sensor : " + kvp.Value);
                switch (kvp.Value.type)
                {
                    case "ZGPSwitch":
                        log.Debug("Sensor is Hue Tap.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.huetap);
                        break;
                    case "ZLLSwitch":
                        log.Debug("Sensor is dimmer.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.dimmer);
                        break;
                    case "ZLLPresence":
                        log.Debug("Sensor is Motion.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.Motion);
                        break;
                    default:
                        log.Debug("Sensor is generic sensor.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.sensor);
                        break;

                }
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get All Objects from the bridge with ID, name and image populated.
        /// </summary>
        /// <param name="bridge">Bridge to get the datastore from.</param>
        /// <returns>A List of objects.</returns>
        public static List<IHueObject> GetBridgeDataStore(Bridge bridge)
        {
            log.Info($@"Fetching DataStore from bridge : {bridge.IpAddress}");
            CommandResult<DataStore> bresult = bridge.GetBridgeDataStore();
            List<IHueObject> hr = null;
            if (bresult.Success)
            {
                DataStore ds = bresult.Data;
                Group zero = GetGroupZero(bridge);
                if (zero != null)
                {
                    ds.groups.Add("0",zero);
                }

                hr = ProcessDataStore(ds);
                log.Debug("Bridge data store : " + Serializer.SerializeToJson(hr));
            }
            
            return hr;
        }

        private static Group GetGroupZero(Bridge bridge)
        {
            
            CommandResult<Group> gr = bridge.GetObject<Group>("0");
            if (gr.Success)
            {
                return gr.Data;
            }
            return null;
        }


        /// <summary>
        /// Process the data from the bridge datastore.
        /// </summary>
        /// <param name="datastore">Datastore to process.</param>
        /// <returns>A list of object processed.</returns>
        private static List<IHueObject> ProcessDataStore(DataStore datastore)
        {
            List<IHueObject> newlist = new List<IHueObject>();
            log.Debug("Processing datastore...");
            newlist.AddRange(ProcessLights(datastore.lights));
            newlist.AddRange(ProcessGroups(datastore.groups));
            newlist.AddRange(ProcessSchedules(datastore.schedules));
            newlist.AddRange(ProcessScenes(datastore.scenes));
            newlist.AddRange(ProcessSensors(datastore.sensors));
            newlist.AddRange(ProcessRules(datastore.rules));
            newlist.AddRange(ProcessRessourceLinks(datastore.resourcelinks));
            log.Debug("Processing complete.");
            return newlist;
        }

        private static List<Resourcelink> ProcessRessourceLinks(Dictionary<string, Resourcelink> listrl)
        {
            if(listrl == null) return new List<Resourcelink>();
            List<Resourcelink> newlist = new List<Resourcelink>();

            foreach (KeyValuePair<string, Resourcelink> kvp in listrl)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing resource links : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.resource);
                newlist.Add(kvp.Value);
            }
            return newlist;
        }

        /// <summary>
        /// Get the mac address of the bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Returns the mac address of the bridge.</returns>
        public static string GetBridgeMac(Bridge bridge)
        {
            CommandResult<BridgeSettings> bresult = bridge.GetBridgeSettings();           
            if (!bresult.Success) return null;
            string hr = string.Empty;
            log.Debug("Fetching bridge mac : " + bresult.Data.mac);
            return bresult.Data.mac;
        }

        /// <summary>
        /// Check if api key is authorized withthe bridge is authorized.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Check if the bridge is authorized.</returns>
        public static bool IsAuthorized(Bridge bridge)
        {
            CommandResult<BridgeSettings> bresult = bridge.GetBridgeSettings();
            if (!bresult.Success) return false;
            log.Debug("Checking if bridge is authorized : " + Serializer.SerializeToJson(bresult.Data.portalservices));
            return bresult.Data.portalservices != null;
        }

        /// <summary>
        /// Get the Bridge Settings
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>The bridge settings</returns>
        public static BridgeSettings GetBridgeSettings(Bridge bridge)
        {
            CommandResult<BridgeSettings> bresult = bridge.GetBridgeSettings();
            if (!bresult.Success) return null;
            log.Debug("Getting bridge settings : " + Serializer.SerializeToJson(bresult.Data));
            return bresult.Data;
        }


        /// <summary>
        /// Toggle the state of an object on and off (Light or group)
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <param name="obj">Object to toggle.</param>
        /// <param name="tt">Transition Time (Optional)</param>
        /// <returns>The new image of the object.</returns>
        public static ImageSource ToggleObjectOnOffState(Bridge bridge, IHueObject obj, ushort? tt = null)
        {
            ImageSource hr = null;
            if (obj is Light)
            {

                CommandResult<Light> bresult = bridge.GetObject<Light>(obj.Id);
                if (bresult.Success)
                {
                    Light currentState = bresult.Data;

                    if (currentState.state.reachable == false)
                    {
                        hr= GetImageForLight(LightImageState.Unr, currentState.modelid);
                    }
                    else
                    {
                        if (currentState.state.on == true)
                        {
                            log.Debug("Toggling light state : OFF");
                            CommandResult<Messages> bsetlightstate = bridge.SetState<Light>(new State { on = false, transitiontime = tt}, obj.Id);

                            if (bsetlightstate.Success)
                            {
                                hr = GetImageForLight(LightImageState.Off, currentState.modelid);
                            }

                        }
                        else
                        {
                            log.Debug("Toggling light state : ON");
                            CommandResult<Messages> bsetlightstate = bridge.SetState<Light>(new State { on = true, transitiontime = tt, bri = WinHueSettings.settings.DefaultBriLight }, obj.Id);

                            if (bsetlightstate.Success)
                            {

                                hr = GetImageForLight(LightImageState.On, currentState.modelid);
                            }

                        }

                    }
                }


            }
            else
            {
                CommandResult<Group> bresult = bridge.GetObject<Group>(obj.Id);

                if (bresult.Success)
                {
                    Group currentstate = bresult.Data;
                    if (currentstate.action.on == true)
                    {
                        log.Debug("Toggling group state : ON");
                        CommandResult<Messages> bsetgroupstate = bridge.SetState<Group>(new Action { on = false, transitiontime = tt}, obj.Id);

                        if (bsetgroupstate.Success)
                        {
                            hr = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOff_Large);
                        }

                    }
                    else
                    {
                        log.Debug("Toggling group state : OFF");
                        CommandResult<Messages> bsetgroupstate = bridge.SetState<Group>(new Action { on = true, transitiontime = tt, bri = WinHueSettings.settings.DefaultBriGroup }, obj.Id);
                        if (bsetgroupstate.Success)
                        {
                            hr = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOn_Large);
                        }

                    }
                }

            }

            return hr;
        }

        /// <summary>
        /// Get a list of users on the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>A list of users</returns>
        public static List<Whitelist> GetBridgeUsers(Bridge bridge)
        {
            CommandResult<Dictionary<string,Whitelist>> bresult = bridge.GetUserList();
            
            if (!bresult.Success) return null;
            
            Dictionary<string, Whitelist> brlisteusers = bresult.Data;
            List<Whitelist> listusers = new List<Whitelist>();
            foreach (KeyValuePair<string, Whitelist> kvp in brlisteusers)
            {
                log.Debug($"Parsing user ID {kvp.Key}, {kvp.Value}");
                kvp.Value.id = kvp.Key;
                listusers.Add(kvp.Value);
            }

            return listusers;
        }

        /// <summary>
        /// List of possible light state.
        /// </summary>
        public enum LightImageState { On = 0, Off = 1, Unr = 3 }

        /// <summary>
        /// Return the new image from the light
        /// </summary>
        /// <param name="imagestate">Requested state of the light.</param>
        /// <param name="modelid">model id of the light.</param>
        /// <returns>New image of the light</returns>
        public static ImageSource GetImageForLight(LightImageState imagestate, string modelid = null)
        {
            string modelID = modelid ?? "default";
            string state = string.Empty;

            switch (imagestate)
            {
                case LightImageState.Off:
                    state = "off";
                    break;
                case LightImageState.On:
                    state = "on";
                    break;
                case LightImageState.Unr:
                    state = "unr";
                    break;
                default:
                    state = "off";
                    break;
            }

            if (modelID == string.Empty)
            {
                log.Debug("STATE : " + state + " empty MODELID using default images");
                return LightImageLibrary.Images["Default"][state];
            }

            ImageSource newImage;

            if (LightImageLibrary.Images.ContainsKey(modelID))
            {
                log.Debug("STATE : " + state + " MODELID : " + modelID);
                newImage = LightImageLibrary.Images[modelID][state];

            }
            else
            {
                log.Debug("STATE : " + state + " unknown MODELID : " + modelID + " using default images.");
                newImage = LightImageLibrary.Images["Default"][state];
            }
            return newImage;
        }

        public static List<T> GetObjectsList<T>(Bridge bridge) where T : IHueObject
        {           
            
            List<T> hr = new List<T>();

            if (typeof(T) == typeof(Light))
            {
                CommandResult<Dictionary<string, Light>> res = bridge.GetListObjects<Light>();
                if (res.Success)
                    hr = ProcessLights(res.Data) as List<T>;
            }
            else if(typeof(T) == typeof(Group))
            {
                CommandResult<Dictionary<string, Group>> res = bridge.GetListObjects<Group>();
                if (res.Success)
                    hr = ProcessGroups(res.Data) as List<T>;
            }
            else if (typeof(T) == typeof(Scene))
            {
                CommandResult<Dictionary<string, Scene>> res = bridge.GetListObjects<Scene>();
                if(res.Success)
                    hr = ProcessScenes(res.Data) as List<T>;
            }
            else if(typeof(T) == typeof(Schedule))
            {
                CommandResult<Dictionary<string, Schedule>> res = bridge.GetListObjects<Schedule>();
                if (res.Success)
                    hr = ProcessSchedules(res.Data) as List<T>;
            }
            else if (typeof(T) == typeof(Sensor))
            {
                CommandResult<Dictionary<string, Sensor>> res = bridge.GetListObjects<Sensor>();
                if (res.Success)
                    hr = ProcessSensors(res.Data) as List<T>;

            }
            else if (typeof(T) == typeof(Resourcelink))
            {
                CommandResult<Dictionary<string, Resourcelink>> res = bridge.GetListObjects<Resourcelink>();
                if (res.Success)
                    hr = ProcessRessourceLinks(res.Data) as List<T>;
            }

            return hr;
        }

        public static T GetObject<T>(Bridge bridge, string id) where T : IHueObject
        {
            CommandResult<T> bresult = bridge.GetObject<T>(id);
            T Object = bresult.Data;
            if (bresult.Success)
            {
                if (typeof(T) == typeof(Light))
                {
                    Light light = Object as Light;

                    log.Debug("Light : " + Object);
                    light.Id = id;
                    light.Image =
                        GetImageForLight(
                            light.state.reachable.GetValueOrDefault()
                                ? light.state.on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off
                                : LightImageState.Unr, light.modelid);
                    Object = (T) Convert.ChangeType(light,typeof(T));
                }
                else if (typeof(T) == typeof(Group))
                {
                    Group group = Object as Group;
                    log.Debug("Group : " + group);
                    group.Id = id;
                    group.Image = GDIManager.CreateImageSourceFromImage(group.action.on.GetValueOrDefault() ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
                    Object = (T)Convert.ChangeType(group, typeof(T));
                }
                else if (typeof(T) == typeof(Sensor) || typeof(T).BaseType == typeof(Sensor))
                {
                    Sensor sensor = bresult.Data as Sensor;
                    sensor.Id = id;
                    sensor.Image = GDIManager.CreateImageSourceFromImage(sensor.type == "ZGPSwitch" ? Properties.Resources.huetap : Properties.Resources.sensor);
                    Object = (T)Convert.ChangeType(sensor, typeof(T));
                }
                else if (typeof(T) == typeof(Rule))
                {
                    Rule rule = Object as Rule;
                    log.Debug("Rule : " + rule);
                    rule.Id = id;
                    rule.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);
                    Object = (T)Convert.ChangeType(rule, typeof(T));
                }
                else if (typeof(T) == typeof(Scene))
                {
                    Scene scene = Object as Scene;
                    log.Debug("Scene : " + scene);
                    scene.Id = id;
                    scene.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.SceneLarge);
                    Object = (T)Convert.ChangeType(scene, typeof(T));
                }
                else if (typeof(T) == typeof(Schedule))
                {
                    Schedule schedule = Object as Schedule;
                    schedule.Id = id;
                    ImageSource imgsource;
                    if (schedule.localtime.Contains("PT"))
                    {
                        log.Debug("Schedule is type Timer.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.timer_clock);
                    }
                    else if (schedule.localtime.Contains('W'))
                    {
                        log.Debug("Schedule is type Alarm.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.stock_alarm);
                    }
                    else if (schedule.localtime.Contains('T'))
                    {
                        log.Debug("Schedule is type Schedule.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.SchedulesLarge);
                    }
                    else
                    {
                        log.Debug("Schedule is unknown type.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.schedules);
                    }

                    schedule.Image = imgsource;
                    Object = (T)Convert.ChangeType(schedule, typeof(T));
                }
                else if (typeof(T) == typeof(Resourcelink))
                {
                    Resourcelink rl = Object as Resourcelink;
                    rl.Id = id;
                    rl.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.resource);
                    Object = (T)Convert.ChangeType(rl, typeof(T));
                }
            }

            return Object;
        }

        public static Type GetTypeFromName(string huetype)
        {
            Assembly asm = typeof(Light).Assembly;
            return Type.GetType($"HueLib2.{huetype.TrimEnd('s').CapitalizeFirstLetter()}, {asm}");
        }
    }

}