using System;
using System.Net;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        public event BridgeNotRespondingEvent BridgeNotResponding;
        public delegate void BridgeNotRespondingEvent(object sender, BridgeNotRespondingEventArgs e);
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
}
