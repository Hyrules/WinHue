using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    public partial class Bridge
    {
        public CommandResult FindNewObjects<T>() where T: HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}"), WebRequestType.POST);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Success = lastMessages.FailureCount == 0;
                        bresult.resultobject = lastMessages;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.resultobject = comres.data;
                        bresult.ex = comres.ex;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = comres.data;
                        bresult.ex = comres.ex;
                        break;
                }
                
            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
            }
            return bresult;
        }

    }
}
