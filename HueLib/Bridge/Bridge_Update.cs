using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using HueLib;

namespace HueLib
{
    public partial class Bridge
    {

        /// <summary>
        /// Available states for the updates
        /// </summary>
        public enum UpdateState 
        { 
            /// <summary>
            /// No Updates are available.
            /// </summary>
            NoUpdateAvailable, 
            /// <summary>
            /// Incoming Update not yet downloaded.
            /// </summary>
            IncomingUpdate,
            /// <summary>
            /// An update is ready.
            /// </summary>
            UpdateReady,
            /// <summary>
            /// Update being applied.
            /// </summary>
            Updating ,
            /// <summary>
            /// There was an error reading update status.
            /// </summary>
            ErrorReadingUpdate 
        };

        /// <summary>
        /// Force the bridge to check for an update.
        /// </summary>
        /// <returns>True or false if the operation is successful (does not return if there is an update)</returns>
        public bool ForceCheckForUpdate()
        {
            bool result = false;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"checkforupdate\":true}}");
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                    {
                        result = true;

                    }
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        
        /// <summary>
        /// Check if there is an update for the bridge.
        /// </summary>
        /// <returns>Software Update or null.</returns>
        public bool GetSwUpdate()
        {
            SwUpdate swu = new SwUpdate();
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);
                if (!string.IsNullOrEmpty(message))
                {
                    if (!message.Contains("\"error\""))
                    {

                        Match mt = Regex.Match(message, "\"swupdate\":{(.*?)}");
                        swu = Serializer.DeserializeToObject<SwUpdate>(mt.Value.Remove(0, 11) + "}");
                        return swu.updatestate == 2;
                    }
                    else
                    {
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(Communication.lastjson));
                        swu = null;
                    }
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    swu = null;
                }
            }
            catch (Exception)
            {
                swu = null;
            }
            return false;
        }

        /// <summary>
        /// Update the bridge firmware.
        /// </summary>
        /// <returns>True or False command sent succesfully.</returns>
        public bool DoSwUpdate()
        {
            bool result = false;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\":" + Serializer.SerializeToJson<SwUpdate>(new SwUpdate() {updatestate = 3}) + "}");
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                    {
                        result = true;
                        
                    }
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Set notify to false once the update has been done.
        /// </summary>
        /// <returns>True or false if the operation was successful or not</returns>
        public bool SetNotify()
        {
            bool result = false;
            try
            {
                string message = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"notify\":false}}");
                if (!string.IsNullOrEmpty(message))
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(message));
                    if (lastMessages.SuccessCount == 1)
                        result = true;
                }
                else
                {
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
