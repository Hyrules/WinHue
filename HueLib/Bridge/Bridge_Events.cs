using System;
using System.CodeDom;

namespace HueLib
{
    public partial class Bridge
    {
        public event BridgeMessage OnMessageAdded;
        public delegate void BridgeMessage(object sender, EventArgs e);

        public event BridgeNotRespondingEvent BridgeNotResponding;
        public delegate void BridgeNotRespondingEvent(object sender, EventArgs e);
    }
}
