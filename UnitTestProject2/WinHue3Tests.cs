using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Collections.Generic;
using HueLib;
using System.ComponentModel;
using System.Windows.Media;
using WinHue3;

namespace UnitTestProject2
{
    [TestClass]
    public class TestScheduleView
    {
             [TestMethod]
             public void TestScheduleTimeConverterConvertBack()
             {
             /*    ScheduleTimeConverter stc = new ScheduleTimeConverter();
                 DateTime dt = (DateTime)stc.ConvertBack("2015-12-31T11:35:35", typeof(DateTime), null, null);
                 Assert.IsTrue(dt.Equals(DateTime.Parse("2015-12-31 11:35:35")),"Test Normal Time");
                 dt = (DateTime)stc.ConvertBack("W000/T11:35:35", typeof(DateTime), null, null);
                 Assert.IsTrue(dt.Hour == 11 && dt.Minute == 35 && dt.Second == 35,"Test Alarm time");
                 dt = (DateTime)stc.ConvertBack("PT11:35:35", typeof(DateTime), null, null);
                 Assert.IsTrue(dt.Hour == 11 && dt.Minute == 35 && dt.Second == 35,"Test Timer time");
                 dt = (DateTime)stc.ConvertBack("PATATE", typeof(DateTime), null, null);
                 Assert.IsTrue(dt.Year == DateTime.Now.Year && dt.Month == DateTime.Now.Month && dt.Day == DateTime.Now.Day && dt.Hour == DateTime.Now.Hour && dt.Minute == DateTime.Now.Minute,"Test Invalid string time");
                 dt = (DateTime)stc.ConvertBack(null, typeof(DateTime), null, null);
                 Assert.IsTrue(dt.Year == DateTime.Now.Year && dt.Month == DateTime.Now.Month && dt.Day == DateTime.Now.Day && dt.Hour == DateTime.Now.Hour && dt.Minute == DateTime.Now.Minute,"Test Null value");*/
             }

             [TestMethod]
             public void TestScheduleTimeConverterConvert()
             {
                 //ScheduleTimeConverter stc = new ScheduleTimeConverter();
              //   String result = stc.Convert(new DateTime)

             }
             
        [TestMethod]
        public void TestScheduleViewModel()
        {
            ScheduleView sl = new ScheduleView(new Light() { Id = "2" }, "thisisanapikeyfortest");
            sl.Bri = 254;
            Assert.AreEqual(254, (int)sl.Bri, "Test Brightness set/get");
            sl.Ct = 420;
            Assert.IsTrue(sl.Ct == 420 && sl.Hue == -1, "Test CT set/get and Hue set null");
            sl.Hue = 23456D;
            Assert.IsTrue(sl.Hue == 23456D && sl.Ct == -1, "Test Hue set/get and CT set null");
            sl.Sat = 243;
            Assert.AreEqual(243, (int)sl.Sat, "Test Sat set/get");
            sl.Name = "Test";
            Assert.AreEqual("Test", sl.Name, "Test Name set/get");
            //       sl.ScheduleMask = "";
            //      Assert.AreEqual("001", sl.ScheduleMask, "Test Schedule Mask get/set");
            sl.Description = "Description Test";
            Assert.AreEqual("Description Test", sl.Description, "Test Description get/set");

            sl.Localtime = new DateTime(2015, 12, 31, 12, 34, 21);
            Assert.AreEqual(new DateTime(2015, 12, 31, 12, 34, 21), sl.Localtime, "Test Localtime set/get");
            sl.Type = 1;
            DateTime dt = new DateTime(2015, 12, 31, 12, 34, 21);
            Assert.IsTrue(dt.Hour == sl.Localtime.Hour && dt.Minute == sl.Localtime.Minute && dt.Second == sl.Localtime.Second, "Test Change Type set/get");
            sl.Type = 2;
            Assert.IsTrue(dt.Hour == sl.Localtime.Hour && dt.Minute == sl.Localtime.Minute && dt.Second == sl.Localtime.Second, "Test Change Type set/get");

            sl.X = 0.234M;
            Assert.AreEqual(0.234M, sl.X, "Test set/get X");

            sl.Y = 0.345M;
            Assert.AreEqual(0.345M, sl.Y, "Test set/get Y");

            sl.X = -0.001M;
            Assert.AreEqual(-0.001M, sl.X, "Test set null");
            Assert.AreEqual(-0.001M, sl.Y, "Test get null");

            sl.X = 0.234M;
            sl.Y = 0.345M;
            sl.Y = -0.001M;
            Assert.AreEqual(-0.001M, sl.X, "Test set null");
            Assert.AreEqual(-0.001M, sl.Y, "Test get null");

            sl.X = 0.234M;
            Assert.AreEqual(0, sl.Y, "Test set X get Y = 0");

            sl.X = -0.001M;

            sl.Y = 0.345M;
            Assert.AreEqual(0, sl.X, "Test set Y get X = 0");

            sl.Randomize = true;

            Assert.IsTrue(sl.GetSchedule().localtime.Contains("A"), "Test randomize true");

            sl.Randomize = false;
            Assert.IsFalse(sl.GetSchedule().localtime.Contains("A"), "Test randomize false");

            sl.Transitiontime = 123D;
            Assert.AreEqual(123D, sl.Transitiontime, "Test transition time set/get");

            sl.Transitiontime = -1D;
            Assert.AreEqual(-1D, sl.Transitiontime, "Test transition time null set/get");

            ScheduleView sv = new ScheduleView(new Light() { Id = "1" }, "thisisanapikeyfortest");

            Schedule ns = sv.GetSchedule();


            //   sl.Localtime = "2012-12-31 11:34:22";
            //       Assert.AreEqual("2012-12-31T11:34:22", sl.Localtime, "Test Localtime set/get");
            // sl.Type = "W";
            //     Assert.AreEqual(sl.Localtime,$@"W{sl.Type}/T11:34:22");

        }

