using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using log4net;
using WinHue3.ExtensionMethods;
using WinHue3.Interface;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using WinHue3.Philips_Hue.HueObjects.SensorObject;
using WinHue3.Settings;
using WinHue3.SupportedLights;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.Utils
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
            Dictionary<string,Light> bresult = bridge?.GetListObjects<Light>();
            if (bresult == null) return null;
            log.Debug("List lights : " + Serializer.SerializeToJson(bresult));
            return ProcessLights(bresult);
        }


        /// <summary>
        /// Get a list of light with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the lights from.</param>
        /// <returns>A List fo lights.</returns>
        public static async Task<List<Light>> GetBridgeLightsAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all lights from bridge : {bridge.IpAddress}");
            Dictionary<string, Light> bresult = await bridge?.GetListObjectsAsyncTask<Light>();
            if (bresult == null) return null;
            log.Debug("List lights : " + Serializer.SerializeToJson(bresult));
            return ProcessLights(bresult);
        }

        /// <summary>
        /// GEt the list of newly discovered lights
        /// </summary>
        /// <param name="bridge">Bridge to get the new lights from.</param>
        /// <returns>A list of lights.</returns>
        public static async Task<List<IHueObject>> GetBridgeNewLightsAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting new lights from bridge {bridge.IpAddress}");
            SearchResult bresult = await bridge?.GetNewObjectsAsyncTask<Light>();
            if (bresult == null) return null;
            log.Debug("Search Result : " + bresult.ToString());
            return ProcessSearchResult(bridge, bresult, true);
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
                    Light bresult = bridge.GetObject<Light>(kvp.Key);
                    if (bresult == null) continue;
                    Light newlight = bresult;
                    newlight.Id = kvp.Key;
                    newlight.Image = GetImageForLight(newlight.state.reachable.GetValueOrDefault() ? newlight.state.on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off : LightImageState.Unr, newlight.modelid);
                    newlist.Add(newlight);
                }
            }
            else // sensors
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    ISensor bresult = bridge.GetObject<ISensor>(kvp.Key);
                    if (bresult == null) continue;
                    ISensor newSensor = bresult;
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
        /// Get a list of group with ID, name and image populated from the selected bridge async.
        /// </summary>
        /// <param name="bridge">Bridge to get the groups from.</param>
        /// <returns>A List of Group.</returns>
        public static List<Group> GetBridgeGroups(Bridge bridge)
        {
            log.Debug($@"Getting all groups from bridge {bridge.IpAddress}");
            Dictionary<string, Group> bresult = bridge.GetListObjects<Group>();
            if (bresult == null) return null;
            Dictionary<string, Group> gs = bresult;
            Group zero = GetGroupZero(bridge);
            if (zero != null)
            {
                gs.Add("0", zero);
            }
            List<Group> hr = ProcessGroups(gs);
            log.Debug("List groups : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of group with ID, name and image populated from the selected bridge async.
        /// </summary>
        /// <param name="bridge">Bridge to get the groups from.</param>
        /// <returns>A List of Group.</returns>
        public static async Task<List<Group>> GetBridgeGroupsAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all groups from bridge {bridge.IpAddress}");
            Dictionary<string, Group> bresult = await bridge.GetListObjectsAsyncTask<Group>();
            if (bresult == null) return null;
            Dictionary<string, Group> gs = bresult;
            Group zero = await GetGroupZeroAsynTask(bridge);
            if (zero != null)
            {
                gs.Add("0", zero);
            }
            List<Group> hr = ProcessGroups(gs);
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
            Dictionary<string, Scene> bresult = bridge.GetListObjects<Scene>();
            if (bresult == null) return null;
            List<Scene> hr = ProcessScenes(bresult);
            log.Debug("List Scenes : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of scenes with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the scenes from.</param>
        /// <returns>A List of scenes.</returns>
        public static async Task<List<Scene>> GetBridgeScenesAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all scenes from bridge {bridge.IpAddress}");
            Dictionary<string, Scene> bresult = await bridge.GetListObjectsAsyncTask<Scene>();
            if (bresult == null) return null;
            List<Scene> hr = ProcessScenes(bresult);
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
                if (kvp.Value.name.Contains("HIDDEN") && !WinHueSettings.settings.ShowHiddenScenes) continue;
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
            Dictionary<string, Schedule> bresult =bridge.GetListObjects<Schedule>();
            if (bresult == null) return null;
            List<Schedule> hr = ProcessSchedules(bresult);
            log.Debug("List Schedules : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of schedules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the schedules from.</param>
        /// <returns>A List of schedules.</returns>
        public static async Task<List<Schedule>> GetBridgeSchedulesAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all schedules from bridge {bridge.IpAddress}");
            Dictionary<string, Schedule> bresult = await bridge.GetListObjectsAsyncTask<Schedule>();
            if (bresult == null) return null;
            List<Schedule> hr = ProcessSchedules(bresult);
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
                    if (kvp.Value.localtime == null) continue;
                    Time = kvp.Value.localtime;
                }
                else
                {
                    log.Debug("Using LocalTime as schedule time.");
                    Time = kvp.Value.localtime;
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
            Dictionary<string,Rule> bresult = bridge.GetListObjects<Rule>();
            if (bresult == null) return null;
            List<Rule> hr = ProcessRules(bresult);
            log.Debug("List Rules : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of rules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the rules from.</param>
        /// <returns>A List of rules.</returns>
        public static async Task<List<Rule>> GetBridgeRulesAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all rules from bridge {bridge.IpAddress}");
            Dictionary<string, Rule> bresult = await bridge.GetListObjectsAsyncTask<Rule>();
            if (bresult == null) return null;
            List<Rule> hr = ProcessRules(bresult);
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
        public static List<ISensor> GetBridgeSensors(Bridge bridge)
        {
            log.Debug($@"Getting all sensors from bridge {bridge.IpAddress}");
            Dictionary<string,ISensor> bresult = bridge.GetListObjects<ISensor>();
            if (bresult == null) return null;
            List<ISensor> hr = ProcessSensors(bresult);
            log.Debug("List Sensors : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static async Task<List<ISensor>> GetBridgeSensorsAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all sensors from bridge {bridge.IpAddress}");
            Dictionary<string, ISensor> bresult = await bridge.GetListObjectsAsyncTask<ISensor>();
            if (bresult == null) return null;
            List<ISensor> hr = ProcessSensors(bresult);
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
            SearchResult bresult = bridge.GetNewObjects<ISensor>();
            List<IHueObject> hr = ProcessSearchResult(bridge, bresult, false);
            log.Debug("Search Result : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of new sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static async Task<List<IHueObject>> GetBridgeNewSensorsAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting new sensors from bridge : {bridge.IpAddress}");
            SearchResult bresult = await bridge.GetNewObjectsAsyncTask<ISensor>();
            List<IHueObject> hr = ProcessSearchResult(bridge, bresult, false);
            log.Debug("Search Result : " + Serializer.SerializeToJson(hr));
            return hr;
        }

        /// <summary>
        /// Process a list of sensors
        /// </summary>
        /// <param name="listsensors">List of sensors to process.</param>
        /// <returns>A list of processed sensors.</returns>
        private static List<ISensor> ProcessSensors(Dictionary<string, ISensor> listsensors)
        {
            List<ISensor> newlist = new List<ISensor>();

            foreach (KeyValuePair<string, ISensor> kvp in listsensors)
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
        /// Get the list of resource links from the bridge async.
        /// </summary>
        /// <param name="bridge">Bridge to get the resource links from.</param>
        /// <returns>A list of resource links.</returns>
        public static List<Resourcelink> GetBridgeResourceLinks(Bridge bridge)
        {
            log.Info($@"Fetching Resource links from bridge : {bridge.IpAddress}");
            Dictionary<string, Resourcelink> bresult = bridge.GetListObjects<Resourcelink>();
            if (bresult == null) return null;
            List<Resourcelink> rl =  ProcessRessourceLinks(bresult);
            return rl;
        }

        /// <summary>
        /// Get the list of resource links from the bridge async.
        /// </summary>
        /// <param name="bridge">Bridge to get the resource links from.</param>
        /// <returns>A list of resource links.</returns>
        public static async Task<List<Resourcelink>> GetBridgeResourceLinksAsyncTask(Bridge bridge)
        {
            log.Info($@"Fetching Resource links from bridge : {bridge.IpAddress}");
            Dictionary<string, Resourcelink> bresult = await bridge.GetListObjectsAsyncTask<Resourcelink>();
            if (bresult == null) return null;
            List<Resourcelink> rl = ProcessRessourceLinks(bresult);
            return rl;
        }

        /// <summary>
        /// Get All Objects from the bridge with ID, name and image populated.
        /// </summary>
        /// <param name="bridge">Bridge to get the datastore from.</param>
        /// <returns>A List of objects.</returns>
        public static List<IHueObject> GetBridgeDataStore(Bridge bridge)
        {
            log.Info($@"Fetching DataStore from bridge : {bridge.IpAddress}");
            DataStore bresult = bridge.GetBridgeDataStore();
            List<IHueObject> hr = null;
            if (bresult == null) return hr;
            DataStore ds = bresult;
            Group zero = GetGroupZero(bridge);
            if (zero != null)
            {
                ds.groups.Add("0",zero);
            }

            hr = ProcessDataStore(ds);
            log.Debug("Bridge data store : " + hr);

            return hr;
        }

        /// <summary>
        /// Get the datastore from the bridge async.
        /// </summary>
        /// <param name="bridge">Bridge to get the datastore from.</param>
        /// <returns>a list of IHueObject</returns>
        public static async Task<List<IHueObject>> GetBridgeDataStoreAsyncTask(Bridge bridge)
        {
            log.Info($@"Fetching DataStore from bridge : {bridge.IpAddress}");
            DataStore bresult = await bridge.GetBridgeDataStoreAsyncTask();
            List<IHueObject> hr = null;
            if (bresult == null) return hr;
            DataStore ds = bresult;
            Group zero = await GetGroupZeroAsynTask(bridge);
            if (zero != null)
            {
                ds.groups.Add("0", zero);
            }

            hr = ProcessDataStore(ds);
            log.Debug("Bridge data store : " + hr);

            return hr;
        }

        /// <summary>
        /// Get the Group Zero.
        /// </summary>
        /// <param name="bridge"></param>
        /// <returns></returns>
        private static Group GetGroupZero(Bridge bridge)
        {
            return bridge.GetObject<Group>("0");
        }

        /// <summary>
        /// Get the Group Zero async.
        /// </summary>
        /// <param name="bridge"></param>
        /// <returns></returns>
        private static async Task<Group> GetGroupZeroAsynTask(Bridge bridge)
        {
            return (Group)await bridge.GetObjectAsyncTask("0", typeof(Group));
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
            BridgeSettings bresult = bridge.GetBridgeSettings();           
            if (bresult == null) return null;
            string hr = string.Empty;
            log.Debug("Fetching bridge mac : " + bresult.mac);
            return bresult.mac;
        }

        /// <summary>
        /// Check if api key is authorized withthe bridge is authorized.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Check if the bridge is authorized.</returns>
        public static bool IsAuthorized(Bridge bridge)
        {
            BridgeSettings bresult = bridge.GetBridgeSettings();
            if (bresult == null) return false;
            log.Debug("Checking if bridge is authorized : " + Serializer.SerializeToJson(bresult.portalservices));
            return bresult.portalservices != null;
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
                Light bresult = bridge.GetObject<Light>(obj.Id);
                if (bresult != null)
                {
                    Light currentState = bresult;

                    if (currentState.state.reachable == false)
                    {
                        hr= GetImageForLight(LightImageState.Unr, currentState.modelid);
                    }
                    else
                    {
                        if (currentState.state.on == true)
                        {
                            log.Debug("Toggling light state : OFF");
                            bool bsetlightstate = bridge.SetState(new State { on = false, transitiontime = tt}, obj.Id);

                            if (bsetlightstate)
                            {
                                hr = GetImageForLight(LightImageState.Off, currentState.modelid);
                            }

                        }
                        else
                        {
                            log.Debug("Toggling light state : ON");
                            bool bsetlightstate = bridge.SetState(new State { on = true, transitiontime = tt, bri = WinHueSettings.settings.DefaultBriLight }, obj.Id);

                            if (bsetlightstate)
                            {

                                hr = GetImageForLight(LightImageState.On, currentState.modelid);
                            }

                        }

                    }
                }


            }
            else
            {
                Group bresult = bridge.GetObject<Group>(obj.Id);

                if (bresult != null)
                {
                    Group currentstate = bresult;
                    if (currentstate.action.on == true)
                    {
                        log.Debug("Toggling group state : ON");
                        bool bsetgroupstate = bridge.SetState(new Action { on = false, transitiontime = tt}, obj.Id);

                        if (bsetgroupstate)
                        {
                            hr = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOff_Large);
                        }

                    }
                    else
                    {
                        log.Debug("Toggling group state : OFF");
                        bool bsetgroupstate = bridge.SetState(new Action { on = true, transitiontime = tt, bri = WinHueSettings.settings.DefaultBriGroup }, obj.Id);
                        if (bsetgroupstate)
                        {
                            hr = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOn_Large);
                        }

                    }
                }

            }

            return hr;
        }

        /// <summary>
        /// Toggle the state of an object on and off (Light or group)
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <param name="obj">Object to toggle.</param>
        /// <param name="tt">Transition Time (Optional)</param>
        /// <returns>The new image of the object.</returns>
        public static async Task<ImageSource> ToggleObjectOnOffStateAsyncTask(Bridge bridge, IHueObject obj, ushort? tt = null)
        {
            ImageSource hr = null;
            if (obj is Light)
            {
                Light bresult = await bridge.GetObjectAsyncTask<Light>(obj.Id);
                if (bresult == null) return null;
                Light currentState = bresult;

                if (currentState.state.reachable == false)
                {
                    hr = GetImageForLight(LightImageState.Unr, currentState.modelid);
                }
                else
                {
                    if (currentState.state.@on == true)
                    {
                        log.Debug("Toggling light state : OFF");
                        bool bsetlightstate = await bridge.SetStateAsyncTask(new State { @on = false, transitiontime = tt }, obj.Id);

                        if (bsetlightstate)
                        {
                            hr = GetImageForLight(LightImageState.Off, currentState.modelid);
                        }

                    }
                    else
                    {
                        log.Debug("Toggling light state : ON");
                        bool bsetlightstate = await bridge.SetStateAsyncTask(new State { @on = true, transitiontime = tt, bri = WinHueSettings.settings.DefaultBriLight }, obj.Id);

                        if (bsetlightstate)
                        {

                            hr = GetImageForLight(LightImageState.On, currentState.modelid);
                        }

                    }

                }
            }
            else
            {
                Group bresult = await bridge.GetObjectAsyncTask<Group>(obj.Id);

                if (bresult == null) return null;
                Group currentstate = bresult;
                if (currentstate.action.@on == true)
                {
                    log.Debug("Toggling group state : ON");
                    bool bsetgroupstate = await bridge.SetStateAsyncTask(new Action { @on = false, transitiontime = tt }, obj.Id);

                    if (bsetgroupstate)
                    {
                        hr = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOff_Large);
                    }

                }
                else
                {
                    log.Debug("Toggling group state : OFF");
                    bool bsetgroupstate = await bridge.SetStateAsyncTask(new Action { @on = true, transitiontime = tt, bri = WinHueSettings.settings.DefaultBriGroup }, obj.Id);
                    if (bsetgroupstate)
                    {
                        hr = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOn_Large);
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
            Dictionary<string,Whitelist> bresult = bridge.GetUserList();
            
            if (bresult == null) return null;
            
            Dictionary<string, Whitelist> brlisteusers = bresult;
            List<Whitelist> listusers = new List<Whitelist>();
            foreach (KeyValuePair<string, Whitelist> kvp in brlisteusers)
            {
                log.Debug($"Parsing user ID {kvp.Key}, {kvp.Value}");
                kvp.Value.Id = kvp.Key;
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
                Dictionary<string, Light> res = bridge.GetListObjects<Light>();
                if (res != null)
                    hr = ProcessLights(res) as List<T>;
            }
            else if(typeof(T) == typeof(Group))
            {
                Dictionary<string, Group> res = bridge.GetListObjects<Group>();
                if (res != null)
                    hr = ProcessGroups(res) as List<T>;
            }
            else if (typeof(T) == typeof(Scene))
            {
                Dictionary<string, Scene> res = bridge.GetListObjects<Scene>();
                if(res != null)
                    hr = ProcessScenes(res) as List<T>;
            }
            else if(typeof(T) == typeof(Schedule))
            {
                Dictionary<string, Schedule> res = bridge.GetListObjects<Schedule>();
                if (res != null)
                    hr = ProcessSchedules(res) as List<T>;
            }
            else if (typeof(T) == typeof(ISensor))
            {
                Dictionary<string, ISensor> res = bridge.GetListObjects<ISensor>();
                if (res != null)
                    hr = ProcessSensors(res) as List<T>;

            }
            else if (typeof(T) == typeof(Resourcelink))
            {
                Dictionary<string, Resourcelink> res = bridge.GetListObjects<Resourcelink>();
                if (res != null)
                    hr = ProcessRessourceLinks(res) as List<T>;
            }

            return hr;
        }

        /// <summary>
        /// Get a specific object from the bridge.
        /// </summary>
        /// <typeparam name="T">Type of object to get</typeparam>
        /// <param name="bridge">the bridge to get the object from</param>
        /// <param name="id">the id of the object</param>
        /// <returns>the requested object or null if error.</returns>
        public static T GetObject<T>(Bridge bridge, string id) where T : IHueObject
        {
            T bresult = bridge.GetObject<T>(id);
            T Object = bresult;
            if (bresult != null)
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
                else if (typeof(T) == typeof(ISensor))
                {
                    ISensor sensor = bresult as ISensor;
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

        /// <summary>
        /// Get object from the bridge in async
        /// </summary>
        /// <param name="bridge">bridge to get the object from.</param>
        /// <param name="id">The id of the object</param>
        /// <param name="objecttype">the type of the object to get.</param>
        /// <returns>the object requested.</returns>
        public static async Task<IHueObject> GetObjectAsyncTask(Bridge bridge, string id, Type objecttype) 
        {
            IHueObject bresult = await bridge.GetObjectAsyncTask(id,objecttype);
            IHueObject Object = bresult;
            if (Object == null) return null;
            Object.Id = id;


            if (bresult == null) return Object;
            if (objecttype == typeof(Light))
            {
                Light light = Object as Light;

                log.Debug("Light : " + Object);
                light.Id = id;
                light.Image =
                    GetImageForLight(
                        light.state.reachable.GetValueOrDefault()
                            ? light.state.@on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off
                            : LightImageState.Unr, light.modelid);
                Object = light;
            }
            else if (objecttype == typeof(Group))
            {
                Group group = Object as Group;
                log.Debug("Group : " + @group);
                @group.Id = id;
                @group.Image = GDIManager.CreateImageSourceFromImage(@group.action.@on.GetValueOrDefault() ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
                Object = @group;
            }
            else if (typeof(ISensor).IsAssignableFrom(objecttype))
            {
                ISensor sensor = bresult as ISensor;
                sensor.Id = id;
                sensor.Image = GDIManager.CreateImageSourceFromImage(sensor.type == "ZGPSwitch" ? Properties.Resources.huetap : Properties.Resources.sensor);
                Object = sensor;
            }
            else if (objecttype == typeof(Rule))
            {
                log.Debug("Rule : " + Object);
                Object.Id = id;
                Object.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);

            }
            else if (objecttype == typeof(Scene))
            {
                log.Debug("Scene : " + Object);
                Object.Id = id;
                Object.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.SceneLarge);
            }
            else if (objecttype == typeof(Schedule))
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
                Object = schedule;
            }
            else if (objecttype == typeof(Resourcelink))
            {
                Object.Id = id;
                Object.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.resource);                    
            }

            return Object;
        }


    }

}