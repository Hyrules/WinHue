using System.Globalization;
using System.Windows.Data;
using HueLib2;
using WinHue3.Resources;

namespace WinHue3
{
    /// <summary>
    /// Bridge object Type description.
    /// </summary>
    public class TypeGroupDescription : PropertyGroupDescription
    {
        /// <summary>
        /// Create the group name from Item.
        /// </summary>
        /// <param name="item">Selected Item.</param>
        /// <param name="level">Level of the Item (Not used)</param>
        /// <param name="culture">Desired culture.</param>
        /// <returns></returns>
        public override object GroupNameFromItem(object item, int level, CultureInfo culture)
        {
            if (item == null) return GUI.ListView_Other;
            if(item.GetType().BaseType == typeof(HueObject)) return GUI.ResourceManager.GetString("ListView_" + item.GetType().Name);
            return GUI.ResourceManager.GetString("ListView_" + item.GetType().BaseType.Name);

        }
    }
}