        [TestMethod]
        public void TestScheduleViewWithInit()
        {
            Schedule sched = new Schedule()
            {
                name = "TestSchedule",
                description = "Schedule for testing",
                localtime = "PT11:55:00",
                command = new Command
                {
                    method = "PUT",
                    address = @"/lights/1/state",
                    body = new Body
                    {
                        bri = 254,
                        sat = 254,
                        hue = 0,
                    }
                },
                Id = "2"

            };

            ScheduleView sl = new ScheduleView(sched, "thisisanapikeyfortest");
            Assert.AreEqual(sched, sl.GetSchedule(), "Test Schedule Inititalisation");
            Assert.IsNull(sl.Autodelete, "Test Autodelete get null");
            sl.Autodelete = true;
            Assert.IsTrue((bool)sl.Autodelete, "Test Autodelete true");
            sl.Autodelete = false;
            Assert.IsFalse((bool)sl.Autodelete, "Test Autodelete false");
            Assert.IsTrue(sl.On, "Test On null True");
            sl.On = false;
            Assert.IsFalse(sl.On, "Test On False");

            Schedule sched2 = new Schedule()
            {
                name = "TestSchedule",
                description = "Schedule for testing",
                localtime = "W127/T11:55:00",
                command = new Command
                {
                    method = "PUT",
                    address = @"/lights/1/state",
                    body = new Body
                    {
                        bri = 254,
                        sat = 254,
                        hue = 0,
                    }
                },
                Id = "2"
            };

            ScheduleView sl2 = new ScheduleView(sched2, "thisisanapikeyfortest");
            Assert.AreEqual("Mon,Tue,Wed,Thu,Fri,Sat,Sun", sl2.ScheduleMask);
            sl2.ScheduleMask = "Mon,Tue,Wed";
            Assert.AreEqual("Mon,Tue,Wed", sl2.ScheduleMask);
            Assert.AreEqual(Visibility.Visible, sl2.IsAlarm, "Test Is Alarm get");
        }
    }

