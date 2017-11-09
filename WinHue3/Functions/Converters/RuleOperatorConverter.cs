using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class RuleOperatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            switch (value.ToString())
            {
                case "eq":
                    return 0;
                case "gt":
                    return 1;
                case "lt":
                    return 2;
                case "dx":
                    return 3;
                case "ddx":
                    return 4;
                case "stable":
                    return 5;
                case "not stable":
                    return 6;
                case "in":
                    return 7;
                case "not in":
                    return 8;
                default:
                    return 0;

            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = System.Convert.ToInt32(value);
            switch (index)
            {
                case 0:
                    return "eq";
                case 1:
                    return "gt";
                case 2:
                    return "lt";
                case 3:
                    return "dx";
                case 4:
                    return "ddx";
                case 5:
                    return "stable";
                case 6:
                    return "not stable";
                case 7:
                    return "in";
                case 8:
                    return "not in";
                default:
                    return string.Empty;

            }

        }
    }
}
