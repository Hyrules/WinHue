using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    public class WebClientTimeout : WebClient
    {
        public int Timeout { get; set; }

        public WebClientTimeout() : this(3000) { }

        public WebClientTimeout(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest) base.GetWebRequest(address);
            if (request != null)
            {
                request.ServicePoint.ConnectionLimit = 20;
                request.Timeout = this.Timeout;
                request.ServicePoint.Expect100Continue = false;
            }
            return (WebRequest)request;
        }

    }


}
