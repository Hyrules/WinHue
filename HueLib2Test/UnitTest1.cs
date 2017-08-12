using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Newtonsoft.Json;
using WinHue3.Addons;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SensorObject;
using WinHue3.Philips_Hue.HueObjects.SensorObject.Daylight;
using WinHue3.Utils;

namespace HueLib2Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void FeedTest()
        {
            RssFeedMonitor fm = new RssFeedMonitor();
            Monitor mon = new Monitor(){Name="Severe Weather", url = "http://meteo.gc.ca/rss/battleboard/qc68_f.xml", Interval = 60};
            fm.AddMonitor(mon);
            fm.StartMonitor("Severe Weather");

        }

        [TestMethod]
        public void TestDecimalArray()
        {
            State s = new State();
            s.xy = new decimal[] {0.987m, 0.324m};
            object obj = Serializer.SerializeToJson(s.xy);
            bool test = obj is Array;
            string xy = string.Join(",", s.xy);


        }


        [TestMethod]
        public void TestSensors()
        {

            string ssensor =
                "{\n    \"state\": {\n        \"daylight\": true,\n        \"lastupdated\": \"2017-08-09T09:56:00\"\n    },\n    \"config\": {\n        \"on\": true,\n        \"configured\": true,\n        \"sunriseoffset\": 0,\n        \"sunsetoffset\": 0\n    },\n    \"name\": \"Daylight\",\n    \"type\": \"Daylight\",\n    \"modelid\": \"PHDL00\",\n    \"manufacturername\": \"Philips\",\n    \"swversion\": \"1.0\"\n}";
           // ISensor obj = JsonConvert.DeserializeObject<ISensor>(ssensor);
            DaylightSensor ds = new DaylightSensor();
            PropertyInfo[] props = ds.GetType().GetHueProperties();

            //string json = JsonConvert.SerializeObject(obj);
        }


        [TestMethod]
        public void TestBuildTree()
        {
          
            


            //BridgeSettings bs = new BridgeSettings();
          //  Light l = new Light();
            BridgeSettings bs = new BridgeSettings();
        //    SwUpdate sw = new SwUpdate();
        //    IHueObject obj = new Light();
         //   Sensor ts = new Sensor(){config = new ClipGenericFlagSensorConfig(), state = new ClipGenericFlagSensorState()};
            PropertyInfo[] props = bs.GetType().GetHueProperties();
             //BuildTree(l.GetType(), "/lights/1", "");

            string ssensor =
                "{\n    \"state\": {\n        \"daylight\": true,\n        \"lastupdated\": \"2017-08-09T09:56:00\"\n    },\n    \"config\": {\n        \"on\": true,\n        \"configured\": true,\n        \"sunriseoffset\": 0,\n        \"sunsetoffset\": 0\n    },\n    \"name\": \"Daylight\",\n    \"type\": \"Daylight\",\n    \"modelid\": \"PHDL00\",\n    \"manufacturername\": \"Philips\",\n    \"swversion\": \"1.0\"\n}";
            //Sensor obj = Serializer.DeserializeToObject<Sensor>(ssensor);
           /* PropertyInfo[] props = obj.GetType().GetHueProperties();
           */
            
            foreach (PropertyInfo pi in props)
            {

           //     rtvi.Add(BuildTree(pi,"/config"));
            }
            
        }

        private static TreeViewItem BuildTree(PropertyInfo root, string currentpath, string selectedpath = null)
        {
            var toVisit = new Stack<PropertyInfo>();
            var toVisitRtvi = new Stack<TreeViewItem>();
            var visitedAncestors = new Stack<PropertyInfo>();
            var pathstack = new Stack<string>();
            var actualpath = currentpath + "/" + root.Name;
            var rtvi = new TreeViewItem() { Header = root.Name, Tag = actualpath };

            toVisit.Push(root);
            while (toVisit.Count > 0)
            {

                var node = toVisit.Peek();
                if (node.PropertyType.GetHueProperties().Length > 0)
                {


                    if (visitedAncestors.PeekOrDefault() != node)
                    {
                        visitedAncestors.Push(node);
                        toVisit.PushReverse(node.PropertyType.GetHueProperties().ToList());

                        if (root.Name != node.Name)
                        {
                            pathstack.Push(actualpath);
                            actualpath = actualpath + "/" + node.Name;
                            toVisitRtvi.Push(rtvi);
                            rtvi = new TreeViewItem() { Header = node.Name, Tag = actualpath };
                        }
                        continue;
                    }
                    visitedAncestors.Pop();
                    if (toVisitRtvi.Count == 0) return rtvi;
                    TreeViewItem currtvi = toVisitRtvi.Pop();
                    currtvi.Items.Add(rtvi);
                    actualpath = pathstack.Pop();
                    currtvi.Tag = actualpath;
                    rtvi = currtvi;
                    toVisit.Pop();
                    continue;
                }


                if (visitedAncestors.Count > 0)
                {
                    TreeViewItem nrtvi = new TreeViewItem() { Header = node.Name, Tag = actualpath + "/" + node.Name };
                    if (selectedpath != null && nrtvi.Tag.ToString().Equals(selectedpath)) nrtvi.IsSelected = true;
                    rtvi.Items.Add(nrtvi);
                    rtvi.IsExpanded = true;
                    if (nrtvi.IsSelected)
                        nrtvi.BringIntoView();
                }

                toVisit.Pop();
            }

            return rtvi;
        }

        /*   private List<RuleTreeViewItem> BuildTree(Type type, string currentPath, string propname, string selectedpath = null)
           {
               List<RuleTreeViewItem> listtree = new List<RuleTreeViewItem>();
               PropertyInfo[] prop = type.GetHueProperties();

               Queue<PropertyInfo> props = new Queue<PropertyInfo>();
               Stack<RuleTreeViewItem> tvi = new Stack<RuleTreeViewItem>();
               Stack<PropertyInfo> currentProps = new Stack<PropertyInfo>();
               foreach (PropertyInfo pi in prop)
               {
                   props.Enqueue(pi);

               }

               RuleTreeViewItem rtvi = new RuleTreeViewItem();

               while (props.Count > 0)
               {
                   PropertyInfo currentprop = props.Dequeue();
                   PropertyInfo[] subprop = currentprop.PropertyType.GetHueProperties();


                   foreach (PropertyInfo pi in subprop)
                   {
                       props.Enqueue(pi);
                   }



                   if (subprop.Length > 0)
                   {
                       tvi.Push(new RuleTreeViewItem() { Header = currentprop.Name });                    
                   }

                   listtree.Add(rtvi);



                   //if (subprop.Length > 0)
                   //{
                   //    currentRtvi = rtvi.Pop();

                   //    foreach (PropertyInfo pi in subprop)
                   //    {
                   //        rtvi.Push(new RuleTreeViewItem() { Header = pi.Name, Path = currentRtvi.Path + "/" + pi.Name });
                   //    }
                   //}

                   //if (currentRtvi == null)
                   //{
                   //    listtree.Add(rtvi.Pop());
                   //}
                   //else
                   //{
                   //    currentRtvi.Items.Add(rtvi.Pop());
                   //    if (rtvi.Count != 0) continue;
                   //    listtree.Add(currentRtvi);
                   //    currentRtvi = null;
                   //}

               }


               return listtree;
           }*/

        [TestMethod]
        public void TestMethod1()
        {
            string msg =
                "[{\"success\":{\"id\":\"1\"}},{\"success\":{\"/schedules/1/command\":{\"address\":\"/api/ArPgZTu1l1n4i3bLKdIhrtDSY4nLxLxMNh0Q8BqQ/lights/2/state\",\"method\":\"PUT\",\"body\":{\"bri\":254,\"hue\":65535,\"sat\":254,\"on\":false}}}},{\"success\":\"/groups/1/deleted\"}]";

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


