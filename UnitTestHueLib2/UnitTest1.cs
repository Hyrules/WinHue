using System;
using System.Configuration;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HueLib2;

namespace UnitTestHueLib2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_GetObject()
        {
            Bridge br = new Bridge(IPAddress.Parse("192.168.5.125"), "00:17:88:26:5f:33", "1.13.0", "01032318", "Philips hue","vzMCP8pBv7Jwh4LJQTAIfpqSvtL-Sg8T31XwtVX7");
            CommandResult rlight = br.GetObject<Light>("1");
            Assert.IsTrue(rlight.Success);
            CommandResult rgroup = br.GetObject<Group>("1");
            Assert.IsTrue(rgroup.Success);
            CommandResult rscene = br.GetObject<Scene>("345623456");
            Assert.IsTrue(rscene.Success);
            CommandResult rsensor = br.GetObject<Sensor>("1");
            Assert.IsTrue(rsensor.Success);
            CommandResult rsched = br.GetObject<Schedule>("1");
            Assert.IsTrue(rsched.Success);
            CommandResult rrule = br.GetObject<Rule>("1");
            Assert.IsTrue(rrule.Success);

            CommandResult rerror = br.GetObject<Light>("44");
            Assert.IsFalse(rerror.Success);
        }

        [TestMethod]
        public void Test_GetListObjects()
        {
            Bridge br = new Bridge(IPAddress.Parse("192.168.5.125"), "00:17:88:26:5f:33", "1.13.0", "01032318", "Philips hue", "vzMCP8pBv7Jwh4LJQTAIfpqSvtL-Sg8T31XwtVX7");
            CommandResult rlight = br.GetListObjects<Light>();
            Assert.IsTrue(rlight.Success);
            CommandResult rgroup = br.GetListObjects<Group>();
            Assert.IsTrue(rgroup.Success);
            CommandResult rscene = br.GetListObjects<Scene>();
            Assert.IsTrue(rscene.Success);
            CommandResult rsensor = br.GetListObjects<Sensor>();
            Assert.IsTrue(rsensor.Success);
            CommandResult rsched = br.GetListObjects<Schedule>();
            Assert.IsTrue(rsched.Success);
            CommandResult rrule = br.GetListObjects<Rule>();
            Assert.IsTrue(rrule.Success);
            
        }

        [TestMethod]
        public void Test_Setter()
        {
            Bridge br = new Bridge(IPAddress.Parse("192.168.5.125"), "00:17:88:26:5f:33", "1.13.0", "01032318", "Philips hue", "vzMCP8pBv7Jwh4LJQTAIfpqSvtL-Sg8T31XwtVX7");
            CommandResult setStatetest = br.SetState<HueLib2.Action>(new HueLib2.Action() {hue=45678}, "1");
            Assert.IsTrue(setStatetest.Success);
            CommandResult renameobj = br.RenameObject<Light>("1", "Basement 1");
        }
    }
}
