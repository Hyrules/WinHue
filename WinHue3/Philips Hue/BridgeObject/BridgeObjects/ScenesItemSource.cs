using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    public class ScenesItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection listscenes = new ItemCollection();
            List<Scene> list = BridgeManager.Instance.SelectedBridge.GetListObjects<Scene>();
            foreach (Scene s in list)
            {
                listscenes.Add(s.Id);
            }
            return listscenes;
        }
    }
}