    [TestClass]
    public class TestDaylightModel
    {
        [TestMethod]
        public void TestDaylightView()
        {
            Sensor sensor = new Sensor
            {
                config = new SensorConfig() { lat = "45.5082N", @long = "75.6070W", sunriseoffset = 60, sunsetoffset = 24 },
                state = new DaylightSensorState(),
                Id = "1"
            };


            DaylightView dv = new DaylightView(sensor);
            Assert.AreEqual("45.5082N", dv.Latitude, "Test get Latitude");
            dv.Latitude = "46.1234N";
            Assert.AreEqual("46.1234N", dv.Latitude, "Test set Latitude");
            Assert.AreEqual("75.6070W", dv.Longitude, "Test get longitude");
            dv.Longitude = "74.6000W";
            Assert.AreEqual("74.6000W", dv.Longitude);
            Assert.AreEqual(60, dv.SunriseOffset, "Test get SunriseOff");
            dv.SunriseOffset = 100;
            Assert.AreEqual(100, dv.SunriseOffset, "Test set SunriseOffset");
            Assert.AreEqual(24, dv.SunsetOffset, "Test get SunsetOffset");
            dv.SunsetOffset = -24;
            Assert.AreEqual(-24, dv.SunsetOffset, "Test set SunsetOffset");

            dv.SunriseOffset = 500;
            Assert.IsFalse(dv.SunriseOffset == 500, "Test Invalid value Sunrise");

            dv.SunsetOffset = -145;
            Assert.IsFalse(dv.SunsetOffset == -145, "Test invalid value sunset");
        }
    }

    [TestClass]
    public class TestGroupViewModel
    {
        [TestMethod]
        public void TestGroupView()
        {
             /*   GroupCreatorView gc = new GroupCreatorView(new Dictionary<string, Light>() { { "1", new Light() { name = "Test1" } }, { "2", new Light() { name = "Test2" } },{ "3", new Light() {name = "Test3" } } });
                gc.SelectedLights = new List<string>() { "1", "2", "3" };
                gc.GroupName = "Patate";
                Assert.AreEqual("Patate", gc.GroupName,"Test Group name set/get");
                List<string> result = gc.SelectedLights;
                List<string> expected = new List<string>() { "1", "2", "3" };
                Assert.AreEqual(expected[0],result[0]);
                Assert.AreEqual(expected[1], result[1]);
                Assert.AreEqual(expected[2], result[2]);*/
                
        }

        [TestMethod]
        public void TestGroupViewInitGroup()
        {
          /*     GroupCreatorView gc = new GroupCreatorView(new Dictionary<string, Light>() { { "1", new Light() { name = "Test1" } }, { "2", new Light() { name = "Test2" } }, { "3", new Light() { name = "Test3" } } }, new Group() { name="Test1", lights= new List<string>() {"1","2","3" } });
               Group grp = gc.GetGroup();
               Assert.AreEqual("Test1", grp.name,"Test get group function");

               GroupCreatorView gc2 = new GroupCreatorView(new Dictionary<string, Light>() { { "1", new Light() { name = "Test1" } }, { "2", new Light() { name = "Test2" } }, { "3", new Light() { name = "Test3" } } }, new Group() { name = "Test1"});
             //  Assert.IsNotNull(gc2.SelectedLights,"Teste get null group lights");*/
        }
    }

    [TestClass]
    public class TestSensorCreatorViewModel
    {

        [TestMethod]
        public void TestSceneCreator()
        {
            SensorCreatorView scv = new SensorCreatorView();
            Assert.AreEqual(string.Empty, scv.ModelID, "Test ipsensor creation get default");
            Assert.AreEqual(string.Empty, scv.SwVersion, "Test swversion creation get default");
            Assert.AreEqual(0, scv.Type, "Test type creation get default");
            Assert.AreEqual(string.Empty, scv.Name, "Test type creation get default");

            scv.Url = "http://www.google.ca";
            Assert.AreEqual("http://www.google.ca", scv.Url, "Test set/get Url");
            scv.Url = string.Empty;
            Assert.AreNotEqual(string.Empty, scv.Url, "Test get/set invalid url");
            Assert.IsTrue(scv.HasErrors);
            scv.Url = "http://www.google.ca"; // remove the errors.

            scv.Type = 3;
            Assert.AreEqual(3, scv.Type, "Test set/get Type");
            scv.Type = 9;
            Assert.AreNotEqual(9, scv.Type, "Test invalid value Type");
            Assert.AreEqual(0, scv.Type, "Test invalid value type to default");

            scv.Name = "Test";
            Assert.AreEqual("Test", scv.Name, "Test set/get Name");
            scv.Name = string.Empty;
            //Assert.AreNotEqual(string.Empty, scv.Name, "Test set/get name invalid value");
            Assert.IsTrue(scv.HasErrors, "Test if errors has been triggered");
            scv.Name = "Test";

            scv.ModelID = "PATATE";
            Assert.AreEqual("PATATE", scv.ModelID, "Test set/get ModelID");
            scv.ModelID = string.Empty;
            //Assert.AreNotEqual(string.Empty, scv.ModelID, "Test set/get invalid ModelID");
            Assert.IsTrue(scv.HasErrors, "Test if errors has been triggered");
            scv.ModelID = "PATATE";

            scv.On = true;
            Assert.AreEqual(true, scv.On, "Test set/get On");

            scv.ManufacturerName = "TestMFGName";
            Assert.AreEqual("TestMFGName", scv.ManufacturerName, "Test get/set mfg name");

            scv.Url = "http://google.ca";
            scv.Type = 2;
            Assert.AreEqual("http://google.ca", scv.Url, "Test change type that keeps url");
            Assert.IsTrue(scv.HasUrl, "Test has url true");
        }

