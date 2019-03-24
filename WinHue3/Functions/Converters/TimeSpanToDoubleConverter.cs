using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Functions.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? new TimeSpan(0, 0, 0) : TimeSpan.FromSeconds((double) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            return ((TimeSpan) value).TotalSeconds;
        }

    }
}

