using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using Xceed.Wpf.DataGrid.Views;

namespace WinHue3.Functions.Rules.Creator
{
   public static class TreeViewHelper
    {
        public static HuePropertyTreeViewItem BuildPropertiesTree(object root,  string currentpath, string name = null, string selectedpath = null)
        {
            PropertyInfo[] listprops = root.GetArrayHueProperties();
            HuePropertyTreeViewItem tvi = new HuePropertyTreeViewItem() { IsSelected = false, Address = new HueAddress(currentpath), Header = name, PropType = root.GetType(), FontWeight = FontWeights.Normal};

            foreach (PropertyInfo p in listprops)
            {
                string actualpath = currentpath + "/" + p.Name;
                object value = p.GetValue(root);

                if (value == null || !value.GetType().HasHueProperties())
                {

                    tvi.Items.Add(new HuePropertyTreeViewItem()
                    {
                        Header = p.Name,
                        Address = new HueAddress(actualpath),
                        IsSelected = selectedpath == actualpath,
                        IsExpanded = selectedpath == actualpath,
                        PropType = p.PropertyType,
                        FontWeight = FontWeights.Normal,
                        FontStyle = FontStyles.Normal
                    });
                }
                else
                {
                    HuePropertyTreeViewItem ttvi = BuildPropertiesTree(value, actualpath, selectedpath);
                    ttvi.Header = p.Name;
                    ttvi.Address = new HueAddress(actualpath);
                    tvi.Items.Add(ttvi);


                }
            }

            return tvi;
        }


