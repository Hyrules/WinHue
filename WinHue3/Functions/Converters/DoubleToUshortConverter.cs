using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Functions.Converters
{
    
    public class DoubleToUShortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return System.Convert.ToDouble(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return System.Convert.ToUInt16(value);

        }
    }
}
