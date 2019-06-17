using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Functions.Rules.Creator
{
   public static class TreeViewHelper
    {
        public static HuePropertyTreeViewItem BuildPropertiesTree(object root, string currentpath, string name = null, string selectedpath = null)
        {
            string obj = JsonConvert.SerializeObject(root, new JsonSerializerSettings(){NullValueHandling = NullValueHandling.Ignore,TypeNameHandling = TypeNameHandling.Objects});
            Dictionary<string, object> dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj);
            HuePropertyTreeViewItem roottvi = new HuePropertyTreeViewItem(){Header = name, PropType = root.GetType(), FontWeight = FontWeights.Normal, IsSelected = false, Address = new HueAddress(currentpath)};
            BuildTree(dic, roottvi, selectedpath);
            return roottvi;
        }

        private static void BuildTree(object item, HuePropertyTreeViewItem node, string selectedpath = null)
        {
            if (item is KeyValuePair<string, object> kvp)
            {
                if (kvp.Key == "Id" || kvp.Key == "Image") return;
                HuePropertyTreeViewItem tvi = new HuePropertyTreeViewItem() { Header = kvp.Key, PropType = kvp.Value.GetType(), FontWeight = FontWeights.Normal, IsSelected = false, Address = new HueAddress(node.Address + $"/{kvp.Key}")};
                if (tvi.Address == selectedpath)
                {
                    tvi.IsSelected = true;
                    ExpandSelectedNodes(node);
                }

                node.Items.Add(tvi);
                if (!IsPrimitive(kvp.Value))
                    BuildTree(kvp.Value, tvi);
            }
            else if (item is Dictionary<string, object> dic)
            {
                foreach (KeyValuePair<string, object> k in dic)
                {
                    BuildTree(k, node);
                }
                
            }
            else if(item is JToken obj)
            {
                if (IsPrimitive(obj)) return;
                Dictionary<string, object> tdic = obj.ToObject<Dictionary<string, object>>();
                
                foreach (KeyValuePair<string, object> tkvp in tdic)
                {
                    HuePropertyTreeViewItem tvi = new HuePropertyTreeViewItem() { Header = tkvp.Key, PropType = tkvp.Value.GetType(), FontWeight = FontWeights.Normal, IsSelected = false, Address = new HueAddress(node.Address + $"/{tkvp.Key}")};
                    if (tvi.Address == selectedpath)
                    {
                        tvi.IsSelected = true;
                        ExpandSelectedNodes(node);
                    }
                        

                    node.Items.Add(tvi);
                    if (IsPrimitive(tkvp.Value)) continue;
                    BuildTree(tkvp.Value,tvi);
                }
                
              

            }
        }

        private static bool IsPrimitive(object obj)
        {
            return (obj is null) || (obj is Array) || (obj.GetType().IsPrimitive) || (obj is string) || (obj is DateTime) || (obj is JArray);
        }

        private static void ExpandSelectedNodes(HuePropertyTreeViewItem node)
        {
            while (node.Parent != null)
            {
                node.IsExpanded = true;
                ExpandSelectedNodes(node.Parent as HuePropertyTreeViewItem);
            }
        }


    }
}