        public static HuePropertyTreeViewItem BuildObjectTreeFromDataStore(DataStore ds, string selectedpath = null)
        {
            HuePropertyTreeViewItem tvi = new HuePropertyTreeViewItem() { IsSelected = false, Header = $"{ds.config.name} [{ds.config.ipaddress}]", Address = new HueAddress(""), PropType = ds.GetType()};

            // LIGHTS
            HuePropertyTreeViewItem tviLights = new HuePropertyTreeViewItem() { Header = "lights", Address = new HueAddress("/lights"), IsSelected = false, PropType = ds.lights.GetType()};
            foreach (KeyValuePair<string, Light> l in ds.lights)
            {
                tviLights.Items.Add(new HuePropertyTreeViewItem()
                {
                    Header = WinHueSettings.settings.ShowID ? $"[{l.Key}] - {l.Value.name}" : $"{l.Value.name}" ,
                    Address = new HueAddress(tviLights.Address + $"/{l.Key}"),
                    IsSelected = selectedpath == tviLights.Address + $"/{l.Key}",
                    PropType = l.Value.GetType()
                });
            }
            tvi.Items.Add(tviLights);
            
            // GROUPS
            HuePropertyTreeViewItem tviGroups = new HuePropertyTreeViewItem() { Header = "groups", Address = new HueAddress("/groups"), IsSelected = false };
            foreach (KeyValuePair<string, Group> g in ds.groups)
            {
                tviGroups.Items.Add(new HuePropertyTreeViewItem()
                {
                    Header = WinHueSettings.settings.ShowID ? $"[{g.Key}] - {g.Value.name}" : $"{g.Value.name}",
                    Address = new HueAddress(tviGroups.Address + $"/{g.Key}"),
                    IsSelected = selectedpath == tviGroups.Address + $"/{g.Key}",
                    PropType = g.Value.GetType()
                });
            }
            tvi.Items.Add(tviGroups);

            // RULES
            HuePropertyTreeViewItem tviRules = new HuePropertyTreeViewItem() { Header = "rules", Address = new HueAddress("/rules"), IsSelected = false };
            foreach (KeyValuePair<string, Rule> r in ds.rules)
            {
                tviRules.Items.Add(new HuePropertyTreeViewItem()
                {
                    Header = WinHueSettings.settings.ShowID ? $"[{r.Key}] - {r.Value.name}" : $"{r.Value.name}",
                    Address = new HueAddress(tviRules.Address + $"/{r.Key}"),
                    IsSelected = selectedpath == tviRules.Address + $"/{r.Key}",
                    PropType = r.Value.GetType()
                });
            }
            tvi.Items.Add(tviRules);

            // SCHEDULES
            HuePropertyTreeViewItem tviSchedules = new HuePropertyTreeViewItem() { Header = "schedules", Address = new HueAddress("/schedules"), IsSelected = false };
            foreach (KeyValuePair<string, Schedule> s in ds.schedules)
            {
                tviSchedules.Items.Add(new HuePropertyTreeViewItem()
                {
                    Header = WinHueSettings.settings.ShowID ? $"[{s.Key}] - {s.Value.name}" : $"{s.Value.name}",
                    Address = new HueAddress(tviSchedules.Address + $"/{s.Key}"),
                    IsSelected = selectedpath == tviSchedules.Address + $"/{s.Key}",
                    PropType = s.Value.GetType()
                });
            }
            tvi.Items.Add(tviSchedules);

            // SCENES
            HuePropertyTreeViewItem tviScenes = new HuePropertyTreeViewItem() { Header = "scenes", Address = new HueAddress("/scenes"), IsSelected = false };
            foreach (KeyValuePair<string, Scene> sc in ds.scenes)
            {
                if (!WinHueSettings.settings.ShowHiddenScenes && sc.Value.name.StartsWith("HIDDEN")) continue;
                tviScenes.Items.Add(new HuePropertyTreeViewItem()
                {
                    Header = WinHueSettings.settings.ShowID ? $"[{sc.Key}] - {sc.Value.name}" : $"{sc.Value.name}",
                    Address = new HueAddress(tviScenes.Address + $"/{sc.Key}"),
                    IsSelected = selectedpath == tviScenes.Address + $"/{sc.Key}",
                    PropType = sc.Value.GetType()
                });
            }
            tvi.Items.Add(tviScenes);

            // RESOURCE LINKS
            HuePropertyTreeViewItem tviResourceLinks = new HuePropertyTreeViewItem() { Header = "ressource links", Address = new HueAddress("/resourcelinks"), IsSelected = false };
            foreach (KeyValuePair<string, Resourcelink> rl in ds.resourcelinks)
            {
                tviResourceLinks.Items.Add(new HuePropertyTreeViewItem()
                {
                    Header = WinHueSettings.settings.ShowID ? $"[{rl.Key}] - {rl.Value.name}" : $"{rl.Value.name}",
                    Address = new HueAddress(tviResourceLinks.Address + $"/{rl.Key}"),
                    IsSelected = selectedpath == tviResourceLinks.Address + $"/{rl.Key}",
                    PropType = rl.Value.GetType()
                });
            }
            tvi.Items.Add(tviResourceLinks);

            //SENSORS
            HuePropertyTreeViewItem tviSensors = new HuePropertyTreeViewItem() { Header = "sensors", Address = new HueAddress("/sensors"), IsSelected = false };
            foreach (KeyValuePair<string, Sensor> sn in ds.sensors)
            {
                tviSensors.Items.Add(new HuePropertyTreeViewItem()
                {
                    Header = WinHueSettings.settings.ShowID ? $"[{sn.Key}] - {sn.Value.name}" : $"{sn.Value.name}",
                    Address = new HueAddress(tviSensors.Address + $"/{sn.Key}"),
                    IsSelected = selectedpath == tviSensors.Address + $"/{sn.Key}",
                    PropType = sn.Value.GetType()
                });
            }
            tvi.Items.Add(tviSensors);

            return tvi;
        }

