using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using HueLib2;

namespace HueLib2
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
        public CommandResult ForceCheckForUpdate()
        {

            CommandResult bresult = new CommandResult { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"checkforupdate\":true}}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    if (lastMessages.FailureCount == 0)
                    {
                        bresult.Success = true;
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
            bresult.resultobject = lastMessages;
            return bresult;
        }
        
        /// <summary>
        /// Check if there is an update for the bridge.
        /// </summary>
        /// <returns>Software Update or null.</returns>
        public CommandResult GetSwUpdate()
        {
            SwUpdate swu = new SwUpdate();
            CommandResult bresult = new CommandResult { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if (brs != null)
                    {
                        bresult.Success = true;
                        bresult.resultobject = brs.swupdate;
                    }
                    else
                    {
                        List<Message> lstmsg = Serializer.DeserializeToObject<List<Message>>(Communication.lastjson);
                        if (lstmsg == null)
                            goto default;
                        else
                        {
                            lastMessages = new MessageCollection(lstmsg);
                            bresult.resultobject = lastMessages;
                        }

                        bresult.resultobject = Serializer.DeserializeToObject<List<Message>>(comres.data);
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    bresult.resultobject = lastMessages;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.resultobject = lastMessages;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Update the bridge firmware.
        /// </summary>
        /// <returns>True or False command sent succesfully.</returns>
        public CommandResult DoSwUpdate()
        {
          
            CommandResult bresult = new CommandResult { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\":" + Serializer.SerializeToJson<SwUpdate>(new SwUpdate() { updatestate = 3 }) + "}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    if (lastMessages.FailureCount == 0)
                    {
                        bresult.Success = true;
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
            bresult.resultobject = lastMessages;
            return bresult;
        }

        /// <summary>
        /// Set notify to false once the update has been done.
        /// </summary>
        /// <returns>True or false if the operation was successful or not</returns>
        public CommandResult SetNotify()
        {
            
            CommandResult bresult = new CommandResult { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"notify\":false}}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    if (lastMessages.FailureCount == 0)
                        bresult.Success = true;
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    break;
            }
            bresult.resultobject = lastMessages;
            return bresult;
        }
    }
}