        [TestMethod]
        public void TestSceneCreatorWithInit()
        {
            SensorCreatorView scv = new SensorCreatorView(new Sensor()
            {
                name = "NewSensor",
                modelid = "sensormodel",
                manufacturername = "mymanufacturer",
                swversion = "1.0",
                type = "IPSENSOR",
                config = new SensorConfig() { on = true, url = "http://google.ca" },
            });

            Assert.AreEqual("NewSensor", scv.Name, "Test Name init");
            Assert.AreEqual("sensormodel", scv.ModelID, "Test Modelid init");
            Assert.AreEqual("mymanufacturer", scv.ManufacturerName, "test Manufacturer Name init");
            Assert.AreEqual("1.0", scv.SwVersion, "Test SwVersion init");
            Assert.AreEqual(0, scv.Type, "Test Type init");
            Assert.IsTrue(scv.HasUrl, "Test HasUrl init");
            Assert.IsTrue(scv.On, "test On init");
        }
        
    }

    [TestClass]
    public class TestHueLib
    {
        [TestMethod]
        public void TestCommunication()
        {
            UnitTestProject2.Communication.SendRequest(new Uri("http://192.168.5.51/meteo/"), WebRequestType.GET);
        }

        [TestMethod]
        public void TestHueIpScan()
        {
       //     Hue.OnIpScanComplete += Hue_OnIpScanComplete;
       //     Hue.ScanIpForBridge();
        }

        private void Hue_OnIpScanComplete(object sender, RunWorkerCompletedEventArgs e)
        {
       //     List<Bridge> dev = (List<Bridge>)e.Result;
        }

        [TestMethod]
        public void TestHue()
        {
            //List<Bridge> dev = Hue.AvailableBridges;
        }
    }

    [TestClass]
    public class TestHueObjectHelper
    {
        [TestMethod]
        public void TestGetLightImage()
        {
            // Test with empty string.
      /*      ImageSource img = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Off, "");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img2 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.On, "");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img3 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Unr, "");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");

            // Test with null value.
            ImageSource img4 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Off, null);
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img5 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.On, null);
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img6 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Unr, null);
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");

            // Test with known light.
            ImageSource img7 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Off, "LST001");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img8 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.On, "LST001");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img9 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Unr, "LST001");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");

            // Test with unknown light.
            ImageSource img10 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Off, "45345");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img11 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.On, "345345");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");
            ImageSource img12 = HueObjectHelper.GetImageForLight(HueObjectHelper.LightImageState.Unr, "345345");
            Assert.IsInstanceOfType(img, typeof(ImageSource), "Image not image source");*/
        }
    }

    [TestClass]
    public class TestFormsClass
    {
        [TestMethod]
        public void TestAddManualIP()
        {
   /*         Form_AddManualIp fip = new Form_AddManualIp();
            if(fip.ShowDialog() == true)
            {
                Assert.AreEqual("192.168.0.1", fip.GetIPAddress());
            }*/
        }

        [TestMethod]
        public void TestWaitDlg()
        {
   /*         Form_Wait fw = new Form_Wait();
            fw.ShowWait("Applying update please wait....", new TimeSpan(0, 0, 0, 10),null);*/

        }
    }


}
