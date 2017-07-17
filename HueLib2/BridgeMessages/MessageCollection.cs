using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HueLib2.BridgeMessages;

namespace HueLib2
{
    public class MessageCollection : Collection<IMessage> 
    {
        public EventArgs e = null;
        public MessageCollection() 
        {
            

        }

        public MessageCollection(IList<IMessage> list) : base(list)
        { 

        }

        /// <summary>
        /// List the amount of success messages.
        /// </summary>
        public int SuccessCount
        {
            get
            {
                return this.Count(s => s.GetType() == typeof(CreationSuccess)) + this.Count(s => s.GetType() == typeof(DeletionSuccess)) + this.Count(s => s.GetType() == typeof(Success));
            }
        }

        /// <summary>
        /// List the amount of failure messages.
        /// </summary>
        public int FailureCount
        {
            get
            {
                return this.Count(s => s.GetType() == typeof(Error));
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (IMessage m in this)
            {
                sb.AppendLine(m.ToString());
            }
            return sb.ToString();
        }
    }
}
