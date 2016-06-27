using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HueLib;
using HueLib_base;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace WinHue3
{
    public static class HueObjectHelper
    {
        /// <summary>
        /// Logging 
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// CTOR
        /// </summary>
        static HueObjectHelper()
        {
            ManagedLights.LoadSupportedLights();
        }

        /// <summary>
        /// Get a list of light with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the lights from.</param>
        /// <returns>A List fo lights.</returns>
        public static List<HueObject> GetBridgeLights(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting all lights from bridge : {bridge.IpAddress}");
            Dictionary<string, Light> listlights = bridge.GetLightList();
            log.Debug("List lights : " + listlights);
            return ProcessLights(listlights);
        }

        /// <summary>
        /// GEt the list of newly discovered lights
        /// </summary>
        /// <param name="bridge">Bridge to get the new lights from.</param>
        /// <returns>A list of lights.</returns>
        public static List<HueObject> GetBridgeNewLights(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting new lights from bridge {bridge.IpAddress}");
            SearchResult sr = bridge.GetNewLights();
            log.Debug("Search Result : " + sr);
            return bridge == null ? new List<HueObject>() : ProcessSearchResult(bridge, sr, true);
        }

        /// <summary>
        /// Process the list of lights
        /// </summary>
        /// <param name="listlights">List of lights to process.</param>
        /// <returns>A list of processed lights.</returns>
        private static List<HueObject> ProcessLights(Dictionary<string, Light> listlights)
        {
            List<HueObject> newlist = new List<HueObject>();
            if (listlights == null) return newlist;

            foreach (KeyValuePair<string, Light> kvp in listlights)
            {
                kvp.Value.Id = kvp.Key;

                kvp.Value.Image = GetImageForLight((bool)kvp.Value.state.reachable ? (bool)kvp.Value.state.on ? LightImageState.On : LightImageState.Off : LightImageState.Unr, kvp.Value.modelid);

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
        private static List<HueObject> ProcessSearchResult(Bridge bridge, SearchResult results, bool type)
        {
            if (results == null) return new List<HueObject>();
            List<HueObject> newlist = new List<HueObject>();
            if (type) // lights
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    Light newlight = bridge.GetLight(kvp.Key);
                    if (newlight == null) continue;
                    newlight.Id = kvp.Key;

                    if (bridge.lastMessages.SuccessCount == 1)
                    {
                        newlight.Image = GetImageForLight((bool)newlight.state.reachable ? (bool)newlight.state.on ? LightImageState.On : LightImageState.Off : LightImageState.Unr, newlight.modelid);
                    }

                    newlist.Add(newlight);
                }
            }
            else // sensors
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    Sensor newSensor = bridge.GetSensor(kvp.Key);
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
        /// Get a light with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the light from.</param>
        /// <param name="id">Id of the light on the bridge.</param>
        /// <returns></returns>
        public static HueObject GetBridgeLight(Bridge bridge, string id)
        {
            if (bridge == null) return null;
            log.Debug($@"Getting light ID : {id} from Bridge : {bridge.IpAddress}");
            Light light = bridge.GetLight(id);
            log.Debug("Light : " + light);
            light.Id = id;

            light.Image = GetImageForLight((bool)light.state.reachable ? (bool)light.state.on ? LightImageState.On : LightImageState.Off : LightImageState.Unr, light.modelid);
            
            return light;
        }

        /// <summary>
        /// Get a list of group with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the groups from.</param>
        /// <returns>A List of Group.</returns>
        public static List<HueObject> GetBridgeGroups(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting all groups from bridge {bridge.IpAddress}");
            Dictionary<string, Group> listgroups = bridge.GetGroupList();
            log.Debug("List groups : " + listgroups);
            return bridge == null ? new List<HueObject>() : ProcessGroups(listgroups);
        }

        /// <summary>
        /// Process groups
        /// </summary>
        /// <param name="listgroups">List of group t</param>
        /// <returns>A list of processed group with image and id.</returns>
        private static List<HueObject> ProcessGroups(Dictionary<string, Group> listgroups)
        {
            List<HueObject> newlist = new List<HueObject>();
            foreach (KeyValuePair<string, Group> kvp in listgroups)
            {
                log.Debug("Processing group : " + kvp.Value);
                kvp.Value.Id = kvp.Key;
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage((bool)kvp.Value.action.on ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a group with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the group from.</param>
        /// <param name="id">Id of the group on the bridge.</param>
        /// <returns></returns>
        public static HueObject GetBridgeGroup(Bridge bridge, string id)
        {
            if (bridge == null) return null;
            log.Debug($@"Getting group ID : {id} from bridge : {bridge.IpAddress}");
            Group group = bridge.GetGroup(id);
            log.Debug("Group : " + group);
            group.Id = id;
            group.Image = GDIManager.CreateImageSourceFromImage((bool)group.action.on ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
            return group;
        }

        /// <summary>
        /// Get a list of scenes with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the scenes from.</param>
        /// <returns>A List of scenes.</returns>
        public static List<HueObject> GetBridgeScenes(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting all scenes from bridge {bridge.IpAddress}");
            Dictionary<string, Scene> listscenes = bridge.GetScenesList();
            log.Debug("List Scenes : " + listscenes);
            return ProcessScenes(listscenes);
        }

        /// <summary>
        /// Process a list of scenes.
        /// </summary>
        /// <param name="listscenes">List of scenes to process.</param>
        /// <returns>A list of processed scenes.</returns>
        private static List<HueObject> ProcessScenes(Dictionary<string, Scene> listscenes)
        {
            List<HueObject> newlist = new List<HueObject>();
            if (listscenes == null) return newlist;

            foreach (KeyValuePair<string, Scene> kvp in listscenes)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing scene : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.SceneLarge);
                if (kvp.Value.name.Contains("HIDDEN") && !WinHueSettings.settings.ShowHiddenScenes) continue;
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a scene with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the scene from.</param>
        /// <param name="id">Id of the scene on the bridge.</param>
        /// <returns>The requested scene.</returns>
        public static HueObject GetBridgeScene(Bridge bridge, string id)
        {
            if (bridge == null) return null;
            log.Debug($@"Getting scene ID : {id} from bridge : {bridge.IpAddress}");
            Scene scene = bridge.GetScene(id);
            log.Debug("Scene : " + scene);
            scene.Id = id;
            scene.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.SceneLarge);
            return scene;
        }

        /// <summary>
        /// Get a list of schedules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the schedules from.</param>
        /// <returns>A List of schedules.</returns>
        public static List<HueObject> GetBridgeSchedules(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting all schedules from bridge {bridge.IpAddress}");
            Dictionary<string, Schedule> listSchedules = bridge.GetScheduleList();
            log.Debug("List Schedules : " + listSchedules);
            return ProcessSchedules(listSchedules);
        }

        /// <summary>
        /// Process a list of schedules
        /// </summary>
        /// <param name="listschedules">List of schedules to process.</param>
        /// <returns>A list of processed schedules.</returns>
        public static List<HueObject> ProcessSchedules(Dictionary<string, Schedule> listschedules)
        {
            List<HueObject> newlist = new List<HueObject>();
            if (listschedules == null)
            {
                log.Debug("List of schedule is null.");
                return newlist;
            }

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
        /// Get a schedule with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the schedule from.</param>
        /// <param name="id">Id of the schedule on the bridge.</param>
        /// <returns>The requested schedule.</returns>
        public static HueObject GetBridgeSchedule(Bridge bridge, string id)
        {
            if (bridge == null || id == null) return null;
            log.Debug($@"Getting schedule ID : {id} from bridge : {bridge.IpAddress}");
            Schedule schedule = bridge.GetSchedule(id);
            log.Debug("Schedule : " + schedule);
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
            return schedule;
        }

        /// <summary>
        /// Get a list of rules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the rules from.</param>
        /// <returns>A List of rules.</returns>
        public static List<HueObject> GetBridgeRules(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting all rules from bridge {bridge.IpAddress}");
            Dictionary<string, Rule> listRules = bridge.GetRulesList();
            log.Debug("List Rules : " + listRules);
            return ProcessRules(listRules);
        }

        /// <summary>
        /// Process a list of rules.
        /// </summary>
        /// <param name="listrules">List of rules to process.</param>
        /// <returns>A processed list of rules.</returns>
        private static List<HueObject> ProcessRules(Dictionary<string, Rule> listrules)
        {
            List<HueObject> newlist = new List<HueObject>();
            if (listrules == null) return newlist;

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
        /// Get a rule with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the rule from.</param>
        /// <param name="id">Id of the rule on the bridge.</param>
        /// <returns>The Rule</returns>
        public static HueObject GetBridgeRule(Bridge bridge, string id)
        {
            if (bridge == null) return null;
            log.Debug($@"Getting rule ID : {id} from bridge {bridge.IpAddress}");
            Rule rule = bridge.GetRule(id);
            log.Debug("Rule : " + rule);
            rule.Id = id;
            rule.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);
            return rule;
        }

        /// <summary>
        /// Get a list of sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static List<HueObject> GetBridgeSensors(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting all sensors from bridge {bridge.IpAddress}");
            Dictionary<string, Sensor> listSensors = bridge.GetSensorList();
            log.Debug("List Sensors : " + listSensors);
            return ProcessSensors(listSensors);
        }

        /// <summary>
        /// Get a list of new sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static List<HueObject> GetBridgeNewSensors(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Debug($@"Getting new sensors from bridge : {bridge.IpAddress}");
            SearchResult sr = bridge.GetNewSensors();
            log.Debug("Search Results : " + sr);
            return ProcessSearchResult(bridge, sr, false);
        }

        /// <summary>
        /// Process a list of sensors
        /// </summary>
        /// <param name="listsensors">List of sensors to process.</param>
        /// <returns>A list of processed sensors.</returns>
        private static List<HueObject> ProcessSensors(Dictionary<string, Sensor> listsensors)
        {
            List<HueObject> newlist = new List<HueObject>();
            if (listsensors == null) return newlist;

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
        /// Get a sensor with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensor from.</param>
        /// <param name="id">Id of the sensor on the bridge.</param>
        /// <returns>The sensor</returns>
        public static HueObject GetBridgeSensor(Bridge bridge, string id)
        {
            if (bridge == null) return null;
            log.Debug($@"Getting sensor ID : {id} from bridge : {bridge.IpAddress}");
            Sensor sensor = bridge.GetSensor(id);
            log.Debug("Sensor : " + sensor);
            sensor.Id = id;
            sensor.Image = GDIManager.CreateImageSourceFromImage(sensor.type == "ZGPSwitch" ? Properties.Resources.huetap : Properties.Resources.sensor);
            return sensor;
        }

        /// <summary>
        /// Get All Objects from the bridge with ID, name and image populated.
        /// </summary>
        /// <param name="bridge">Bridge to get the datastore from.</param>
        /// <returns>A List of objects.</returns>
        public static List<HueObject> GetBridgeDataStore(Bridge bridge)
        {
            if (bridge == null) return new List<HueObject>();
            log.Info($@"Fetching DataStore from bridge : {bridge.IpAddress}");
            DataStore ds = bridge.GetBridgeDataStore();
            log.Debug("Bridge data store : " + ds);
            return ProcessDataStore(ds);
        }

        /// <summary>
        /// Process the data from the bridge datastore.
        /// </summary>
        /// <param name="datastore">Datastore to process.</param>
        /// <returns>A list of object processed.</returns>
        private static List<HueObject> ProcessDataStore(DataStore datastore)
        {
            List<HueObject> newlist = new List<HueObject>();
            if (datastore == null) return newlist;
            log.Debug("Processing datastore...");
            newlist.AddRange(ProcessLights(datastore.lights));
            newlist.AddRange(ProcessGroups(datastore.groups));
            newlist.AddRange(ProcessSchedules(datastore.schedules));
            newlist.AddRange(ProcessScenes(datastore.scenes));
            newlist.AddRange(ProcessSensors(datastore.sensors));
            newlist.AddRange(ProcessRules(datastore.rules));
            log.Debug("Processing complete.");
            return newlist;
        }


        /// <summary>
        /// Return the object latest values from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the object from.</param>
        /// <param name="obj">Object to get values.</param>
        /// <returns>The latest values of the object.</returns>
        public static HueObject GetBridgeObject(Bridge bridge, HueObject obj)
        {
            if (obj == null) return null;
            log.Debug($@"Fetching object : {obj.Id} of type {obj.GetType()}");
            if (obj is Light)
                return GetBridgeLight(bridge, obj.Id);
            if (obj is Group)
                return GetBridgeGroup(bridge, obj.Id);
            if (obj is Scene)
                return GetBridgeScene(bridge, obj.Id);
            if (obj is Schedule)
                return GetBridgeSchedule(bridge, obj.Id);
            if (obj is Sensor)
                return GetBridgeSensor(bridge, obj.Id);
            if (obj is Rule)
                return GetBridgeRule(bridge, obj.Id);
            return null;

        }

        /// <summary>
        /// Get the mac address of the bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Returns the mac address of the bridge.</returns>
        public static string GetBridgeMac(Bridge bridge)
        {
            BridgeSettings brs = bridge.GetBridgeSettings();
            log.Debug("Fetching bridge mac : " + brs.mac);
            return brs != null ? brs.mac : string.Empty;
        }

        /// <summary>
        /// Check if api key is authorized withthe bridge is authorized.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Check if the bridge is authorized.</returns>
        public static bool IsAuthorized(Bridge bridge)
        {
            BridgeSettings brs = bridge?.GetBridgeSettings();
            log.Debug("Checking if bridge is authorized : " + brs?.portalservices);
            return brs?.portalservices != null;
        }

        /// <summary>
        /// Toggle the state of an object on and off (Light or group)
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <param name="id">ID of the object to toggle.</param>
        /// <returns>The new image of the object.</returns>
        public static ImageSource ToggleObjectOnOffState(Bridge bridge, HueObject obj)
        {
            if (obj == null) return null;
            if (bridge == null) return obj.Image;
            if (!(obj is Light) && !(obj is Group)) return obj.Image;
            if (obj is Light)
            {

                Light currentState = bridge.GetLight(obj.Id);
                if (currentState?.state == null) return obj.Image;
                if (currentState.state.reachable == false) return GetImageForLight(LightImageState.Unr,currentState.modelid);
                if (currentState.state.on == true)
                {
                    log.Debug("Toggling light state : OFF");
                    bridge.SetLightState(obj.Id, new State() { on = false });

                    if (bridge.lastMessages.SuccessCount == 1)
                    {
                        currentState.Image = GetImageForLight(LightImageState.Off, currentState.modelid);
                    }
                    return currentState.Image;
                }
                else
                {
                    log.Debug("Toggling light state : ON");
                    bridge.SetLightState(obj.Id, new State() { on = true });
                    
                    if (bridge.lastMessages.SuccessCount == 1)
                    {
                        currentState.Image = GetImageForLight(LightImageState.On, currentState.modelid);
                    }
                    return currentState.Image;
                }
            }
            else
            {
                Group currentState = bridge.GetGroup(obj.Id);
                if (currentState?.action == null) return obj.Image;
                if (currentState.action.on == true)
                {
                    log.Debug("Toggling group state : ON");
                    bridge.SetGroupAction(obj.Id, new HueLib_base.Action() { on = false });
                    return bridge.lastMessages.SuccessCount == 1 ? GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOff_Large) : obj.Image;
                }
                else
                {
                    log.Debug("Toggling group state : OFF");
                    bridge.SetGroupAction(obj.Id, new HueLib_base.Action() { on = true });
                    return bridge.lastMessages.SuccessCount == 1 ? GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOn_Large) : obj.Image;
                }
            }
        }

        /// <summary>
        /// Get a list of users on the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>A list of users</returns>
        public static List<Whitelist> GetBridgeUsers(Bridge bridge)
        {          
            Dictionary<string, Whitelist> brlisteusers = bridge.GetUserList();
            if(brlisteusers == null) return new List<Whitelist>();
            List<Whitelist> listusers = new List<Whitelist>();
            foreach(KeyValuePair<string,Whitelist> kvp in brlisteusers)
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
        public enum LightImageState { On = 0, Off = 1, Unr = 3};

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

            switch(imagestate)
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
                return ManagedLights.listAvailableLights["default"].img[state];
            }

            ImageSource newImage;

            if (ManagedLights.listAvailableLights.ContainsKey(modelID))
            {
                log.Debug("STATE : " + state + " MODELID : " + modelID);
                newImage = ManagedLights.listAvailableLights[modelID].img[state];
            }
            else
            {
                log.Debug("STATE : " + state + " unknown MODELID : " + modelID + " using default images.");
                newImage = ManagedLights.listAvailableLights["default"].img[state];
            }
            return newImage;
        }


    }
}
