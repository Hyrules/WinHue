using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject.BridgeMessages;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
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
            CommResult comres = Comm.SendRequest(new Uri(url), type, data);
            if (comres.Status == WebExceptionStatus.Success)
            {
                if(type != WebRequestType.Get)
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return comres.Data;
            }
            ProcessCommandFailure(url,comres.Status); 
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
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(url), type, data);
            if (comres.Status == WebExceptionStatus.Success)
            {
                if(type != WebRequestType.Get)
                    LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(comres.Data));
                return comres.Data;
            }
            ProcessCommandFailure(url, comres.Status);
            return null;
        }

        /// <summary>
        /// Get all objects from the bridge.
        /// </summary>
        /// <returns>A DataStore of objects from the bridge.</returns>
        public DataStore GetBridgeDataStore()
        {
            

            CommResult comres = Comm.SendRequest(new Uri(BridgeUrl), WebRequestType.Get);
            if (comres.Status == WebExceptionStatus.Success)
            {
                DataStore listObjets = Serializer.DeserializeToObject<DataStore>(comres.Data);
                if (listObjets != null) return listObjets;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(Comm.Lastjson));
            }
            ProcessCommandFailure(BridgeUrl, comres.Status);
            return null;
        }

        /// <summary>
        /// Get all objects from the bridge async.
        /// </summary>
        /// <returns>A DataStore of objects from the bridge.</returns>
        public async Task<DataStore> GetBridgeDataStoreAsync()
        {
            CommResult comres = await Comm.SendRequestAsyncTask(new Uri(BridgeUrl), WebRequestType.Get);
            if (comres.Status == WebExceptionStatus.Success)
            {
                DataStore listObjets = Serializer.DeserializeToObject<DataStore>(comres.Data);
                if (listObjets != null) return listObjets;
                LastCommandMessages.AddMessage(Serializer.DeserializeToObject<List<IMessage>>(Comm.Lastjson));
            }
            ProcessCommandFailure(BridgeUrl, comres.Status);
            return null;
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
