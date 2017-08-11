using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.DataGrid.FilterCriteria;

namespace HueLib2
{
    public class CommandResult<T> 
    {
        private bool _success;
        private T _data;
        private Exception _exception;
        private string _message;

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        public T Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
