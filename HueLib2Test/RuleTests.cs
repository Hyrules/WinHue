using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using WinHue3.Functions.Rules.Creator;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHueTest
{
    [TestClass]
    public class RuleTests
    {
        private Bridge bridge;
        private List<IHueObject> listobj;

        [TestInitialize]
        public void InitTests()
        {
            bridge = new Bridge(IPAddress.Parse("192.168.5.30"), "00:17:88:26:5f:33", "Philips hue", "30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ");
            listobj = bridge.GetAllObjects();
        }

        [TestMethod]
        public void TestRuleCreator()
        {
            RuleCreatorViewModel rcvm = new RuleCreatorViewModel();
            rcvm.Initialize(bridge);
            Assert.IsNotNull(rcvm.ListHueObjects,"List of Hue Objects is null");

        }

    }
}
