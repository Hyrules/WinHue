using System;
using System.Net;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        public event BridgeNotRespondingEvent BridgeNotResponding;
        public delegate void BridgeNotRespondingEvent(object sender, BridgeNotRespondingEventArgs e);

        public event BridgeAccessDeniedEvent BridgeAccessDenied;
        public delegate void BridgeAccessDeniedEvent(object sender, BridgeAccessDeniedEventArgs e);
    }

    public class BridgeNotRespondingEventArgs : EventArgs
    {
        public WebExceptionStatus Exception { get; private set; }
        public string Url { get; private set; }
        public Bridge Bridge { get; private set; }

        public BridgeNotRespondingEventArgs(Bridge bridge,string url, WebExceptionStatus ex)
        {
            Bridge = bridge;
            Exception = ex;
            Url = url;
        }
    }

    public class BridgeAccessDeniedEventArgs : EventArgs
    {
        public string Url { get; private set; }
        public Bridge Bridge { get; private set; }

        public BridgeAccessDeniedEventArgs(Bridge bridge, string url)
        {
            Url = url;
            Bridge = bridge;
        }
    }
}
