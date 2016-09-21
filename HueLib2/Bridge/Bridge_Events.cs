using System;

namespace HueLib2
{
    public partial class Bridge
    {
        public event BridgeMessage OnMessageAdded;
        public delegate void BridgeMessage(object sender, EventArgs e);

        public event BridgeNotRespondingEvent BridgeNotResponding;
        public delegate void BridgeNotRespondingEvent(object sender, EventArgs e);
    }
}
