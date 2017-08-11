using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeMessages
{
    public class Messages
    {
        public Messages()
        {
            ListMessages = new List<IMessage>();
        }

        public Messages(List<IMessage> msg)
        {
            ListMessages = new List<IMessage>();
            AddMessage(msg);
        }

        public List<IMessage> ListMessages { get; internal set; }
        public bool Success => ListMessages.TrueForAll(x => x.GetType() == typeof(Success));
        public bool Error => ListMessages.TrueForAll(x => x.GetType() == typeof(Error));
        public Error LastError => ListMessages.LastOrDefault(x => x.GetType() == typeof(Error)) as Error;
        public Success LastSuccess => ListMessages.LastOrDefault(x => x.GetType() == typeof(Success)) as Success;

        public void AddMessage(List<IMessage> msg)
        {
            ListMessages.Clear();
            ListMessages.AddRange(msg);
            OnMessageAdded?.Invoke(this,null);
        }

        public void AddMessage(IMessage msg)
        {
            ListMessages.Clear();
            ListMessages.Add(msg);
            OnMessageAdded?.Invoke(this,null);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IMessage m in ListMessages)
            {
                sb.AppendLine(m.ToString());
            }
            return sb.ToString();
        }

        public event Message OnMessageAdded;
        public delegate void Message(object sender, EventArgs e);
    }
}
