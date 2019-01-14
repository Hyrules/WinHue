using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WinHue3.Functions.Converters
{
    public class TimeSpanToUintConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            TimeSpan? ts = (TimeSpan?) value;
            return System.Convert.ToUInt16(ts?.TotalMilliseconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            TimeSpan ts = TimeSpan.FromMilliseconds((uint)value);
            return ts;
        }
    }
}
