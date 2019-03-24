using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Org.BouncyCastle.Asn1.Cms;

namespace WinHue3.Functions.Converters
{
    public class TimeSpanToUShortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return TimeSpan.FromMilliseconds((ushort)value *100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return System.Convert.ToUInt16(((TimeSpan) value).TotalMilliseconds / 100);
        }
    }
}
