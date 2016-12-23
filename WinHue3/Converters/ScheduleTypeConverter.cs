using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class ScheduleTypeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case "T":
                    return 0;
                case "PT":
                    return 1;
                case "W":
                    return 2;
                default:
                    return 0;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((int)value)
            {
                case 0:
                    return "T";
                case 1:
                    return "PT";
                case 2:
                    return "W";
                default:
                    return "T";
            }
        }
    }
}
