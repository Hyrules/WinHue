using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WinHue3.Functions.Rules.Creator;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.RuleObject;

namespace WinHueTest
{
    [TestClass]
    public class RuleViewModelTests
    {
        private Bridge bridge;
        private List<Rule> listobj;
        private RuleCreatorViewModel rcvm;

        [TestInitialize]
        public void InitTests()
        {
            bridge = new Bridge(IPAddress.Parse("192.168.5.30"), "00:17:88:26:5f:33", "Philips hue", "30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ");
            listobj = bridge.GetListObjects<Rule>();
            rcvm = new RuleCreatorViewModel();
            _ = rcvm.Initialize(bridge);
        }

        [TestMethod]
        public void TestRuleEdit()
        {
            Rule testrule = listobj.FirstOrDefault(x => x.Id == "5");
            rcvm.Rule = testrule;            
            Assert.AreEqual(testrule.name, rcvm.Rule.name, "Rule name is not equal");
            Assert.AreEqual(testrule.status, rcvm.Rule.status, "Rule status is not equal");
            CollectionAssert.AreEqual(testrule.conditions, rcvm.Rule.conditions, "Rule conditions are not equals");
            
        }

    }
}
