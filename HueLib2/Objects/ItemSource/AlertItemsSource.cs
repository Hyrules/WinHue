using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <Summary>
    /// AlertItemsSource class
    /// </Summary>
    public class AlertItemsSource : IItemsSource
    {
        /// <summary>
        /// Returm the value
        /// </summary>
        /// <returns>Value</returns>
        public ItemCollection GetValues()
        {
            ItemCollection types = new ItemCollection() {string.Empty,"none","select","lselect"};
            return types;
        }
    }
}
