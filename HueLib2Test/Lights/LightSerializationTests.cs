using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHueTest.Lights
{
    [TestClass]
    public class LightSerializationTests
    {
        private Bridge bridge;
        private List<Light> listobj;

        [TestInitialize]
        public void InitTests()
        {
            bridge = new Bridge(IPAddress.Parse("192.168.5.30"), "00:17:88:26:5f:33", "Philips hue", "30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ");
            listobj = bridge.GetListObjects<Light>();
        }

        [TestMethod]
        public void TestDeserialization()
        {

        }

    }
}
