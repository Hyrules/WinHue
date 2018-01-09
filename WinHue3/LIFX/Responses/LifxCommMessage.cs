using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinHue3.LIFX.Responses
{
    public class LifxCommMessage<T> where T : new()
    {
        private readonly T _data;
        private Exception _ex;
        private bool _error;

        public T Data => _data;
        public Exception Ex => _ex;
        public bool Error => _error;


        public LifxCommMessage(Exception ex, T data, bool error)
        {
            _error = error;
            _ex = ex;
            _data = data;
        }

        
    }
}
