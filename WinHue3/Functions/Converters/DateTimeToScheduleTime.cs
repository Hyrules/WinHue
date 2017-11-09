using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToScheduleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return value == null ? DateTime.Now : DateTime.Parse((string)value, CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value);
        }

    }
}
