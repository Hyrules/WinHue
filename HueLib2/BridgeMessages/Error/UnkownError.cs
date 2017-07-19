using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HueLib2.BridgeMessages;

namespace HueLib2
{
    public class UnkownError
    {
        public UnkownError(CommResult comres)
        {
            status = comres.status;
            description = comres.data;
        }
        public WebExceptionStatus status { get; set; }
        public string description { get; set; }
    }
}
