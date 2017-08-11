using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DateTime.Now.Date;
            return DateTime.Parse((string)value,CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return ((DateTime)value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
