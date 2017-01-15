using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class GroupTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = value.ToString();
            switch (type)
            {
                case "LightGroup":
                    return 0;
                case "Room":
                    return 1;
                default:
                    return 0;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            switch (index)
            {
                case 0:
                    return "LightGroup";
                case 1:
                    return "Room";
                default:
                    return "LightGroup";
            }

        }
    }
}
