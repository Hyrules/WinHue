using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using HueLib;
using HueLib.BridgeMessages.Error;

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

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"checkforupdate\":true}}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    lastMessages = new MessageCollection(lstmsg);
                    if (lastMessages.SuccessCount == 1)
                    {
                        result = true;

                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    if (!comres.data.Contains("\"error\""))
                    {

                        Match mt = Regex.Match(comres.data, "\"swupdate\":{(.*?)}");
                        swu = Serializer.DeserializeToObject<SwUpdate>(mt.Value.Remove(0, 11) + "}");
                        return swu.updatestate == 2;
                    }
                    else
                    {
                        List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                        if(lstmsg == null)
                            goto default;
                        else
                        {
                            lastMessages = new MessageCollection(lstmsg);
                        }
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\":" + Serializer.SerializeToJson<SwUpdate>(new SwUpdate() { updatestate = 3 }) + "}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    lastMessages = new MessageCollection(lstmsg);
                    if (lastMessages.SuccessCount == 1)
                    {
                        result = true;

                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
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

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"notify\":false}}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    if(lstmsg == null)
                        goto default;
                    lastMessages = new MessageCollection(lstmsg);
                    if (lastMessages.SuccessCount == 1)
                        result = true;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }

            return result;
        }
    }
}
