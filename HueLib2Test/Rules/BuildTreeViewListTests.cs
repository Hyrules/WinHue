using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.Rules;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueDimmer;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.HueTap;

namespace WinHueTest.Rules
{
    [TestClass]
    public class BuildTreeViewListTests
    {
        [TestMethod]
        public void TestTreeViewBuild()
        {
            Sensor s = new Sensor();
            s.state = new ButtonSensorState();
            s.config = new HueTapSensorConfig();
            HuePropertyTreeViewItem h = BuildTree(s);              
        }

        private static HuePropertyTreeViewItem BuildTree(IHueObject obj)
        {
            HuePropertyTreeViewItem root = new HuePropertyTreeViewItem() { Header = obj.name, PropType = obj.GetType(), FontWeight = FontWeights.Normal, IsSelected = false, Address = new HueAddress($"/{obj.GetType().Name.ToLower() +"s"}") };
            PropertyInfo[] pi = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            pi = pi.Where(x => x.Name != "Id" && x.Name != "Image" && x.Name != "visible").ToArray();


            return root;
        }




    }



}
