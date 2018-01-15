using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Functions.Schedules.NewCreator
{
    public class EnabledItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection listenabled = new ItemCollection() { "enabled", "disabled"};
            return listenabled;
        }
    }
}
