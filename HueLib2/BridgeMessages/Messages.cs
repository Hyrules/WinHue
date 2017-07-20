using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HueLib2.BridgeMessages
{
    [JsonConverter(typeof(MessagesConverter))]
    public class Messages
    {
        private List<Success> _success;
        private List<Error> _errors;

        public Messages()
        {
            _success = new List<Success>();
            _errors = new List<Error>();
        }

        public List<Success> SuccessMessages
        {
            get { return _success; }
            internal set { _success = value; }
        }

        public List<Error> ErrorMessages
        {
            get { return _errors; }
            internal set { _errors = value; }
        }

        public bool AnyErrors => _errors.Count > 0;
        public bool AnySuccess => _success.Count > 0;

        public bool AllErrors => _errors.Count > 0 && _success.Count == 0;
        public bool AllSuccess => _errors.Count == 0 && _success.Count > 1;

        public int Count => SuccessMessages.Count + ErrorMessages.Count;
        
    }
}
