using System;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        public event BridgeNotRespondingEvent BridgeNotResponding;
        public delegate void BridgeNotRespondingEvent(object sender, EventArgs e);
    }
}
