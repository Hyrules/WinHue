using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;

namespace WinHueTest.Rules
{
    [TestClass]
    public class RuleSerializationTests
    {
        private Bridge bridge;
        private List<Rule> listobj;

        [TestInitialize]
        public void InitTests()
        {
            bridge = new Bridge(IPAddress.Parse("192.168.5.30"), "00:17:88:26:5f:33", "Philips hue", "30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ");
            listobj = bridge.GetListObjects<Rule>();
        }

        [TestMethod]
        public void TestSerialization()
        {

        }

        [TestMethod]
        public void TestDeserialization()
        {
            string test =
                "{\n        \"name\": \"Tap 2.1 Bedroom Off\",\n        \"owner\": \"none\",\n        \"created\": \"2015-11-07T21:59:03\",\n        \"lasttriggered\": \"2019-06-14T15:12:10\",\n        \"timestriggered\": 9,\n        \"status\": \"enabled\",\n        \"recycle\": false,\n        \"conditions\": [\n            {\n                \"address\": \"/sensors/2/state/buttonevent\",\n                \"operator\": \"eq\",\n                \"value\": \"34\"\n            },\n            {\n                \"address\": \"/sensors/2/state/lastupdated\",\n                \"operator\": \"dx\"\n            }\n        ],\n        \"actions\": [\n            {\n                \"address\": \"/groups/0/action\",\n                \"method\": \"PUT\",\n                \"body\": {\n                    \"scene\": \"aXbLRnAPyYIFfvt\"\n                }\n            }\n        ]\n    }";

            Rule rule = JsonConvert.DeserializeObject<Rule>(test);

        }
    }
}
