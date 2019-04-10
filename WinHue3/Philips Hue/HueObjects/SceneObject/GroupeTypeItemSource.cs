using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.SceneObject
{
    public class GroupeTypeItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection types = new ItemCollection() { "LightScene", "GroupScene" };
            return types;
        }
    }
}
