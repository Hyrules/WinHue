using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Utils;
using WinHue3.Functions.Rules.Creator;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using Org.BouncyCastle.Utilities;
using WinHue3.Philips_Hue.BridgeObject.Entertainment_API;
using WinHue3.Functions.Animations2;
using WinHue3.Functions.Converters;
using WinHue3.Functions.Rules;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using System.Net.Http;

namespace WinHueTest
{
    [TestClass]
    public class WinHueMiscTests
    {
        [TestMethod]
        public void TestHueObject()
        {
            HueObject<Light> light = new HueObject<Light>();
            Type t = light.Type;

        }


        [TestMethod]
        public void HttpGetTest()
        {
            HttpClient client = new HttpClient
            {
                Timeout = new TimeSpan(0, 0, 0, 3000),
                BaseAddress = new Uri("http://banane/api/1234567890/")
            };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response;
            try
            {
                response = client.GetAsync("lights").Result;
                if (response.IsSuccessStatusCode)
                {
                    Task<string> task = response.Content.ReadAsStringAsync();
                    task.RunSynchronously();
                    string data = task.Result;
                }
                else
                {

                }
            }
            catch(WebException ex)
            {
                Assert.Fail(ex.Message.ToString());
            }


        }




        [TestMethod] 
        public void TimeSpanTest()
        { 
            TimeSpanToUShortConverter tsc = new TimeSpanToUShortConverter();
            TimeSpan ts = (TimeSpan)tsc.Convert((ushort)65535, typeof(TimeSpan),null, CultureInfo.InvariantCulture);
            Assert.IsTrue(ts.TotalMilliseconds == 6553500);
            ushort us = (ushort) tsc.ConvertBack(new TimeSpan(0, 1, 49, 13, 500), typeof(ushort), null, CultureInfo.InvariantCulture);
            Assert.IsTrue(us == 65535);
        }


        [TestMethod]
        public void TestRegex()
        {
            Regex timerRegex = new Regex(@"(R(\d\d)//?)?PT(\d\d:\d\d:\d\d)A?(\d\d:\d\d:\d\d)?");
            Match mc = timerRegex.Match("PT00:12:34");
            Assert.IsTrue(mc.Length >= 1);
            Regex alarmRegex = new Regex(@"(W(\d\d\d)//?)?T(\d\d:\d\d:\d\d)(A(\d\d:\d\d:\d\d))?");
            Match mc2 = alarmRegex.Match("W064/T11:22:33");
            Assert.IsTrue(mc2.Length >= 1);
            Regex scheduleRegex = new Regex(@"(\d\d\d\d-\d\d-\d\dT\d\d:\d\d:\d\d)A?(\d\d:\d\d:\d\d)?");
            Match mc3 = scheduleRegex.Match("2013-12-31T00:11:33:55");
            Assert.IsTrue(mc3.Length >= 1);
        }

        [TestMethod]
        public void TestDTLS()
        {
           /*
            IPEndPoint LocalEP = new IPEndPoint(IPAddress.Any, 2100);
            IPEndPoint RemoteEP = new IPEndPoint(IPAddress.Parse("192.168.5.30"),2100);
            Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
            socket.Bind(LocalEP);
            
            BasicTlsPskIdentity tlsid = new BasicTlsPskIdentity("0jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ", HexStringToByteArray("C4D0375C64A9FD05EDE7841805F2DB47"));
            HueDtlsClient hdtls = new HueDtlsClient(tlsid,null);
            hdtls.NotifyHandshakeComplete();
            DtlsClientProtocol dtlsClientProtocol = new DtlsClientProtocol(new SecureRandom());
            socket.Connect(RemoteEP);
            HueDatagramTransport hdgt = new HueDatagramTransport(socket);
            DtlsTransport tr = dtlsClientProtocol.Connect(hdtls, hdgt);
            */
        }

        public static byte[] HexStringToByteArray(string hex) {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex) {
            int val = (int)hex;
            //For uppercase A-F letters:
            return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            //return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        [TestMethod]
        public void TestMethod1()
        {
           /* string msg =
                "[{\"success\":{\"id\":\"1\"}},{\"success\":{\"/schedules/1/command\":{\"address\":\"/api/ArPgZTu1l1n4i3bLKdIhrtDSY4nLxLxMNh0Q8BqQ/lights/2/state\",\"method\":\"PUT\",\"body\":{\"bri\":254,\"hue\":65535,\"sat\":254,\"on\":false}}}},{\"success\":\"/groups/1/deleted\"}]";
*/
            // TEST ABSOLUTE TIME
            Assert.IsTrue(TestTimestamp("2017-01-01T23:59:59"));
            Assert.IsTrue(TestTimestamp("2017-01-01T23:59:59A11:59:59"));
            Assert.IsFalse(TestTimestamp("2017-01-01T23:59:60"));
            Assert.IsFalse(TestTimestamp("2017-01-01"));
            Assert.IsFalse(TestTimestamp("2017-01-01A23:59:59"));

            // TEST INTERVALS

            Assert.IsTrue(TestTimestamp("W127/T23:59:59"));
            Assert.IsTrue(TestTimestamp("W127/T23:59:59/T23:59:59"));
            Assert.IsTrue(TestTimestamp("T23:59:59/T23:59:59"));
            Assert.IsFalse(TestTimestamp("T23:59:59"));
            Assert.IsFalse(TestTimestamp("W128/T23:59:59"));
            Assert.IsFalse(TestTimestamp("W127/T24:59:59/T23:59:59"));
            Assert.IsFalse(TestTimestamp("T23:60:59/T23:59:59"));

            // TEST TIMERS

            //ColorTemperature ct = 560;

        }

        private static bool TestTimestamp(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            try
            {
                //[xxx] items are optional
                // MATCH FOR TIME INTEVALS [W127/]T23:59:59/T23:59:59
                Regex r = new Regex(@"(W([0-1]2[0-7]|[0-1][0-1][0-9])/)?T([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)/T([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)");
                Match m = r.Match(value);
                if (m.Success) return true;

                // MATCH FOR RECURRING TIMES W126/T23:59:59[A11:59:59]
                r = new Regex(@"(W([0-1]2[0-7]|[0-1][0-1]\d)/)T([0-1]?\d|2[0-3]):([0-5]\d):([0-5]\d)(A(0\d|1[0-1]):([0-5]\d):([0-5]?\d))?");
                m = r.Match(value);
                if (m.Success) return true;

                // MATCH FOR TIMERS [R[nn]/]PT23:59:59[A:11:59:59]
                r = new Regex(@"(R(0\d|[1-9]\d)/)?PT([0-1]\d|[0-2][0-3]):([0-5]\d):([0-5]\d)(A(0\d|1[0-1]):([0-5]\d):([0-5]\d))?");
                m = r.Match(value);
                if (m.Success) return true;

                // MATCH FOR ABSOLUTE TIME YYYY-MM-DDT23:59:59[A11:59:59]
                r = new Regex(@"(\d\d\d\d)-(0\d|1[0-2])-([0-2]\d|3[0-1])T([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)(A(0\d|1[0-1]):([0-5]\d):([0-5]\d))?");
                m = r.Match(value);
                if (m.Success) return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}


