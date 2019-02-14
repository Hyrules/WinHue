using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using log4net;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.Lights.SupportedDevices;
using WinHue3.Interface;
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

        #region OBJECT_GETTERS

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
        private static ImageSource GetImageForLight(LightImageState imagestate, string modelid = null, string archetype = null)
        {
            string modelID = modelid ?? "DefaultHUE";
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
                return LightImageLibrary.Images["DefaultHUE"][state];
            }

            ImageSource newImage;

            if (LightImageLibrary.Images.ContainsKey(modelID)) // Check model ID first
            {
                log.Debug("STATE : " + state + " MODELID : " + modelID);
                newImage = LightImageLibrary.Images[modelID][state];

            }
            else if (archetype != null && LightImageLibrary.Images.ContainsKey(archetype)) // Check archetype after model ID, giving model ID priority
            {
                log.Debug("STATE : " + state + " ARCHETYPE : " + archetype);
                newImage = LightImageLibrary.Images[archetype][state];
            }
            else // Neither model ID or archetype are known
            {
                log.Debug("STATE : " + state + " unknown MODELID : " + modelID + " and ARCHETYPE : " + archetype + " using default images.");
                newImage = LightImageLibrary.Images["DefaultHUE"][state];
            }
            return newImage;
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
        /// Get the list of resource links from the bridge async.
        /// </summary>
        /// <param name="bridge">Bridge to get the resource links from.</param>
        /// <returns>A list of resource links.</returns>
        public static List<Resourcelink> GetBridgeResourceLinks(Bridge bridge)
        {
            log.Info($@"Fetching Resource links from bridge : {bridge.IpAddress}");
            Dictionary<string, Resourcelink> bresult = bridge.GetListObjects<Resourcelink>();
            if (bresult == null) return null;
            List<Resourcelink> rl = ProcessResourceLinks(bresult);
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
            List<Resourcelink> rl = ProcessResourceLinks(bresult);
            return rl;
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

        private static void RemoveHiddenObjects<T>(ref List<T> list, List<Tuple<string,string>> hiddenObjects) where T : IHueObject
        {

            foreach (Tuple<string, string> h in hiddenObjects)
            {

                int index = list.FindItemIndex(x => x.Id == h.Item1 && x.GetHueType() == h.Item2);
                if (index == -1) continue;
                list.RemoveAt(index);
            }
        }

        /// <summary>
        /// Get a list of sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static List<Sensor> GetBridgeSensors(Bridge bridge)
        {
            log.Debug($@"Getting all sensors from bridge {bridge.IpAddress}");
            Dictionary<string, Sensor> bresult = bridge.GetListObjects<Sensor>();
            if (bresult == null) return null;
            List<Sensor> hr = ProcessSensors(bresult);
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Sensors : " + Serializer.SerializeJsonObject(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static async Task<List<Sensor>> GetBridgeSensorsAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all sensors from bridge {bridge.IpAddress}");
            Dictionary<string, Sensor> bresult = await bridge.GetListObjectsAsyncTask<Sensor>();
            if (bresult == null) return null;
            List<Sensor> hr = ProcessSensors(bresult);
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Sensors : " + Serializer.SerializeJsonObject(hr));
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
            SearchResult bresult = bridge.GetNewObjects<Sensor>();
            List<IHueObject> hr = ProcessSearchResult(bridge, bresult, false);
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("Search Result : " + Serializer.SerializeJsonObject(hr));
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
            SearchResult bresult = await bridge.GetNewObjectsAsyncTask<Sensor>();
            List<IHueObject> hr = ProcessSearchResult(bridge, bresult, false);
            log.Debug("Search Result : " + Serializer.SerializeJsonObject(hr));
            return hr;
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
            log.Debug("List lights : " + Serializer.SerializeJsonObject(bresult));
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
            log.Debug("List lights : " + Serializer.SerializeJsonObject(bresult));
            return ProcessLights(bresult);
        }

        /// <summary>
        /// GEt the list of newly discovered lights
        /// </summary>
        /// <param name="bridge">Bridge to get the new lights from.</param>
        /// <returns>A list of lights.</returns>
        [Obsolete]
        public static async Task<List<IHueObject>> GetBridgeNewLightsAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting new lights from bridge {bridge.IpAddress}");
            SearchResult bresult = await bridge?.GetNewObjectsAsyncTask<Light>();
            if (bresult == null) return null;
            log.Debug("Search Result : " + bresult);
            return ProcessSearchResult(bridge, bresult, true);
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
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List groups : " + Serializer.SerializeJsonObject(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of group with ID, name and image populated from the selected bridge async.
        /// </summary>
        /// <param name="bridge">Bridge to get the groups from.</param>
        /// <returns>A List of Group.</returns>
        [Obsolete]
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
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List groups : " + Serializer.SerializeJsonObject(hr));
            return hr;
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
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Scenes : " + Serializer.SerializeJsonObject(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of scenes with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the scenes from.</param>
        /// <returns>A List of scenes.</returns>
        [Obsolete]
        public static async Task<List<Scene>> GetBridgeScenesAsyncTask(Bridge bridge)
        {
            log.Debug($@"Getting all scenes from bridge {bridge.IpAddress}");
            Dictionary<string, Scene> bresult = await bridge.GetListObjectsAsyncTask<Scene>();
            if (bresult == null) return null;
            List<Scene> hr = ProcessScenes(bresult);
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Scenes : " + Serializer.SerializeJsonObject(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of schedules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the schedules from.</param>
        /// <returns>A List of schedules.</returns>
        public static List<Schedule> GetBridgeSchedules(Bridge bridge)
        {
            log.Debug($@"Getting all schedules from bridge {bridge.IpAddress}");
            Dictionary<string, Schedule> bresult = bridge.GetListObjects<Schedule>();
            if (bresult == null) return null;
            List<Schedule> hr = ProcessSchedules(bresult);
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Schedules : " + Serializer.SerializeJsonObject(hr));
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
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Schedules : " + Serializer.SerializeJsonObject(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of rules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the rules from.</param>
        /// <returns>A List of rules.</returns>
        public static List<Rule> GetBridgeRules(Bridge bridge)
        {
            log.Debug($@"Getting all rules from bridge {bridge.IpAddress}");
            Dictionary<string, Rule> bresult = bridge.GetListObjects<Rule>();
            if (bresult == null) return null;
            List<Rule> hr = ProcessRules(bresult);
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Rules : " + Serializer.SerializeJsonObject(hr));
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
            RemoveHiddenObjects(ref hr, WinHueSettings.bridges.BridgeInfo[bridge.Mac].hiddenobjects);
            log.Debug("List Rules : " + Serializer.SerializeJsonObject(hr));
            return hr;
        }

        /// <summary>
        /// Get a list of users on the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>A list of users</returns>
        public static List<Whitelist> GetBridgeUsers(Bridge bridge)
        {
            Dictionary<string, Whitelist> bresult = bridge.GetUserList();

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

        #endregion

        #region PROCESSORS

        /// <summary>
        /// Process the data from the bridge datastore.
        /// </summary>
        /// <param name="datastore">Datastore to process.</param>
        /// <returns>A list of object processed.</returns>
        public static List<IHueObject> ProcessDataStore(DataStore datastore)
        {
            List<IHueObject> newlist = new List<IHueObject>();
            log.Debug("Processing datastore...");
            newlist.AddRange(ProcessLights(datastore.lights));
            newlist.AddRange(ProcessGroups(datastore.groups));
            newlist.AddRange(ProcessSchedules(datastore.schedules));
            newlist.AddRange(ProcessScenes(datastore.scenes));
            newlist.AddRange(ProcessSensors(datastore.sensors));
            newlist.AddRange(ProcessRules(datastore.rules));
            newlist.AddRange(ProcessResourceLinks(datastore.resourcelinks));
            log.Debug("Processing complete.");
            return newlist;
        }

        private static List<Resourcelink> ProcessResourceLinks(Dictionary<string, Resourcelink> listrl)
        {
            if (listrl == null) return new List<Resourcelink>();
            List<Resourcelink> newlist = new List<Resourcelink>();

            foreach (KeyValuePair<string, Resourcelink> kvp in listrl)
            {
                if (kvp.Value is null)
                {
                    log.Error("Resource Link ID : " + kvp.Key + " was null");
                    continue;
                }
                kvp.Value.Id = kvp.Key;
                kvp.Value.visible = true;
                log.Debug("Processing resource links : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.resource);
                newlist.Add(kvp.Value);
            }
            return newlist;
        }

        /// <summary>
        /// Process a list of sensors
        /// </summary>
        /// <param name="listsensors">List of sensors to process.</param>
        /// <returns>A list of processed sensors.</returns>
        private static List<Sensor> ProcessSensors(Dictionary<string, Sensor> listsensors)
        {
            if (listsensors == null) return new List<Sensor>();
            List<Sensor> newlist = new List<Sensor>();

            foreach (KeyValuePair<string, Sensor> kvp in listsensors)
            {
                if (kvp.Value == null)
                {
                    log.Error("Sensor ID : " + kvp.Key + " was null");
                    continue;
                }
                kvp.Value.Id = kvp.Key;
                kvp.Value.visible = true;
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
        /// Process the list of lights
        /// </summary>
        /// <param name="listlights">List of lights to process.</param>
        /// <returns>A list of processed lights.</returns>
        private static List<Light> ProcessLights(Dictionary<string, Light> listlights)
        {
            if(listlights == null) return new List<Light>();
            List<Light> newlist = new List<Light>();

            foreach (KeyValuePair<string, Light> kvp in listlights)
            {
                if (kvp.Value is null)
                {
                    log.Error("Light ID : " + kvp.Key + " was null");
                    continue;
                }
                kvp.Value.Id = kvp.Key;
                kvp.Value.visible = true;
                if(kvp.Value.manufacturername == "OSRAM" && WinHueSettings.settings.OSRAMFix)
                {
                    kvp.Value.Image = GetImageForLight(kvp.Value.state.on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off, kvp.Value.modelid);
                }
                else
                {
                    kvp.Value.Image = GetImageForLight(kvp.Value.state.reachable.GetValueOrDefault() ? kvp.Value.state.on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off : LightImageState.Unr, kvp.Value.modelid, kvp.Value.config.archetype);
                }
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
        private static List<IHueObject> ProcessSearchResult(Bridge bridge, SearchResult results, bool type)
        {
            List<IHueObject> newlist = new List<IHueObject>();
            if (type) // lights
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    if (kvp.Value is null)
                    {
                        log.Error("Light search ID : " + kvp.Key + " was null");
                        continue;
                    }
                    Light bresult = bridge.GetObject<Light>(kvp.Key);
                    if (bresult == null) continue;
                    Light newlight = bresult;
                    newlight.Id = kvp.Key;
                    newlight.Image = GetImageForLight(newlight.state.reachable.GetValueOrDefault() ? newlight.state.on.GetValueOrDefault() ? LightImageState.On : LightImageState.Off : LightImageState.Unr, newlight.modelid, newlight.config.archetype);
                    newlist.Add(newlight);
                }
            }
            else // sensors
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    if (kvp.Value is null)
                    {
                        log.Error("Sensor search ID : " + kvp.Key + " was null");
                        continue;
                    }
                    Sensor bresult = bridge.GetObject<Sensor>(kvp.Key);
                    if (bresult == null) continue;
                    Sensor newSensor = bresult;
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
        /// Process groups
        /// </summary>
        /// <param name="listgroups">List of group t</param>
        /// <returns>A list of processed group with image and id.</returns>
        private static List<Group> ProcessGroups(Dictionary<string, Group> listgroups)
        {
            if (listgroups == null) return new List<Group>();
            List<Group> newlist = new List<Group>();
            foreach (KeyValuePair<string, Group> kvp in listgroups)
            {
                if (kvp.Value is null)
                {
                    log.Error("Group ID : " + kvp.Key + " was null");
                    continue;
                }
                log.Debug("Processing group : " + kvp.Value);
                kvp.Value.visible = true;
                kvp.Value.Id = kvp.Key;
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(kvp.Value.state.any_on.GetValueOrDefault() ? (kvp.Value.state.all_on.GetValueOrDefault() ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupSome_Large) : Properties.Resources.HueGroupOff_Large);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }


        /// <summary>
        /// Process a list of scenes.
        /// </summary>
        /// <param name="listscenes">List of scenes to process.</param>
        /// <param name="bypassShowId">Bypass the Show ID Parameters (Used in the rule creator)</param>
        /// <returns>A list of processed scenes.</returns>
        private static List<Scene> ProcessScenes(Dictionary<string, Scene> listscenes)
        {
            if (listscenes == null) return new List<Scene>();
            List<Scene> newlist = new List<Scene>();

            foreach (KeyValuePair<string, Scene> kvp in listscenes)
            {
                if (kvp.Value is null)
                {
                    log.Error("Scene ID : " + kvp.Key + " was null");
                    continue;
                }
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing scene : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.scenes);
                kvp.Value.visible = !(kvp.Value.name.StartsWith("HIDDEN") && !WinHueSettings.settings.ShowHiddenScenes);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Process a list of schedules
        /// </summary>
        /// <param name="listschedules">List of schedules to process.</param>
        /// <returns>A list of processed schedules.</returns>
        public static List<Schedule> ProcessSchedules(Dictionary<string, Schedule> listschedules)
        {
            if (listschedules == null) return new List<Schedule>();
            List<Schedule> newlist = new List<Schedule>();

            foreach (KeyValuePair<string, Schedule> kvp in listschedules)
            {
                if (kvp.Value is null)
                {
                    log.Error("Schedule ID : " + kvp.Key + " was null");
                    continue;
                }
                log.Debug("Assigning id to schedule ");
                kvp.Value.visible = true;
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
        /// Process a list of rules.
        /// </summary>
        /// <param name="listrules">List of rules to process.</param>
        /// <returns>A processed list of rules.</returns>
        private static List<Rule> ProcessRules(Dictionary<string, Rule> listrules)
        {
            if (listrules == null) return new List<Rule>();
            List<Rule> newlist = new List<Rule>();

            foreach (KeyValuePair<string, Rule> kvp in listrules)
            {
                if (kvp.Value is null)
                {
                    log.Error("Rule ID : " + kvp.Key + " was null");
                    continue;
                }
                kvp.Value.Id = kvp.Key;
                kvp.Value.visible = true;
                log.Debug("Processing rule : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        #endregion

    }

}