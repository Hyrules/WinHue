using System;
using System.Net;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;

namespace WinHue3.Philips_Hue.BridgeObject
{
    public partial class Bridge
    {
        public event BridgeNotRespondingEvent BridgeNotResponding;
        public delegate void BridgeNotRespondingEvent(object sender, EventArgs e);
    }
}
