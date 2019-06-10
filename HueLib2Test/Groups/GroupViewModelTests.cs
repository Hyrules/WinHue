using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using WinHue3.Functions.Groups.Creator;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHueTest
{
    [TestClass]
    public class GroupViewModelTests
    {
        /// <summary>
        /// This test unit does not actually create a group but rather see if the ViewModel will end up giving the right values before creating the group.
        /// </summary>
        private Bridge bridge;
        private List<IHueObject> listobj;
        private GroupCreatorViewModel gcvm;
        [TestInitialize]
        public void InitTests()
        {
            bridge = new Bridge(IPAddress.Parse("192.168.5.30"), "00:17:88:26:5f:33", "Philips hue", "30jodHoH6BvouvzmGR-Y8nJfa0XTN1j8sz2tstYJ");
            listobj = bridge.GetAllObjects();
            gcvm = new GroupCreatorViewModel();
            gcvm.GroupCreator = new GroupCreatorModel();
            gcvm.GroupCreator.ListAvailableLights = new ObservableCollection<Light>(listobj.OfType<Light>().ToList());

        }

        [TestMethod]
        public void TestGroupEdit()
        {
            
            Group testgrp = listobj.OfType<Group>().FirstOrDefault(x => x.Id != "0");
            gcvm.Group = testgrp;
            Assert.IsTrue(gcvm.Group.Id == testgrp.Id, "Group ID is not equal");
            Assert.IsTrue(gcvm.Group.name == testgrp.name, "Group Name is not equal");
            CollectionAssert.AreEqual(testgrp.lights, gcvm.Group.lights, "Group lights are not equal");
            Assert.AreEqual(testgrp.@class, gcvm.Group.@class, "Group class are not equal");
            
        }    

        [TestMethod]
        public void TestClear()
        {
            Group testgrp = listobj.OfType<Group>().FirstOrDefault(x => x.Id != "0");
            gcvm.Group = testgrp;
            gcvm.ClearFieldsCommand.Execute(null);
            Assert.IsTrue(gcvm.GroupCreator.Listlights.Count == 0, "List of light has not been cleared");
            Assert.IsTrue(gcvm.GroupCreator.Name == string.Empty, "Name has not been cleared");
            Assert.IsTrue(gcvm.GroupCreator.Type == "LightGroup", "Type has not been cleared");
            Assert.IsTrue(gcvm.GroupCreator.Class == "Other", "Class has not been cleared");
        }

        [TestMethod]
        public void TestCreateGroup()
        {
            gcvm.GroupCreator.Name = "TestGroup";
            gcvm.GroupCreator.Listlights.Add(new Light() { Id = "1" });
            gcvm.GroupCreator.Listlights.Add(new Light() { Id = "5" });
            gcvm.GroupCreator.Listlights.Add(new Light() { Id = "12" });
            Assert.AreEqual("TestGroup", gcvm.Group.name, "Names are not equals");
            Assert.IsTrue(gcvm.Group.lights.Contains("1"), "Light ID 1 not present in array");
            Assert.IsTrue(gcvm.Group.lights.Contains("5"), "Light ID 5 not present in array");
            Assert.IsTrue(gcvm.Group.lights.Contains("12"), "Light ID 12 not present in array");
        }
    }
}
