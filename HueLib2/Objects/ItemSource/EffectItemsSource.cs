using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HueLib2
{
    /// <summary>
    /// EffectItemsSource class
    /// </summary>
    public class EffectItemsSource : IItemsSource
    {
        /// <summary>
        /// Return the value.
        /// </summary>
        /// <returns>Value</returns>
        public ItemCollection GetValues()
        {
            ItemCollection effects = new ItemCollection() { "", "none", "colorloop" };
            return effects;
        }
    }
}
