using System;
using System.Globalization;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class SelectedActionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            switch (value.ToString())
            {
                case "lights":
                    return 0;
                case "groups":
                    return 1;
                case "scenes":
                    return 2;
                case "sensors":
                    return 3;
                case "schedules":
                    return 4;
                default:
                    return -1;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            int index = (int)value;
            switch (index)
            {
                case 0:
                    return "lights";
                case 1:
                    return "groups";
                case 2:
                    return "scenes";
                case 3:
                    return "sensors";
                case 4:
                    return "schedules";
                default:
                    return string.Empty;
            }
        }
    }
}
