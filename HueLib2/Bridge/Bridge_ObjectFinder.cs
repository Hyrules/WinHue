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
                CommResult result = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}"), WebRequestType.POST);
                if (result.status == WebExceptionStatus.Success)
                {
                    lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(result.data));
                    bresult.Success = lastMessages.FailureCount == 0;
                    bresult.resultobject = lastMessages;
                }
                else
                {
                    bresult.resultobject = result.data;
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
