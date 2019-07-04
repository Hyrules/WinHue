using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.Communication2;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        /// <summary>
        /// List of possible light state.
        /// </summary>
        public enum LightImageState { On = 0, Off = 1, Unr = 3 }

        /// <summary>
        /// Logging 
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private void RemoveHiddenObjects<T>(ref List<T> list, List<Tuple<string, string>> hiddenObjects) where T : IHueObject
        {
            
            foreach (Tuple<string, string> h in hiddenObjects)
            {
                
                list.RemoveAll(x => x.Id == h.Item1 && x.GetType().Name == h.Item2);
                //int index = list.FindItemIndex(x => x.Id == h.Item1 && x.GetType().Name == h.Item2);
                //if (index == -1) continue;
                //list.RemoveAt(index);
            }
        }

        /// <summary>
        /// Process the search results.
        /// </summary>
        /// <param name="bridge">Bridge to process the search results from</param>
        /// <param name="results">Search result to process</param>
        /// <param name="type">Type of result to process. Lights or Sensors</param>
        /// <returns>A list of objects.</returns>
        private List<IHueObject> ProcessSearchResult(SearchResult results, bool type)
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
                    Light bresult = GetObject<Light>(kvp.Key);
                    if (bresult == null) continue;
                    Light newlight = bresult;
                    newlight.Id = kvp.Key;
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
                    Sensor bresult = GetObject<Sensor>(kvp.Key);
                    if (bresult == null) continue;
                    Sensor newSensor = bresult;
                    newSensor.Id = kvp.Key;
                    newlist.Add(newSensor);
                }
            }

            return newlist;
        }

        /// <summary>
        /// Check if api key is authorized withthe bridge is authorized.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Check if the bridge is authorized.</returns>
        public bool IsAuthorized()
        {
            BridgeSettings bresult = GetBridgeSettings();
            if (bresult == null) return false;
            log.Debug("Checking if bridge is authorized : " + Serializer.SerializeJsonObject(bresult.portalservices));
            return bresult.portalservices != null;
        }


        /// <summary>
        /// GEt the list of newly discovered lights
        /// </summary>
        /// <param name="bridge">Bridge to get the new lights from.</param>
        /// <returns>A list of lights.</returns>
        public async Task<List<IHueObject>> GetBridgeNewLightsAsyncTask()
        {
            log.Debug($@"Getting new lights from bridge {IpAddress}");
            SearchResult bresult = await GetNewObjectsAsync<Light>();
            if (bresult == null) return null;
            log.Debug("Search Result : " + bresult);
            return ProcessSearchResult(bresult, true);
        }


        /// <summary>
        /// Get a list of new sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public async Task<List<IHueObject>> GetBridgeNewSensorsAsyncTask()
        {
            log.Debug($@"Getting new sensors from bridge : {IpAddress}");
            SearchResult bresult = await GetNewObjectsAsync<Sensor>();
            List<IHueObject> hr = ProcessSearchResult(bresult, false);
            log.Debug("Search Result : " + Serializer.SerializeJsonObject(hr));
            return hr;
        }

        /// <summary>
        /// Display a message box with all the last errors.
        /// </summary>
        public void ShowErrorMessages()
        {
            StringBuilder errors = new StringBuilder();
            errors.AppendLine(GlobalStrings.Error_WhileCreatingObject);
            foreach (Error error in LastCommandMessages.ListMessages.OfType<Error>())
            {
                errors.AppendLine("");
                errors.AppendLine(error.ToString());
            }

            MessageBox.Show(errors.ToString(), GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Send a raw json command to the bridge without altering the bridge lastmessages
        /// </summary>
        /// <param name="url">url to send the command to.</param>
        /// <param name="data">raw json data string</param>
        /// <param name="type">type of command.</param>
        /// <returns>json test resulting of the command.</returns>
        public string SendRawCommand(string url, string data, WebRequestType type)
        {
            HttpResult comres = HueHttpClient.SendRequest(new Uri(url), type, data);
            if (comres.Success)
            {
                if(type != WebRequestType.Get)
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return comres.Data;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }


        /// <summary>
        /// Send a raw json command to the bridge without altering the bridge lastmessages
        /// </summary>
        /// <param name="url">url to send the command to.</param>
        /// <param name="data">raw json data string</param>
        /// <param name="type">type of command.</param>
        /// <returns>json test resulting of the command.</returns>
        public async Task<string> SendRawCommandAsyncTask(string url, string data, WebRequestType type)
        {
            HttpResult comres = await HueHttpClient.SendRequestAsyncTask(new Uri(url), type, data);
            if (comres.Success)
            {
                if(type != WebRequestType.Get)
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return comres.Data;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this, url, WebExceptionStatus.NameResolutionFailure));
            return null;
        }

        /// <summary>
        /// Toggle the state of an object on and off (Light or group)
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <param name="obj">Object to toggle.</param>
        /// <param name="tt">Transition Time (Optional)</param>
        /// <param name="dimvalue">Value for the dim (Optional)</param>
        /// <param name="state">New state at toggle (Optional)</param>
        /// <returns>The new image of the object.</returns> 
        public async Task<bool> ToggleObjectOnOffStateAsyncTask(IHueObject obj, ushort? tt = null, byte? dimvalue = null, IBaseProperties state = null)
        {
            bool result = false;
            if (obj is Light)
            {
                Light bresult = await GetObjectAsync<Light>(obj.Id);
                if (bresult == null) return false;
                Light currentState = bresult;

                if (currentState.state.@on == true)
                {
                    log.Debug("Toggling light state : OFF");
                    result = await SetStateAsyncTask(new State { @on = false, transitiontime = tt }, obj.Id);
                }
                else
                {
                    log.Debug("Toggling light state : ON");

                    State newstate;

                    if (WinHueSettings.settings.SlidersBehavior == 0)
                    {
                        newstate = new State()
                        {
                            on = true,
                            transitiontime = tt
                        }; ;
                        if (!WinHueSettings.settings.UseLastBriState && bresult.state.bri != null)
                        {
                            newstate.bri = dimvalue ?? WinHueSettings.settings.DefaultBriLight;
                        }
                    }
                    else
                    {
                        newstate = state as State ?? new State();
                        newstate.on = true;
                        newstate.transitiontime = tt;
                    }

                    result = await SetStateAsyncTask(newstate, obj.Id);


                }

                
            }
            else
            {
                Group bresult = await GetObjectAsync<Group>(obj.Id);

                if (bresult == null) return false;
                Group currentstate = bresult;
                if (currentstate.action.@on == true)
                {
                    log.Debug("Toggling group state : ON");
                    result = await SetStateAsyncTask(new Action { @on = false, transitiontime = tt }, obj.Id);

                }
                else
                {
                    log.Debug("Toggling group state : OFF");

                    Action newaction;

                    if (WinHueSettings.settings.SlidersBehavior == 0)
                    {
                        newaction = new Action()
                        {
                            on = true,
                            transitiontime = tt
                        };

                        if (!WinHueSettings.settings.UseLastBriState)
                        {
                            newaction.bri = dimvalue ?? WinHueSettings.settings.DefaultBriGroup;
                        }
                    }
                    else
                    {
                        newaction = state as Action ?? new Action();
                        newaction.on = true;
                        newaction.transitiontime = tt;
                    }

                    result = await SetStateAsyncTask(newaction, obj.Id);
                    

                }
            }

            return result;
        }

        /// <summary>
        /// Check if the bridge is autorized with the current ApiKey
        /// </summary>
        /// <returns></returns>
        public bool CheckAuthorization()
        {
            bool authorization = false;
            if (ApiKey == String.Empty) return false;
            BridgeSettings bs = GetBridgeSettings();
            if (bs == null) return false;
            if (bs.portalservices != null)
            {
                authorization = true;
            }

            return authorization;
        }

        /// <summary>
        /// Process the failure of a command to the bridge.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="exception"></param>
        private void ProcessCommandFailure(string url, WebExceptionStatus exception)
        {
            switch (exception)
            {
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() {type = -1, address = url, description = "A Timeout occured." });
                    break;
                case WebExceptionStatus.ConnectFailure:
                    LastCommandMessages.AddMessage(new Error() {type = -1, address = url, description = "Unable to contact the bridge"});
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { type = -2, address = url, description = "A unknown error occured." });
                    break;
            }
            BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs(this,url, exception));
        }

        public override string ToString()
        {
            return $@"ipaddress : {IpAddress}, IsDefault : {IsDefault}, SwVersion : {SwVersion}, Mac : {Mac}, ApiVersion : {ApiVersion}, ApiKey : {ApiKey}, BridgeUrl : {BridgeUrl} ";
        }
    }
}
