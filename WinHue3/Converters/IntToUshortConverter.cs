using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Converters
{
    class IntToUshortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            return System.Convert.ToInt32(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((int)value == -1) return null;
            return System.Convert.ToUInt16(value);

        }
    }
}
