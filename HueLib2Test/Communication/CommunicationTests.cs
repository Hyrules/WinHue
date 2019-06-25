using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.Communication2;

namespace WinHueTest.Communication
{
    [TestClass]
    public class CommunicationTests
    {
        private string baseUri;
        private string badbaseUri;

        [TestInitialize]
        public void InitTests()
        {
            baseUri = "http://192.168.5.30/api/30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ/";
            badbaseUri = "http://192.168.5.29/api/30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ/";
            HueHttpClient.Timeout = 3000;
        }


        [TestMethod]
        public void GetTest()
        {
            HttpResult res = HueHttpClient.SendRequest( new Uri(baseUri + "lights"), WebRequestType.Get);
            Assert.IsTrue(res.Success);
        }

        [TestMethod]
        public void GetBadTest()
        {
            HttpResult res = HueHttpClient.SendRequest(new Uri(badbaseUri + "lights"), WebRequestType.Get);
            Assert.IsFalse(res.Success);
        }
    }
}
