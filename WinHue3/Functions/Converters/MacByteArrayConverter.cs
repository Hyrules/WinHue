using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WinHue3.Functions.Converters
{
    public class MacByteArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return BitConverter.ToString(((byte[]) value).Reverse().ToArray(), 0, 6);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new byte[6];
            return PhysicalAddress.Parse(value.ToString()).GetAddressBytes().Reverse();
        }
    }
}
