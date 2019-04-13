using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinHue3.Philips_Hue_2.BridgeMessages
{
    public class Messages
    {
        public Messages()
        {
            ListMessages = new List<IMessage>();
        }

        public Messages(List<IMessage> msgs)
        {
            ListMessages = new List<IMessage>();
            AddMessage(msgs);
        }

        public List<IMessage> ListMessages { get; private set; }
        public bool Success => ListMessages.TrueForAll(x => x.GetType() == typeof(Success));
        public bool Error => ListMessages.TrueForAll(x => x.GetType() == typeof(Philips_Hue.BridgeObject.BridgeMessages.Error));
        public Error LastError => ListMessages.LastOrDefault(x => x.GetType() == typeof(Error)) as Error;
        public Success LastSuccess => ListMessages.LastOrDefault(x => x.GetType() == typeof(Success)) as Success;

        public void AddMessage(List<IMessage> msg)
        {
            if (msg == null) return;
            ListMessages.Clear();
            ListMessages.AddRange(msg);
            OnMessageAdded?.Invoke(this,new MessageAddedEventArgs(msg));
        }

        public void AddMessage(IMessage msg)
        {
            if (msg == null) return;
            ListMessages.Clear();
            ListMessages.Add(msg);
            OnMessageAdded?.Invoke(this,new MessageAddedEventArgs(msg));
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
        public delegate void Message(object sender, MessageAddedEventArgs e);
    }

    public class MessageAddedEventArgs : EventArgs
    {
        public List<IMessage> Messages { get; private set; }

        public MessageAddedEventArgs(List<IMessage> newmsgs)
        {
            Messages = new List<IMessage>();
            if (newmsgs == null) return;
            Messages.AddRange(newmsgs);
        }

        public MessageAddedEventArgs(IMessage newmsg)
        {
            Messages = new List<IMessage>();
            if (newmsg == null) return;
            Messages.Add(newmsg);
        }
    }
}
