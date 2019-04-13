using System;
using System.Net;

namespace WinHue3.Philips_Hue_2.Communication
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
