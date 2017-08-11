using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HueLib2.BridgeMessages;
using HueLib2.Objects.HueObject;

namespace HueLib2
{
    public partial class Bridge
    {
        public CommandResult<Messages> FindNewObjects<T>() where T: IHueObject
        {
            CommandResult<Messages> bresult = new CommandResult<Messages>() {Success = false};
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}"), WebRequestType.POST);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = new Messages(Serializer.DeserializeToObject<List<IMessage>>(comres.data));
                        bresult.Success = lastMessages.Success;
                        bresult.Data = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new Messages();
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}", description = "A Timeout occured.", type = 65535 });
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.Exception = comres.ex;
                        break;
                    default:
                        lastMessages = new Messages();
                        lastMessages.ListMessages.Add(new Error() { address = BridgeUrl + $"/{typename}", description = "An unkown error occured.", type = 65535 });
                        bresult.Exception = comres.ex;
                        break;
                }
                
            }
            else
            {
                bresult.Exception = new NullReferenceException("Type cannot be null.");
            }
            return bresult;
        }

    }
}
