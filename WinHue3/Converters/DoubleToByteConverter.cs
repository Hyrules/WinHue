using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class DoubleToByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return -1;
            return System.Convert.ToDouble(value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value == -1) return null;
            return System.Convert.ToByte(value);

        }
    }
}
