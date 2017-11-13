using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WinHue3.ExtensionMethods;

namespace WinHue3.Functions.Rules.Creator
{
   public static class TreeViewHelper
    {
        public static TreeViewItem BuildPropertiesTree(object root, string currentpath, string selectedpath = null)
        {
            PropertyInfo[] listprops = root.GetArrayHueProperties();
            TreeViewItem tvi = new TreeViewItem() { IsSelected = false};

            foreach (PropertyInfo p in listprops)
            {
                string actualpath = currentpath + "/" + p.Name;
                object value = p.GetValue(root);

                if (value == null || !value.GetType().HasHueProperties())
                {

                    tvi.Items.Add(new TreeViewItem() { Header = p.Name, Tag = actualpath, IsSelected = selectedpath == actualpath});
                }
                else
                {
                    TreeViewItem ttvi = BuildPropertiesTree(value, actualpath);
                    ttvi.Header = p.Name;
                    ttvi.Tag = actualpath;
                    tvi.Items.Add(ttvi);

                }
            }

            return tvi;
        }

    }
}
