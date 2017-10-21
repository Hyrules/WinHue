using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {

        /// <summary>
        /// Force the bridge to check online for an update.
        /// </summary>
        /// <returns>True or false if the operation is successful (does not return if there is an update)</returns>
        public bool CheckOnlineForUpdate()
        {
            CommResult comres = Comm.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"checkforupdate\":true}}");
            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data)) ;
                    return LastCommandMessages.Success;
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error(){address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535});
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "An unknown error occured.", type = 65535 });
                    break;
            }
            return false;
        }


        /// <summary>
        /// Force the bridge to check online for an update async.
        /// </summary>
        /// <returns>True or false if the operation is successful (does not return if there is an update)</returns>
        public async Task<bool> CheckOnlineForUpdateAsyncTask()
        {
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"checkforupdate\":true}}");
            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535 });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "An unknown error occured.", type = 65535 });
                    break;
            }
            return false;
        }

        /// <summary>
        /// Check if there is an update available on the bridge.
        /// </summary>
        /// <returns>Software Update or null.</returns>
        public bool CheckUpdateAvailable()
        {
            CommResult comres = Comm.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                    if (brs != null)
                    {
                        return brs.swupdate.updatestate == 2;
                    }
                    else
                    {
                        LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(Comm.Lastjson));
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535 });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A unknown error occured.", type = 65535 });
                    break;
            }
            return false;
        }

        /// <summary>
        /// Check if there is an update available on the bridge async.
        /// </summary>
        /// <returns>Software Update or null.</returns>
        public async Task<bool> CheckUpdateAvailableAsyncTask()
        {
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.Data);
                    if (brs != null)
                    {
                        if (brs.swupdate.updatestate == 2)
                        {
                            UpdateAvailable = true;
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(Comm.Lastjson));
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535 });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A unknown error occured.", type = 65535 });
                    break;
            }
            return false;
        }

        /// <summary>
        /// Update the bridge firmware.
        /// </summary>
        /// <returns>True or False command sent succesfully.</returns>
        public bool UpdateBridge()
        {
          
            CommResult comres = Comm.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\":" + Serializer.SerializeToJson(new SwUpdate() { updatestate = 3 }) + "}");

            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535 });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A unknown error occured.", type = 65535 });
                    break;
            }

            return false;
        }

        /// <summary>
        /// Update the bridge firmware async.
        /// </summary>
        /// <returns>True or False command sent succesfully.</returns>
        public async Task<bool> UpdateBridgeAsyncTask()
        {

            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\":" + Serializer.SerializeToJson(new SwUpdate() { updatestate = 3 }) + "}");

            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535 });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A unknown error occured.", type = 65535 });
                    break;
            }

            return false;
        }

        /// <summary>
        /// Set notify to false once the update has been done.
        /// </summary>
        /// <returns>True or false if the operation was successful or not</returns>
        public bool SetNotify()
        {
            
            CommResult comres = Comm.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"notify\":false}}");

            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;
                    
                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535 });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:                
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A unknown error occured.", type = 65535 });
                    break;
            }

            return false;
        }

        /// <summary>
        /// Set notify to false once the update has been done.
        /// </summary>
        /// <returns>True or false if the operation was successful or not</returns>
        public async Task<bool> SetNotifyAsyncTask()
        {

            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, "{\"swupdate\": {\"notify\":false}}");

            switch (comres.Status)
            {
                case WebExceptionStatus.Success:
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                    return LastCommandMessages.Success;

                case WebExceptionStatus.Timeout:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A Timeout occured.", type = 65535 });
                    BridgeNotResponding?.Invoke(this, _e);
                    break;
                default:
                    LastCommandMessages.AddMessage(new Error() { address = BridgeUrl + "/config", description = "A unknown error occured.", type = 65535 });
                    break;
            }

            return false;
        }

    }
}
