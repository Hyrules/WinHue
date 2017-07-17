using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using HueLib2;
using HueLib2.BridgeMessages;

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
        public CommandResult<MessageCollection> ForceCheckForUpdate()
        {

            CommandResult<MessageCollection> bresult = new CommandResult<MessageCollection> { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"checkforupdate\":true}}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<IMessage> msg = Serializer.DeserializeToObject<List<IMessage>>(comres.data);
                    lastMessages = new MessageCollection();
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
            bresult.Data = lastMessages;
            return bresult;
        }
        
        /// <summary>
        /// Check if there is an update for the bridge.
        /// </summary>
        /// <returns>Software Update or null.</returns>
        public CommandResult<SwUpdate> GetSwUpdate()
        {
            SwUpdate swu = new SwUpdate();
            CommandResult<SwUpdate> bresult = new CommandResult<SwUpdate> { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if (brs != null)
                    {
                        bresult.Success = true;
                        bresult.Data = brs.swupdate;
                    }
                    else
                    {
                        List<IMessage> lstmsg = Serializer.DeserializeToObject<List<IMessage>>(Communication.lastjson);
                        if (lstmsg == null)
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

            return bresult;
        }

        /// <summary>
        /// Update the bridge firmware.
        /// </summary>
        /// <returns>True or False command sent succesfully.</returns>
        public CommandResult<MessageCollection> DoSwUpdate()
        {
          
            CommandResult<MessageCollection> bresult = new CommandResult<MessageCollection> { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\":" + Serializer.SerializeToJson<SwUpdate>(new SwUpdate() { updatestate = 3 }) + "}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
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
            bresult.Data = lastMessages;
            return bresult;
        }

        /// <summary>
        /// Set notify to false once the update has been done.
        /// </summary>
        /// <returns>True or false if the operation was successful or not</returns>
        public CommandResult<MessageCollection> SetNotify()
        {
            
            CommandResult<MessageCollection> bresult = new CommandResult<MessageCollection> { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"notify\":false}}");

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
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
            bresult.Data = lastMessages;
            return bresult;
        }
    }
}
