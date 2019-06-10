using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.Communication;

namespace WinHueTest.Groups
{
    [TestClass]
    public class GroupSerializationTests
    {
        private Bridge bridge;
        private List<Group> listobj;

        [TestInitialize]
        public void InitTests()
        {
            bridge = new Bridge(IPAddress.Parse("192.168.5.30"), "00:17:88:26:5f:33", "Philips hue", "30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ");
            listobj = bridge.GetListObjects<Group>();
        }


        [TestMethod]
        public void TestSerialization()
        {
            string result = "{\"lights\":[\"5\",\"16\",\"15\"],\"name\":\"PC Room\",\"type\":\"LightGroup\",\"sensors\":[]}";
            string group = Serializer.CreateJsonObject(listobj[0]);
            Assert.AreEqual(result, group,"Serialized group are not the same");

        }

        [TestMethod]
        public void TestDeserialization()
        {
          
            Assert.IsNotNull(bridge.GetObject<Group>("0"),"Deserialization failed");

        }
    }
}
