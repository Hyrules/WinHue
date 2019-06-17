using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;

namespace WinHueTest.Schedules
{
    [TestClass]
    public class ScheduleSerializationTests
    {
        private Bridge bridge;
        private List<Schedule> listobj;

        [TestInitialize]
        public void InitTests()
        {
            bridge = new Bridge(IPAddress.Parse("192.168.5.30"), "00:17:88:26:5f:33", "Philips hue", "30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ");
            listobj = bridge.GetListObjects<Schedule>();
        }

        [TestMethod]
        public void TestSerializationCreate()
        {
            
            
        }

        [TestMethod]
        public void TestSerializationModify()
        {
        
        }


        [TestMethod]
        public void TestDeserialization()
        {
            string schedule =
                "{\"name\":\"Work\",\"description\":\"This is my description\",\"command\":{\"address\":\"/api/2854d5c1-b8fc-4f41-8910-756fc00b6e0a/lights/2/state\",\"body\":{\"on\":true,\"bri\":254,\"hue\":65535,\"sat\":254},\"method\":\"PUT\"},\"localtime\":\"W124/T06:00:00\",\"time\":\"W124/T10:00:00\",\"created\":\"2018-12-24T14:46:20\",\"status\":\"disabled\",\"recycle\":false}";
            Schedule sc = JsonConvert.DeserializeObject<Schedule>(schedule);
            Assert.AreEqual("Work", sc.name,"Name is not equal");
            Assert.AreEqual("This is my description", sc.description,"Description is not equal");
            Assert.AreEqual("W124/T06:00:00", sc.localtime, "localtime is not equal");
            Assert.AreEqual("/api/2854d5c1-b8fc-4f41-8910-756fc00b6e0a/lights/2/state", sc.command.address, "Command Address is not equal");
            Assert.AreEqual("{\"on\":true,\"bri\":254,\"hue\":65535,\"sat\":254}", sc.command.body, "Command body is not equal");
            Assert.AreEqual("2018-12-24T14:46:20", sc.created, "Created is not equal");
            Assert.IsFalse(sc.recycle.GetValueOrDefault(), "Recycle is not eu");
            Assert.AreEqual("disabled",sc.status,"Status is not equal");
        }

    }
}
