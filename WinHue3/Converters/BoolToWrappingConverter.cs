using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class BoolToWrappingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((TextWrapping)value)
            {
                case TextWrapping.NoWrap:
                    return false;
                case TextWrapping.Wrap:
                    return true;
                default:
                    return false;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }
    }
}
