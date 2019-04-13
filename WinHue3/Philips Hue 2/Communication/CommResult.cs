using System;
using System.Net;

namespace WinHue3.Philips_Hue_2.Communication
{
    public class CommResult
    {
        public WebExceptionStatus Status;
        public string Data;
        public Exception Ex;
        public Uri Url;
    }
}
