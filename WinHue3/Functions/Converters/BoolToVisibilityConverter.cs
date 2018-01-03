using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinHue3.Functions.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return Visibility.Collapsed;
            bool val = (bool) value;

            if (parameter is string)
            {
                if (parameter.ToString() == "reverse")
                {
                    val = !val;
                }
            }

            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility)) return false;
            bool val = (Visibility)value == Visibility.Visible;

            if (parameter is string)
            {
                if (parameter.ToString() == "reverse")
                {
                    val = !val;
                }
            }

            return val;
        }
    }
}