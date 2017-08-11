using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2.BridgeMessages
{
    public class Messages
    {
        public Messages(List<IMessage> msg)
        {
            ListMessages = msg;
        }

        public Messages() { ListMessages = new List<IMessage>(); }
            
        public List<IMessage> ListMessages { get; internal set; }
        public bool Success => ListMessages.TrueForAll(x => x.GetType() == typeof(Success));
        public bool Error => ListMessages.TrueForAll(x => x.GetType() == typeof(Error));
        public Error LastError => ListMessages.LastOrDefault(x => x.GetType() == typeof(Error)) as Error;
        public Success LastSuccess => ListMessages.LastOrDefault(x => x.GetType() == typeof(Success)) as Success;
    }
}
