using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class RuleEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "enabled")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "enabled" : "disabled";
        }
    }
}