        public static HuePropertyTreeViewItem BuildPropertiesTreeFromDataStore(DataStore ds, string selectedpath = null)
        {

            HuePropertyTreeViewItem tvi = new HuePropertyTreeViewItem() { IsSelected = false, Header = $"{ds.config.name} [{ds.config.ipaddress}]", Address = new HueAddress(""), FontStyle = FontStyles.Italic};

            // LIGHTS
            HuePropertyTreeViewItem tviLights = new HuePropertyTreeViewItem() {Header = "lights", Address = new HueAddress("/lights"), IsSelected = false, FontStyle = FontStyles.Italic };
            foreach (KeyValuePair<string, Light> l in ds.lights)
            {
                tviLights.Items.Add(BuildPropertiesTree(l.Value, tviLights.Address + $"/{l.Key}", WinHueSettings.settings.ShowID ? $"[{l.Key}] - {l.Value.name}" : $"{l.Value.name}", selectedpath));
            }
            tvi.Items.Add(tviLights);

            // GROUPS
            HuePropertyTreeViewItem tviGroups = new HuePropertyTreeViewItem() { Header = "groups", Address = new HueAddress("/groups"), IsSelected = false, FontStyle = FontStyles.Italic };
            foreach (KeyValuePair<string, Group> g in ds.groups)
            {
                tviGroups.Items.Add(BuildPropertiesTree(g.Value, tviGroups.Address + $"/{g.Key}", WinHueSettings.settings.ShowID ? $"[{g.Key}] - {g.Value.name}" : $"{g.Value.name}", selectedpath));
            }
            tvi.Items.Add(tviGroups);

            // RULES
            HuePropertyTreeViewItem tviRules = new HuePropertyTreeViewItem() { Header = "rules", Address = new HueAddress("/rules"), IsSelected = false, FontStyle = FontStyles.Italic };
            foreach (KeyValuePair<string, Rule> r in ds.rules)
            {
                tviRules.Items.Add(BuildPropertiesTree(r.Value, tviRules.Address + $"/{r.Key}", WinHueSettings.settings.ShowID ? $"[{r.Key}] - {r.Value.name}" : $"{r.Value.name}", selectedpath));
            }
            tvi.Items.Add(tviRules);

            // SCHEDULES
            HuePropertyTreeViewItem tviSchedules = new HuePropertyTreeViewItem() { Header = "schedules", Address = new HueAddress("/schedules"), IsSelected = false, FontStyle = FontStyles.Italic };
            foreach (KeyValuePair<string, Schedule> s in ds.schedules)
            {
                tviSchedules.Items.Add(BuildPropertiesTree(s.Value, tviSchedules.Address + $"/{s.Key}", WinHueSettings.settings.ShowID ? $"[{s.Key}] - {s.Value.name}" : $"{s.Value.name}", selectedpath));
            }
            tvi.Items.Add(tviSchedules);

            // SCENES
            HuePropertyTreeViewItem tviScenes = new HuePropertyTreeViewItem() { Header = "scenes", Address = new HueAddress("/scenes"), IsSelected = false, FontStyle = FontStyles.Italic };
            foreach (KeyValuePair<string, Scene> sc in ds.scenes)
            {
                if (!WinHueSettings.settings.ShowHiddenScenes && sc.Value.name.StartsWith("HIDDEN")) continue;
                tviScenes.Items.Add(BuildPropertiesTree(sc.Value, tviSchedules.Address + $"/{sc.Key}", WinHueSettings.settings.ShowID ? $"[{sc.Key}] - {sc.Value.name}" : $"{sc.Value.name}", selectedpath));
            }
            tvi.Items.Add(tviScenes);

            // RESOURCE LINKS
            HuePropertyTreeViewItem tviResourceLinks = new HuePropertyTreeViewItem() { Header = "ressource links", Address = new HueAddress("/resourcelinks"), IsSelected = false, FontStyle = FontStyles.Italic };
            foreach (KeyValuePair<string, Resourcelink> rl in ds.resourcelinks)
            {
                tviResourceLinks.Items.Add(BuildPropertiesTree(rl.Value, tviResourceLinks.Address + $"/{rl.Key}", WinHueSettings.settings.ShowID ? $"[{rl.Key}] - {rl.Value.name}" : $"{rl.Value.name}", selectedpath));
            }
            tvi.Items.Add(tviResourceLinks);

            //SENSORS
            HuePropertyTreeViewItem tviSensors = new HuePropertyTreeViewItem() { Header = "sensors", Address = new HueAddress("/sensors"), IsSelected = false, FontStyle = FontStyles.Italic };
            foreach (KeyValuePair<string, Sensor> sn in ds.sensors)
            {
                tviSensors.Items.Add(BuildPropertiesTree(sn.Value, tviSensors.Address + $"/{sn.Key}", WinHueSettings.settings.ShowID ? $"[{sn.Key}] - {sn.Value.name}" : $"{sn.Value.name}", selectedpath));
            }
            tvi.Items.Add(tviSensors);


            // CONFIG
            HuePropertyTreeViewItem tviConfig = BuildPropertiesTree(ds.config, "/config", "config", selectedpath);
            tvi.Items.Add(tviConfig);

            return tvi;
        }

    }
}
