using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;

namespace WinHue3.Functions.Rules.Creator
{
   public static class TreeViewHelper
    {
        public static HuePropertyTreeViewItem BuildPropertiesTree(object root,  string currentpath, string name = null, string selectedpath = null)
        {
            PropertyInfo[] listprops = root.GetArrayHueProperties();
            HuePropertyTreeViewItem tvi = new HuePropertyTreeViewItem() { IsSelected = false, Path = currentpath, Name = name};

            foreach (PropertyInfo p in listprops)
            {
                string actualpath = currentpath + "/" + p.Name;
                object value = p.GetValue(root);

                if (value == null || !value.GetType().HasHueProperties())
                {

                    tvi.Childrens.Add(new HuePropertyTreeViewItem() { Name = p.Name, Path = actualpath, IsSelected = selectedpath == actualpath, IsExpanded = selectedpath == actualpath });
                }
                else
                {
                    HuePropertyTreeViewItem ttvi = BuildPropertiesTree(value, actualpath, selectedpath);
                    ttvi.Name = p.Name;
                    ttvi.Path = actualpath;
                    tvi.Childrens.Add(ttvi);

                }
            }

            return tvi;
        }

        public static HuePropertyTreeViewItem BuildPropertiesTreeFromDataStore(DataStore ds, string selectedpath = null)
        {

            HuePropertyTreeViewItem tvi = new HuePropertyTreeViewItem() { IsSelected = false};

            // LIGHTS
            HuePropertyTreeViewItem tviLights = new HuePropertyTreeViewItem() {Name = "lights", Path = "/lights", IsSelected = false};
            foreach (KeyValuePair<string, Light> l in ds.lights)
            {
                tviLights.Childrens.Add(BuildPropertiesTree(l.Value, tviLights.Path + $"/{l.Key}", $"[{l.Key}] - {l.Value.name}", selectedpath));
            }
            tvi.Childrens.Add(tviLights);

            // GROUPS
            HuePropertyTreeViewItem tviGroups = new HuePropertyTreeViewItem() { Name = "groups", Path = "/groups", IsSelected = false };
            foreach (KeyValuePair<string, Group> g in ds.groups)
            {
                tviGroups.Childrens.Add(BuildPropertiesTree(g.Value, tviGroups.Path + $"/{g.Key}", $"[{g.Key}] - {g.Value.name}", selectedpath));
            }
            tvi.Childrens.Add(tviGroups);

            // RULES
            HuePropertyTreeViewItem tviRules = new HuePropertyTreeViewItem() { Name = "rules", Path = "/rules", IsSelected = false };
            foreach (KeyValuePair<string, Rule> r in ds.rules)
            {
                tviRules.Childrens.Add(BuildPropertiesTree(r.Value, tviRules.Path + $"/{r.Key}", $"[{r.Key}] - {r.Value.name}", selectedpath));
            }
            tvi.Childrens.Add(tviRules);

            // SCHEDULES
            HuePropertyTreeViewItem tviSchedules = new HuePropertyTreeViewItem() { Name = "schedules", Path = "/schedules", IsSelected = false };
            foreach (KeyValuePair<string, Schedule> s in ds.schedules)
            {
                tviSchedules.Childrens.Add(BuildPropertiesTree(s.Value, tviSchedules.Path + $"/{s.Key}", $"[{s.Key}] - {s.Value.name}", selectedpath));
            }
            tvi.Childrens.Add(tviSchedules);

            // SCENES
            HuePropertyTreeViewItem tviScenes = new HuePropertyTreeViewItem() { Name = "scenes", Path = "/scenes", IsSelected = false };
            foreach (KeyValuePair<string, Scene> sc in ds.scenes)
            {
                tviScenes.Childrens.Add(BuildPropertiesTree(sc.Value, tviSchedules.Path + $"/{sc.Key}", $"[{sc.Key}] - {sc.Value.name}", selectedpath));
            }
            tvi.Childrens.Add(tviScenes);

            // RESOURCE LINKS
            HuePropertyTreeViewItem tviResourceLinks = new HuePropertyTreeViewItem() { Name = "ressource links", Path = "/resourcelinks", IsSelected = false };
            foreach (KeyValuePair<string, Resourcelink> rl in ds.resourcelinks)
            {
                tviResourceLinks.Childrens.Add(BuildPropertiesTree(rl.Value, tviResourceLinks.Path + $"/{rl.Key}", $"[{rl.Key}] - {rl.Value.name}", selectedpath));
            }
            tvi.Childrens.Add(tviResourceLinks);

            // CONFIG
            HuePropertyTreeViewItem tviConfig = new HuePropertyTreeViewItem() { Name = "config", Path = "/config", IsSelected = false };
            HuePropertyTreeViewItem config = BuildPropertiesTree(ds.config, "/config", selectedpath);

            foreach (HuePropertyTreeViewItem t in config.Childrens)
            {
                tviConfig.Childrens.Add(t);
            }
            tvi.Childrens.Add(tviConfig);

            return tvi;
        }

    }
}
