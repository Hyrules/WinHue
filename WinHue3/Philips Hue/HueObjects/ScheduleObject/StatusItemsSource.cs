using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.HueObjects.ScheduleObject
{
    public class StatusItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection status = new ItemCollection() { "enabled","disabled"};
            return status;
        }
    }
}
