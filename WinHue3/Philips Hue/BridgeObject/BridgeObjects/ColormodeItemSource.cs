using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    public class ColormodeItemSource : IItemsSource
    {
        /// <summary>
        /// Return the value.
        /// </summary>
        /// <returns>Value</returns>
        public ItemCollection GetValues()
        {
            ItemCollection colormode = new ItemCollection() { "hs", "xy", "ct" };
            return colormode;
        }
    }
}
