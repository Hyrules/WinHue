using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Functions.Converters
{
    public class SliderToXYConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            return System.Convert.ToDecimal(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double) value == -1) return null;
            return System.Convert.ToDouble(value);
        }
    }
}
