using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WinHue3.Addons.Converter
{
    public class RssOperatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return 0;
            if ((string) value == string.Empty) return 0;
            switch ((string)value)
            {
                case "Contains":
                    return 0;
                case "Equals":
                    return 1;
                case "Greater":
                    return 2;
                case "Lower":
                    return 3;
                default:
                    return 0;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            switch ((int)value)
            {
                case 0:
                    return "Contains";
                case 1:
                    return "Equals";
                case 2:
                    return "Greater";
                case 3:
                    return "Lower";
                default:
                    return "Contains";
            }

        }
    }
}
