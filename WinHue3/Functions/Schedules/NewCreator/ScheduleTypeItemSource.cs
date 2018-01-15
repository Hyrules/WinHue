using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class ScheduleTypeItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection listTypes = new ItemCollection(){"Schedule","Timer", "Alarm" };
            return listTypes;
        }
    }
}
