using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Reflection;

namespace WinHue3.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToScheduleConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.Parse((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}
