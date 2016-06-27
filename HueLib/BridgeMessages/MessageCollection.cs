using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HueLib
{
    public class MessageCollection : Collection<Message>
    {
        public EventArgs e = null;
        public MessageCollection()
        {

        }

        public MessageCollection(IList<Message> list) : base(list)
        {
     
        }

        protected override void ClearItems()
        {
            
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, Message item)
        {
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, Message item)
        {
            base.SetItem(index, item);
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
            foreach (Message m in this)
            {
                sb.AppendLine(m.ToString());
            }
            return sb.ToString();
        }
    }
}
